#region Copyright Notice
// This file is part of CrystalMPQ.
// 
// Copyright (C) 2007-2011 Fabien BARBIER
// 
// CrystalMPQ is licenced under the Microsoft Reciprocal License.
// You should find the licence included with the source of the program,
// or at this URL: http://www.microsoft.com/opensource/licenses.mspx#Ms-RL
#endregion

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
#if DEBUG
using System.Diagnostics;
#endif

namespace CrystalMpq
{
	/// <summary>Exposes <see cref="Stream"/> with the data contained in an <see cref="MpqFile"/>.</summary>
	public sealed class MpqFileStream : Stream
	{
		#region Patch Headers

		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct PatchInfoHeader
		{
			public uint HeaderLength;
			public uint Flags;
			public uint PatchLength;
			public fixed byte PatchMD5[16];
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct PatchHeader
		{
			public uint Signature;
			public uint PatchLength;
			public uint OriginalFileSize;
			public uint PatchedFileSize;
		}

		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct PatchMD5ChunkData
		{
			public fixed byte OrginialFileMD5[16];
			public fixed byte PatchedFileMD5[16];
		}

		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct PatchBsdiff40Header
		{
			public ulong Signature;
			public ulong ControlBlockLength;
			public ulong DifferenceBlockLength;
			public ulong PatchedFileSize;
		}

		#endregion

		private MpqFile file;
		private uint lastBlockLength;
		private long position;
		int currentBlock;
		int readBufferOffset;
		private byte[] blockBuffer;
		private byte[] compressedBuffer;
		private uint[] fileHeader;
		private long offset;
		private uint length;
		private uint seed;

		internal MpqFileStream(MpqFile file, Stream baseStream = null)
		{
			try
			{
				PatchInfoHeader? patchInfoHeader; // Used to differentiate between regulat files and patch files. Also contains the patch header :p

				// Store bits of information as local variables, in order to adjust them later.
				bool singleUnit = (file.Flags & MpqFileFlags.SingleBlock) != 0;
				bool compressed = file.IsCompressed;
				uint compressedSize = (uint)file.CompressedSize;

				this.file = file;
				this.offset = file.Offset;

				// Process the patch information header first

				if (file.IsPatch)
				{
					// Resolving the base file this early may be a waste if the patch ever happens to be a COPY patch… Anyway, it allows for checking the base file's integrity.
					// But seriously, what's the point in COPY patches anyway ? Aren't those just like regular MPQ files, only with added (useless) weight ?
					if ((baseStream = baseStream ?? file.Archive.ResolveBaseFileInternal(file)) == null)
						throw new FileNotFoundException(string.Format(ErrorMessages.GetString("PatchBaseFileNotFound"), file.Name));

					patchInfoHeader = ReadPatchInfoHeader(file.Archive, file.Offset);

					offset += patchInfoHeader.Value.HeaderLength;
					length = patchInfoHeader.Value.PatchLength; // No matter what crap may be written in the block table, it seems that this field is always right (I had to update the decompression method just for that…)

					if (patchInfoHeader.Value.PatchLength <= file.CompressedSize)
					{
						// As it seems, there are some bogus entries in the block table of mpq patch archives. (Only for patch files though)
						// If you browse the list of DBC files, i'd say there are about 10% of them which have a bogus block table entry.
						// So, for detecting them, we'll use the same method as in stormlib. We'll try to read the patch header to know is the patch is compressed or not.

						// By the way, we cannot detect whether the patch is compressed or not if it is encrypted.
						if (file.IsEncrypted) throw new InvalidDataException(ErrorMessages.GetString("PatchInfoHeaderInvalidData"));

						// Try to read the patch header in the data following the information header and adjust the compressed size dependign on the result:
						// Since we are “sure” of the uncompressed size (given in the patch header), there is no point in compression if the compressed data isn't even one byte less.
						// Thus, we can mostly safely decrease the compressed size by 1, which, by the way, is necessary to make decompression work in UpdateBuffer()…
						compressedSize = patchInfoHeader.Value.PatchLength - ((compressed = !TestPatchHeader(file.Archive, offset)) ? (uint)1 : 0);

						// It appears that the single unit flag is also lying on some patch entries. Files reported as blocky (such as some of the cataclysm mp3) are in fact single unit…
						// Forcing this single unit flag to true when the file is compressed seems to be a good solution. Also, we may (or not :p) save a bit of memory by using blocks for uncompressed files.
						singleUnit = compressed;
					}
				}
				else
				{
					patchInfoHeader = null;
					length = checked((uint)file.Size);
				}

				// Set up the stream the same way for both patches and regular files…

				if (file.IsEncrypted)
				{
					if (file.Seed == 0) throw new SeedNotFoundException(file.BlockIndex);
					else this.seed = file.Seed;

					if ((file.Flags & MpqFileFlags.PositionEncrypted) != 0)
						this.seed = (this.seed + (uint)file.Offset) ^ (uint)this.length;
				}

				if (singleUnit)
					this.fileHeader = new uint[] { 0, compressedSize };
				else if (compressed)
					this.fileHeader = ReadBlockOffsets(file.Archive, this.seed, this.offset, (int)((length + file.Archive.BlockSize - 1) / file.Archive.BlockSize + 1));
				else
				{
					this.fileHeader = new uint[(int)(length + file.Archive.BlockSize - 1) / file.Archive.BlockSize + 1];
					this.fileHeader[0] = 0;
					for (int i = 1; i < this.fileHeader.Length; i++)
					{
						this.fileHeader[i] = this.fileHeader[i - 1] + (uint)file.Archive.BlockSize;
						if (this.fileHeader[i] > length) this.fileHeader[i] = (uint)this.length;
					}
				}
				
				// Treat the files smaller than the block size as single unit. (But only now that we've read the file header)
				singleUnit |= length <= file.Archive.BlockSize;

				this.blockBuffer = new byte[singleUnit ? length : (uint)file.Archive.BlockSize];
				if (compressed) this.compressedBuffer = new byte[singleUnit ? compressedSize : (uint)file.Archive.BlockSize];
				this.lastBlockLength = this.length > 0 ? this.length % (uint)this.blockBuffer.Length : 0;
				if (this.lastBlockLength == 0) this.lastBlockLength = (uint)this.blockBuffer.Length;
				this.currentBlock = -1;

				UpdateBuffer();

				// If we finished initializing a stream to patch data, all there is left is to apply the patch
				if (patchInfoHeader != null)
				{
					// The patching methods will read from this stream instance (whose constructor has yet to finish… !) and return the patched data.
					this.blockBuffer = ApplyPatch(patchInfoHeader.Value, baseStream);
					// Once the patch has been applied, transform this stream into a mere memory stream. (The same as with single unit files, in fact)
					this.compressedBuffer = null;
					this.fileHeader = new uint[] { 0, (uint)this.blockBuffer.Length };
					this.position = 0;
					this.currentBlock = 0;
					this.readBufferOffset = 0;
					this.length = (uint)this.blockBuffer.Length;
				}
			}
			finally { if (baseStream != null) baseStream.Dispose(); }
		}

		private static unsafe bool TestPatchHeader(MpqArchive archive, long offset)
		{
			var sharedBuffer = CommonMethods.GetSharedBuffer(4);

			if (archive.ReadArchiveData(sharedBuffer, 0, offset, 4) != 4) throw new EndOfStreamException();

			return sharedBuffer[0] == 0x50 && sharedBuffer[1] == 0x54 && sharedBuffer[2] == 0x43 && sharedBuffer[3] == 0x48;
		}

		private static unsafe PatchInfoHeader ReadPatchInfoHeader(MpqArchive archive, long offset)
		{
			// Always get a buffer big enough, even if the extra bytes are not present…
			// As of now (09/2011), the header should always be 28 bytes long, but this may change in the future…
			var sharedBuffer = CommonMethods.GetSharedBuffer(sizeof(PatchInfoHeader));

			// No buffer should ever be smaller than 28 bytes… right ?
			if (archive.ReadArchiveData(sharedBuffer, 0, offset, 28) != 28)
				throw new EndOfStreamException(ErrorMessages.GetString("PatchInfoHeaderEndOfStream")); // It's weird if we could not read the whole 28 bytes… (At worse, we should have read trash data)

			var patchInfoHeader = new PatchInfoHeader();

			patchInfoHeader.HeaderLength = (uint)sharedBuffer[0] | (uint)sharedBuffer[1] << 8 | (uint)sharedBuffer[2] << 16 | (uint)sharedBuffer[3] << 24;
			patchInfoHeader.Flags = (uint)sharedBuffer[4] | (uint)sharedBuffer[5] << 8 | (uint)sharedBuffer[6] << 16 | (uint)sharedBuffer[7] << 24;
			patchInfoHeader.PatchLength = (uint)sharedBuffer[8] | (uint)sharedBuffer[9] << 8 | (uint)sharedBuffer[10] << 16 | (uint)sharedBuffer[11] << 24;

			// Let's assume the MD5 is not mandatory…
			if (patchInfoHeader.HeaderLength >= 28)
				for (int i = 0; i < 16; i++) patchInfoHeader.PatchMD5[i] = sharedBuffer[12 + i];

			return patchInfoHeader;
		}

		private static unsafe uint[] ReadBlockOffsets(MpqArchive archive, uint hash, long offset, int count)
		{
			int length = count * sizeof(uint);
			var sharedBuffer = CommonMethods.GetSharedBuffer(length);

			if (archive.ReadArchiveData(sharedBuffer, 0, offset, length) != length) throw new EndOfStreamException();

			var offsets = new uint[count];

			Buffer.BlockCopy(sharedBuffer, 0, offsets, 0, length);

			if (!BitConverter.IsLittleEndian) CommonMethods.SwapBytes(offsets);

			// If hash is valid, decode the header
			if (hash != 0) unchecked { CommonMethods.Decrypt(offsets, hash - 1); }

			return offsets;
		}

		private unsafe byte[] ApplyPatch(PatchInfoHeader patchInfoHeader, Stream baseStream)
		{
			PatchHeader patchHeader;

			Read((byte*)&patchHeader, sizeof(PatchHeader));
			if (!BitConverter.IsLittleEndian) CommonMethods.SwapBytes((uint*)&patchHeader, sizeof(PatchHeader) >> 2);

			if (patchHeader.Signature != 0x48435450 /* 'PTCH' */) throw new InvalidDataException(ErrorMessages.GetString("PatchHeaderInvalidSignature"));
			if (patchHeader.PatchedFileSize != file.Size) throw new InvalidDataException(ErrorMessages.GetString("PatchHeaderInvalidFileSize"));
			if (baseStream.Length != patchHeader.OriginalFileSize) throw new InvalidDataException(ErrorMessages.GetString("PatchHeaderInvalidBaseFileSize"));

			// Once the initial tests are passed, we can load the whole patch in memory.
			// This will take a big amount of memory, but will avoid having to unpack the file twice…

			var originalData = new byte[baseStream.Length];
			if (baseStream.Read(originalData, 0, originalData.Length) != originalData.Length) throw new EndOfStreamException();

			var md5 = CommonMethods.SharedMD5;

			var originalHash = md5.ComputeHash(originalData);

			PatchMD5ChunkData md5ChunkData;
			bool hasMD5 = false;
			
			while (true)
			{
				long chunkPosition = Position;
				var chunkHeader = stackalloc uint[2];

				if (Read((byte*)chunkHeader, 8) != 8) throw new EndOfStreamException();
				if (!BitConverter.IsLittleEndian) CommonMethods.SwapBytes(chunkHeader, 2);

				if (chunkHeader[0] == 0x5F35444D /* 'MD5_' */)
				{
					if (Read((byte*)&md5ChunkData, sizeof(PatchMD5ChunkData)) != sizeof(PatchMD5ChunkData)) throw new EndOfStreamException();

					if (!CommonMethods.CompareData(originalHash, md5ChunkData.OrginialFileMD5)) throw new InvalidDataException(ErrorMessages.GetString("PatchBaseFileMD5Failed"));

					hasMD5 = true;
				}
				else if (chunkHeader[0] == 0x4D524658 /* 'XFRM' */)
				{
					// This may not be a real problem, however, let's not handle this case for now… (May fail because of the stupid bogus patches…)
					if (chunkPosition + chunkHeader[1] != Length) throw new InvalidDataException(ErrorMessages.GetString("PatchXfrmChunkError"));

					uint patchType;

					if (Read((byte*)&patchType, 4) != 4) throw new EndOfStreamException();
					if (!BitConverter.IsLittleEndian) patchType = CommonMethods.SwapBytes(patchType);

					uint patchLength = chunkHeader[1] - 12;

					byte[] patchedData;

					if (patchType == 0x59504F43 /* 'COPY' */) patchedData = ApplyCopyPatch(ref patchInfoHeader, ref patchHeader, patchLength, originalData);
					if (patchType == 0x30445342 /* 'BSD0' */) patchedData = ApplyBsd0Patch(ref patchInfoHeader, ref patchHeader, patchLength, originalData);
					else throw new NotSupportedException("Unsupported patch type: '" + CommonMethods.FourCCToString(chunkHeader[0]) + "'");

					if (hasMD5)
					{
						var patchedHash = md5.ComputeHash(patchedData);

						if (!CommonMethods.CompareData(patchedHash, md5ChunkData.PatchedFileMD5)) throw new InvalidDataException("PatchFinalFileMD5Failed");
					}

					return patchedData;
				}
				else throw new InvalidDataException(string.Format(ErrorMessages.GetString("PatchUnknownChunk"), CommonMethods.FourCCToString(chunkHeader[0])));

				Seek(chunkPosition + chunkHeader[1], SeekOrigin.Begin);
			}
		}

		private byte[] ApplyCopyPatch(ref PatchInfoHeader patchInfoHeader, ref PatchHeader patchHeader, uint patchLength, byte[] originalData)
		{
			if (patchLength != patchHeader.PatchedFileSize) throw new InvalidDataException("CopyPatchInvalidSize");

			var patchedData = patchLength == originalData.Length ? originalData : new byte[patchLength];

			if (Read(patchedData, 0, patchedData.Length) != patchedData.Length) throw new EndOfStreamException();

			return patchedData;
		}

		private unsafe byte[] ApplyBsd0Patch(ref PatchInfoHeader patchInfoHeader, ref PatchHeader patchHeader, uint patchLength, byte[] originalData)
		{
			byte[] patchData;

			if (patchLength < patchHeader.PatchLength) patchData = UnpackRle();
			else
			{
				patchData = new byte[patchLength];
				if (Read(patchData, 0, checked((int)patchLength)) != patchLength) throw new EndOfStreamException();
			}

			fixed (byte* patchDataPointer = patchData)
			{
				var bsdiffHeader = (PatchBsdiff40Header*)patchDataPointer;

				if (!BitConverter.IsLittleEndian) CommonMethods.SwapBytes((ulong*)patchDataPointer, sizeof(PatchBsdiff40Header) >> 3);

				if (bsdiffHeader->Signature != 0x3034464649445342 /* 'BSDIFF40' */) throw new InvalidDataException(ErrorMessages.GetString("Bsd0PatchHeaderInvalidSignature"));

				var controlBlock = (uint*)(patchDataPointer + sizeof(PatchBsdiff40Header));
				var differenceBlock = (byte*)controlBlock + bsdiffHeader->ControlBlockLength;
				var extraBlock = differenceBlock + bsdiffHeader->DifferenceBlockLength;

				if (!BitConverter.IsLittleEndian) CommonMethods.SwapBytes(controlBlock, bsdiffHeader->ControlBlockLength >> 2);

				var patchBuffer = new byte[bsdiffHeader->PatchedFileSize];

				fixed (byte* originalDataPointer = originalData)
				fixed (byte* patchBufferPointer = patchBuffer)
				{
					var sourcePointer = originalDataPointer;
					var destinationPointer = patchBufferPointer;
					int sourceCount = originalData.Length;
					int destinationCount = patchBuffer.Length;

					while (destinationCount != 0)
					{
						uint differenceLength = *controlBlock++;
						uint extraLength = *controlBlock++;
						uint sourceOffset = *controlBlock++;

						if (differenceLength > destinationCount) throw new InvalidDataException(ErrorMessages.GetString("Bsd0PatchInvalidData"));
						destinationCount = (int)(destinationCount - differenceLength);

						// Apply the difference patch (Patched Data = Original data + Difference data)
						for (; differenceLength-- != 0; destinationPointer++, sourcePointer++)
						{
							*destinationPointer = *differenceBlock++;
							if (sourceCount > 0) *destinationPointer += *sourcePointer;
						}

						if (extraLength > destinationCount) throw new InvalidDataException(ErrorMessages.GetString("Bsd0PatchInvalidData"));
						destinationCount = (int)(destinationCount - extraLength);

						// Apply the extra data patch (New data)
						for (; extraLength-- != 0; ) *destinationPointer++ = *extraBlock++;

						sourcePointer += (sourceOffset & 0x80000000) != 0 ? unchecked((int)(0x80000000 - sourceOffset)) : (int)sourceOffset;
					}
				}

				return patchBuffer;
			}
		}

		private unsafe byte[] UnpackRle()
		{
			uint length = this.ReadUInt32();

			var decompressionBuffer = new byte[length];

			int i = 0;

			while (i < length)
			{
				int @byte = ReadByte();

				if (@byte == -1) goto Finish;

				if ((@byte & 0x80) != 0)
				{
					for (int j = (@byte & 0x7F) + 1; j-- != 0 && i < length; )
					{
						@byte = ReadByte();

						if (@byte == -1) goto Finish;

						decompressionBuffer[i++] = (byte)@byte;
					}
				}
				else i += @byte + 1;
			}

		Finish: ;
			return decompressionBuffer;
		}

		public sealed override bool CanRead { get { return true; } }
		public sealed override bool CanWrite { get { return false; } }
		public sealed override bool CanSeek { get { return true; } }

		public sealed override long Position
		{
			get { return position; }
			set
			{
				if (position < 0 || position > length) throw new ArgumentOutOfRangeException("value");
				position = (int)value;
				UpdateBuffer();
			}
		}

		public sealed override long Length { get { return length; } }

		public MpqFile File { get { return file; } }

		public sealed override int ReadByte()
		{
			if (position >= length) return -1;

			if (readBufferOffset >= blockBuffer.Length) UpdateBuffer();

			position++;

			return blockBuffer[readBufferOffset++];
		}

		/// <summary>Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
		public unsafe sealed override int Read(byte[] buffer, int offset, int count)
		{
			if (offset < 0 || offset > buffer.Length) throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || checked(offset + count) > buffer.Length) throw new ArgumentOutOfRangeException("count");

			fixed (byte* bufferPointer = buffer)
				return Read(bufferPointer + offset, count);
		}

		[CLSCompliant(false)]
		public unsafe int Read(byte* buffer, int count)
		{
			if (count < 0) throw new ArgumentOutOfRangeException("count");

			if (position < 0) return 0;
			if (position + count > length) count = (int)(length - (uint)position);

			fixed (byte* readBufferPointer = blockBuffer)
			{
				var destinationPointer = buffer;
				var sourcePointer = readBufferPointer + readBufferOffset;

				for (int i = count; i-- != 0; readBufferOffset++, position++)
				{
					if (readBufferOffset >= blockBuffer.Length)
					{
						UpdateBuffer();
						sourcePointer = readBufferPointer + readBufferOffset;
					}
					*destinationPointer++ = *sourcePointer++;
				}
			}

			return count;
		}

		public sealed override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }

		public sealed override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin: position = (int)offset; break;
				case SeekOrigin.Current: position += (int)offset; break;
				case SeekOrigin.End: position = (int)(Length + offset); break;
				default: throw new ArgumentOutOfRangeException("origin");
			}

			UpdateBuffer();

			return position;
		}

		public sealed override void SetLength(long value) { throw new NotSupportedException(); }

		public sealed override void Flush() { }

		public sealed override void Close()
		{
			base.Close();

			blockBuffer = null;
			compressedBuffer = null;
		}

		private void UpdateBuffer()
		{
			if (position < 0 || position >= length) return;

			int newBlock = (int)(position / blockBuffer.Length);

			if (currentBlock != newBlock)
			{
				ReadBlock(newBlock);
				currentBlock = newBlock;
			}

			readBufferOffset = (int)(position % blockBuffer.Length);
		}

		private unsafe void ReadBlock(int block)
		{
			int length = (int)(fileHeader[block + 1] - fileHeader[block]);
			bool last = block == fileHeader.Length - 2;
			bool compressed = !(length == file.Archive.BlockSize || last && (uint)length == lastBlockLength);
			var buffer = compressed ? compressedBuffer : blockBuffer;

			file.Archive.ReadArchiveData(buffer, 0, offset + fileHeader[block], length);

			if (file.IsEncrypted)
			{
				// If last bytes don't fit in an uint, then they won't be encrypted/decrypted
				// Therefore we just leave "length" here as a parameter and bits 0..1 will be cut
				CommonMethods.Decrypt(buffer, seed + (uint)block, length);
			}

			if (compressed)
			{
				int byteCount;

				// Check the advanced compression scheme first, as it is the only used in modern games.
				if ((file.Flags & MpqFileFlags.MultiCompressed) != 0)
					byteCount = CommonMethods.DecompressBlock(compressedBuffer, length, blockBuffer, true);
				else /*if ((file.Flags & MpqFileFlags.DclCompressed) != 0)*/
					byteCount = CommonMethods.DecompressBlock(compressedBuffer, length, blockBuffer, false);

				if (byteCount != (last ? lastBlockLength : (uint)blockBuffer.Length)) throw new InvalidDataException();
			}

			// As an added bonus, clear the reference to the compressed data buffer once we don't need it anymore
			if (this.blockBuffer.Length == this.length) this.compressedBuffer = null;
		}
	}
}
