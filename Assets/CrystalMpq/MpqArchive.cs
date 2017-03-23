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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CrystalMpq
{
	/// <summary>
	/// This class is used to read MPQ archives.
	/// It gives you access to all files contained in the archive.
	/// </summary>
	public partial class MpqArchive : IDisposable
	{
		#region MpqFileEnumerator Structure

		public struct MpqFileEnumerator : IEnumerator<MpqFile>
		{
			private MpqFile[] files;
			private int index;

			internal MpqFileEnumerator(MpqFile[] files)
			{
				this.files = files;
				this.index = -1;
			}

			public MpqFile Current { get { return files[index]; } }

			object IEnumerator.Current { get { return Current; } }

			public void Dispose() { files = null; }

			public bool MoveNext() { return index < files.Length && ++index < files.Length; }

			public void Reset() { index = -1; }
		}

		#endregion

		#region MpqFileCollection Class

		/// <summary>Represents a collection of <see cref="MpqFile"/> in an <see cref="MpqArchive"/>.</summary>
		public sealed class MpqFileCollection : IList<MpqFile>
		{
			private MpqArchive archive;

			internal MpqFileCollection(MpqArchive archive)
			{
				if (archive == null) throw new ArgumentNullException("archive");

				this.archive = archive;
			}

			/// <summary>Gets a file from the collection.</summary>
			/// <param name="index">Index of the desired <see cref="MpqFile"/> item.</param>
			/// <returns>Returns the <see cref="MpqFile"/> at the specified index.</returns>
			public MpqFile this[int index] { get { return archive.files[index]; } }

			MpqFile IList<MpqFile>.this[int index]
			{
				get { return this[index]; }
				set { throw new NotSupportedException(); }
			}

			/// <summary>Gets the number of <see cref="MpqFile"/> items in the collection.</summary>
			public int Count { get { return archive.files.Length; } }

			bool ICollection<MpqFile>.IsReadOnly { get { return true; } }

			#region Update Methods

			void ICollection<MpqFile>.Add(MpqFile item) { throw new NotSupportedException(); }
			void IList<MpqFile>.Insert(int index, MpqFile item) { throw new NotSupportedException(); }

			bool ICollection<MpqFile>.Remove(MpqFile item) { throw new NotSupportedException(); }
			void IList<MpqFile>.RemoveAt(int index) { throw new NotSupportedException(); }

			void ICollection<MpqFile>.Clear() { throw new NotSupportedException(); }

			#endregion

			#region Enumeration Methods

			/// <summary>Gets an enumerator for the collection.</summary>
			/// <returns>Returns an enumerator for the current collection.</returns>
			public MpqFileEnumerator GetEnumerator() { return new MpqFileEnumerator(archive.files); }
			IEnumerator<MpqFile> IEnumerable<MpqFile>.GetEnumerator() { return GetEnumerator(); }
			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

			#endregion

			bool ICollection<MpqFile>.Contains(MpqFile item) { return ((IList<MpqFile>)archive.files).Contains(item); }
			int IList<MpqFile>.IndexOf(MpqFile item) { return ((IList<MpqFile>)archive.files).IndexOf(item); }

			void ICollection<MpqFile>.CopyTo(MpqFile[] array, int arrayIndex) { archive.files.CopyTo(array, arrayIndex); }
		}

		#endregion

		#region Static Fields

		internal static readonly uint HashTableHash = CommonMethods.Hash("(hash table)", 0x300);
		internal static readonly uint BlockTableHash = CommonMethods.Hash("(block table)", 0x300);
		internal const string ListFileName = "(listfile)";
		internal const string AttributesFileName = "(attributes)";
		internal const string WeakSignatureFileName = "(signature)";
		internal const string UserDataFileName = "(user data)";
		internal const uint MpqArchiveSignature = 0x1A51504D;
		internal const uint MpqUserDataSignature = 0x1B51504D;
		internal const uint MpqStrongSignatureSignature = 0x5349474E;

		#endregion

		private Stream stream;
		private long userDataOffset;
		private long userDataLength;
		private long archiveDataOffset;
		private long archiveDataLength;
		private long headerSize;
		private int blockSize;
		private MpqFormat archiveFormat;
		private MpqHashTable hashTable;
		internal MpqBlockTable blockTable;
		private MpqFile[] files;
		private readonly MpqFileCollection fileCollection;
		private MpqFile listFile;
		private MpqFile attribFile;
		private bool listFileParsed;
		private bool attribFileParsed;
		private bool hasStrongSignature;
		private readonly ResolveStreamEventArgs resolveStreamEventArgs;
		private readonly object syncRoot;

		/// <summary>Occurs when the base file for a given <see cref="MpqFile"/> should be resolved.</summary>
		/// <remarks>
		/// This event will be raised when opening an <see cref="MpqFile"/> which is a patch file.
		/// Because patch files should be applied to a base file, it is needed to access to this file.
		/// The application is responsible for providing a stream containing valid data for this to work.
		/// </remarks>
		public event EventHandler<ResolveStreamEventArgs> ResolveBaseFile;

		#region Instance Constructors

		private MpqArchive()
		{
			this.syncRoot = new object();
			this.resolveStreamEventArgs = new ResolveStreamEventArgs();
			this.fileCollection = new MpqFileCollection(this);
			this.listFileParsed = false;
			this.attribFileParsed = false;
		}

		/// <summary>Initializes a new instance of the <see cref="MpqArchive"/> class.</summary>
		/// <remarks>
		/// The file is opened for read access and shares read access.
		/// For safety, it is impossible to write to the file as long as it is open.
		/// If you wish to allow write access to the file (at your own risk), please use one of the constructors taking a <see cref="Stream"/>.
		/// The listfile will be parsed if present in the archive.
		/// </remarks>
		/// <param name="filename">The MPQ archive's filename.</param>
		/// <exception cref="InvalidDataException">The specified file is not a valid MPQ archive, or the archive is corrupt.</exception>
		public MpqArchive(string filename)
			: this(filename, true) { }

		/// <summary>Initializes a new instance of the <see cref="MpqArchive"/> class.</summary>
		/// <remarks>
		/// The file is opened for read access and shares read access.
		/// For safety, it is impossible to write to the file as long as it is open.
		/// If you wish to allow write access to the file (at your own risk), please use one of the constructors taking a <see cref="Stream"/>.
		/// </remarks>
		/// <param name="filename">The MPQ archive's filename.</param>
		/// <param name="shouldParseListFile">Determines if the listfile will be parsed.</param>
		/// <param name="nameCache">The <see cref="MpqNameCache"/> to use for caching filenames.</param>
		/// <exception cref="InvalidDataException">The specified file is not a valid MPQ archive, or the archive is corrupt.</exception>
		public MpqArchive(string filename, bool shouldParseListFile)
			: this()
		{
			if (filename == null) throw new ArgumentNullException("filename");

			OpenInternal(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read), shouldParseListFile);
		}

		/// <summary>Initializes a new instance of the <see cref="MpqArchive"/> class.</summary>
		/// <remarks>The listfile will be parsed if present.</remarks>
		/// <param name="stream">A <see cref="Stream"/> containing the MPQ archive.</param>
		/// <exception cref="InvalidDataException">The specified stream does not contain a valid MPQ archive, or the archive is corrupt.</exception>
		public MpqArchive(Stream stream)
			: this(stream, true) { }

		/// <summary>Initializes a new instance of the <see cref="MpqArchive"/> class.</summary>
		/// <param name="stream">A <see cref="Stream"/> containing the MPQ archive.</param>
		/// <param name="shouldParseListFile">Determines if the listfile will be parsed.</param>
		/// <exception cref="InvalidDataException">The specified stream does not contain a valid MPQ archive, or the archive is corrupt.</exception>
		public MpqArchive(Stream stream, bool shouldParseListFile)
			: this() { OpenInternal(stream, shouldParseListFile); }

		#endregion

		public void Dispose() { Close(); }

		public void Close()
		{
			if (stream != null)
				lock (syncRoot)
				{
					if (stream != null)
						stream.Close();
					stream = null;
				}
		}

		private void OpenInternal(Stream stream, bool shouldParseListFile)
		{
			// MPQ offsets can be 32 bits, 48 bits or 64 bits depending on the MPQ version used…
			long hashTableOffset, hashTableCompressedSize, hashTableSize;
			long blockTableOffset, blockTableCompressedSize, blockTableSize;
			long highBlockTableOffset, highBlockTableCompressedSize, highBlockTableSize;
			long enhancedHashTableOffset, enhancedHashTableCompressedSize;
			long enhancedBlockTableOffset, enhancedBlockTableCompressedSize;
			uint hashTableLength, blockTableLength;
			uint rawChunkSize;
			uint signature;
			byte[] hashes;

			if (!stream.CanSeek) throw new InvalidOperationException(ErrorMessages.GetString("SeekableStreamRequired"));
			if (checked(stream.Position + stream.Length) < 0x20) throw new InvalidDataException(ErrorMessages.GetString("NotEnoughData"));

			// We use a lot of "long" and "int" variables here, but data is likely stored as ulong and uint…
			// So better test for overflow… Who knows what might happen in the future… ;)
			// The "safe" pattern is to read as unsigned and cast to signed where oferflow is possible.
			checked
			{
				this.stream = stream;
				archiveDataOffset = stream.Position; // We don't handle searching for MPQ data. The stream must already be positionned at the beginning of the MPQ data.
				signature = stream.ReadUInt32();
				// The first part of the file may be MPQ user data. The next part should be regular MPQ archive data.
				if (signature == MpqUserDataSignature)
				{
					userDataOffset = archiveDataOffset;
					userDataLength = stream.ReadUInt32();
					stream.Seek(stream.ReadUInt32() - 3 * sizeof(uint), SeekOrigin.Current);
					archiveDataOffset = stream.Position;
					signature = stream.ReadUInt32();
				}
				// Checking for MPQ archive signature
				if (signature !=  MpqArchiveSignature) throw new InvalidDataException(ErrorMessages.GetString("InvalidData"));
				headerSize = stream.ReadUInt32();
				archiveDataLength = stream.ReadUInt32();
				// MPQ format detection
				// Unknown MPQ version will raise an error… This seems like a safe idea.
				ushort mpqVersion = stream.ReadUInt16();
				switch (mpqVersion) // Read MPQ format
				{
					case 0: // Original MPQ format
						archiveFormat = MpqFormat.Original;
						if (headerSize < 0x20) throw new InvalidDataException(ErrorMessages.GetString("InvalidArchiveHeader"));
						break;
					case 1: // Extended MPQ format (WoW Burning Crusade)
						archiveFormat = MpqFormat.BurningCrusade;
						if (headerSize < 0x2C) throw new InvalidDataException(ErrorMessages.GetString("InvalidArchiveHeader"));
						break;
					case 2: // Enhanced MPQ format (Take 1)
						archiveFormat = MpqFormat.CataclysmFirst;
						// Header may not contain any additional field than BC extended MPQ format.
						// However, if additional fields are present, the header should be at least 68 bytes long.
						if (headerSize < 0x2C || (headerSize > 0x2C && headerSize < 0x44))
							throw new InvalidDataException(ErrorMessages.GetString("InvalidArchiveHeader"));
						break;
					case 3: // Enhanced MPQ format (Take 2)
						archiveFormat = MpqFormat.CataclysmSecond;
						if (headerSize < 0xD0) throw new InvalidDataException(ErrorMessages.GetString("InvalidArchiveHeader"));
						break;
					default:
						throw new MpqVersionNotSupportedException(mpqVersion); // Newer MPQ versions can probably be read just as well by the existing code… But can't know for sure…
				}
				blockSize = 0x200 << stream.ReadUInt16(); // Calculate block size
				hashTableOffset = stream.ReadUInt32(); // Get Hash Table Offset
				blockTableOffset = stream.ReadUInt32(); // Get Block Table Offset
				hashTableLength = stream.ReadUInt32(); // Get Hash Table Size
				blockTableLength = stream.ReadUInt32(); // Get Block Table Size

				// Assign the compressed size for the various tables.
				// Since compression was non-existant with V1 & V2, we know the compressed size is the uncompressed size.
				// If the compressed size is different as specified in V4, this will be overwritten later.
				hashTableCompressedSize = 4 * sizeof(uint) * hashTableLength;
				if (blockTableOffset > hashTableOffset && blockTableOffset - hashTableOffset < hashTableCompressedSize) // Compute compressed hash table length if needed
					hashTableCompressedSize = blockTableOffset - hashTableOffset;
				blockTableCompressedSize = 4 * sizeof(uint) * blockTableLength;

				// Process additional values for "Burning Crusade" MPQ format
				if (archiveFormat >= MpqFormat.BurningCrusade)
				{
					ushort hashTableOffsetHigh, blockTableOffsetHigh;

					// Read extended information
					highBlockTableOffset = (long)stream.ReadUInt64();
					highBlockTableCompressedSize = highBlockTableOffset != 0 ? sizeof(uint) * blockTableLength : 0;
					hashTableOffsetHigh = stream.ReadUInt16();
					blockTableOffsetHigh = stream.ReadUInt16();
					// Modify offsets accordingly
					hashTableOffset |= (long)hashTableOffsetHigh << 32;
					blockTableOffset |= (long)blockTableOffsetHigh << 32;

					// Handle MPQ version 3 (Cataclysm First) and newer
					if (archiveFormat >= MpqFormat.CataclysmFirst && headerSize >= 0x44)
					{
						archiveDataLength = (long)stream.ReadUInt64();
						enhancedBlockTableOffset = (long)stream.ReadUInt64();
						enhancedHashTableOffset = (long)stream.ReadUInt64();

						// Handle MPQ version 4 (Cataclysm Second)
						if (archiveFormat >= MpqFormat.CataclysmSecond)
						{
							hashTableCompressedSize = (long)stream.ReadUInt64();
							blockTableCompressedSize = (long)stream.ReadUInt64();
							highBlockTableCompressedSize = (long)stream.ReadUInt64();
							enhancedHashTableCompressedSize = (long)stream.ReadUInt64();
							enhancedBlockTableCompressedSize = (long)stream.ReadUInt64();

							rawChunkSize = stream.ReadUInt32();

							hashes = new byte[6 * 16];

							if (stream.Read(hashes, 0, hashes.Length) != hashes.Length)
								throw new EndOfStreamException();
						}
						else
						{
							// TODO: Compute the uncompresed size for the new enhanced tables of version 3… (Will have to check how to do that…)
							highBlockTableCompressedSize = highBlockTableOffset > 0 ? sizeof(ushort) * blockTableLength : 0;
						}
					}
					else
					{
#if DEBUG
						long oldArchiveSize = archiveDataLength;
#endif
						// Compute 64 bit archive size (We don't really need it for MPQ reading, but for integrity checks, we do…)
						if (highBlockTableOffset > hashTableOffset && highBlockTableOffset > blockTableOffset)
							archiveDataLength = highBlockTableOffset + sizeof(ushort) * blockTableLength;
						else if (blockTableOffset > hashTableOffset)
							archiveDataLength = blockTableOffset + 4 * sizeof(uint) * blockTableLength;
						else
							archiveDataLength = hashTableOffset + 4 * sizeof(uint) * hashTableLength;
#if DEBUG
						Debug.Assert(oldArchiveSize >= archiveDataLength);
#endif
					}
				}
				else
				{
					highBlockTableOffset = 0;
					highBlockTableCompressedSize = 0;
				}

				if (!CheckOffset((uint)headerSize)
					|| !CheckOffset(hashTableOffset) || !CheckOffset(blockTableOffset)
					|| hashTableLength < blockTableLength)
					throw new InvalidDataException(ErrorMessages.GetString("InvalidArchiveHeader"));

				hashTableSize = 4 * sizeof(uint) * hashTableLength;
				blockTableSize = 4 * sizeof(uint) * blockTableLength;
				highBlockTableSize = highBlockTableOffset != 0 ? sizeof(ushort) * blockTableLength : 0;

				// Check for strong signature presence
				if (stream.Length >= archiveDataOffset + archiveDataLength + 2052)
				{
					stream.Seek(archiveDataOffset + archiveDataLength, SeekOrigin.Begin);

					hasStrongSignature = stream.ReadUInt32() == MpqStrongSignatureSignature;
				}
			}

			// Create buffers for table reading
			var tableReadBuffer = hashTableSize < hashTableCompressedSize || blockTableSize < blockTableCompressedSize || highBlockTableCompressedSize < highBlockTableSize ?
				new byte[Math.Max(Math.Max(hashTableCompressedSize, blockTableCompressedSize), highBlockTableCompressedSize)] :
				null;

			var tableBuffer = new byte[Math.Max(hashTableSize, blockTableSize)];

			// Read Hash Table
			ReadHashTable(tableBuffer, hashTableLength, hashTableOffset, hashTableCompressedSize, tableReadBuffer);

			// Read Block Table
			ReadBlockTable(tableBuffer, blockTableLength, blockTableOffset, blockTableCompressedSize, highBlockTableOffset, highBlockTableCompressedSize, tableReadBuffer);

			// When possible, find and parse the listfile…
			listFile = FindFile(ListFileName);
			if (listFile != null)
			{
				if (shouldParseListFile)
					ParseListFile();
			}

			// When possible, find and parse the attributes.
			attribFile = FindFile(AttributesFileName);
			if (attribFile != null)
			{
				ParseAttributeFile();
			}
		}

		private bool CheckOffset(long offset) { return offset >= 0 && offset < archiveDataLength; }

		/// <summary>Reads the encrypted <see cref="System.UInt32"/> table at the specified offset in the archive.</summary>
		/// <remarks>
		/// This method will place the bytes in their native order.
		/// Reading must be done by pinning the buffer and accessing it as an <see cref="System.UInt32"/> buffer.
		/// The only purpose of this method is to share code between the hash table and block table reading methods.
		/// Because of its specific behavior, it should not be used anywhere else…
		/// </remarks>
		/// <param name="buffer">The destination buffer.</param>
		/// <param name="tableLength">Length of the table in units of 16 bytes.</param>
		/// <param name="tableOffset">The offset in the archive.</param>
		/// <param name="dataLength">Length of the data to read.</param>
		/// <param name="hash">The hash to use for decrypting the data.</param>
		/// <param name="compressedReadBuffer">The compressed read buffer to use for holding temporary data.</param>
		private unsafe void ReadEncryptedUInt32Table(byte[] buffer, long tableLength, long tableOffset, long dataLength, uint hash, byte[] compressedReadBuffer)
		{
			int uintCount = checked((int)(tableLength << 2));
			long realDataLength = checked(sizeof(uint) * uintCount);

			bool isCompressed = dataLength < realDataLength;
			// Stream.Read only takes an int length for now, and it is unlikely that the tables will ever exceed 2GB.
			// But anyway, if this ever happens in the future the overflow check should ensure us that the program will crash nicely.
			int dataLengthInt32 = checked((int)dataLength);

			stream.Seek(archiveDataOffset + tableOffset, SeekOrigin.Begin);
			if (stream.Read(isCompressed ? compressedReadBuffer : buffer, 0, dataLengthInt32) != dataLengthInt32)
				throw new EndOfStreamException(); // Throw an exception if we are not able to read as many bytes as we were told we could read…

			fixed (byte* bufferPointer = buffer)
			{
				uint* tablePointer = (uint*)bufferPointer;

				// If the data is compressed, we will have to swap endianness three times on big endian platforms, but it can't be helped.
				// (Endiannes swap is done by the Encryption.Decrypt method automatically when passing a byte buffer.)
				// However, if we don't have to decompress, endiannes swap is only done once, before decrypting.

				if (isCompressed)
				{
					if (hash != 0) CommonMethods.Decrypt(compressedReadBuffer, hash, dataLengthInt32); // On big endian platforms : Read UInt32, Swap Bytes, Compute, Swap Bytes, Store UInt32
					if (CommonMethods.DecompressBlock(compressedReadBuffer, dataLengthInt32, buffer, true) != realDataLength)
						throw new InvalidDataException(); // Only allow the exact amount of bytes as a result
				}

				if (!BitConverter.IsLittleEndian) CommonMethods.SwapBytes(tablePointer, uintCount);

				if (!isCompressed && hash != 0) CommonMethods.Decrypt(tablePointer, hash, uintCount);
			}
		}

		private void ReadHighBlockTable(byte[] buffer, long tableLength, long tableOffset, long dataLength, byte[] compressedReadBuffer)
		{
			int ushortCount = checked((int)tableLength);
			long realDataLength = checked(sizeof(ushort) * ushortCount);

			bool isCompressed = dataLength < realDataLength;
			// Stream.Read only takes an int length for now, and it is unlikely that the tables will ever exceed 2GB.
			// But anyway, if this ever happens in the future the overflow check should ensure us that the program will crash nicely.
			int dataLengthInt32 = checked((int)dataLength);

			stream.Seek(archiveDataOffset + tableOffset, SeekOrigin.Begin);
			if (stream.Read(isCompressed ? compressedReadBuffer : buffer, 0, dataLengthInt32) != dataLengthInt32)
				throw new EndOfStreamException(); // Throw an exception if we are not able to read as many bytes as we were told we could read…

			if (isCompressed)
				if (CommonMethods.DecompressBlock(compressedReadBuffer, dataLengthInt32, buffer, true) != realDataLength)
					throw new InvalidDataException(); // Only allow the exact amount of bytes as a result

			if (!BitConverter.IsLittleEndian) CommonMethods.SwapBytes16(buffer);
		}

		private unsafe void ReadHashTable(byte[] buffer, long tableLength, long tableOffset, long dataLength, byte[] compressedReadBuffer)
		{
			ReadEncryptedUInt32Table(buffer, tableLength, tableOffset, dataLength, HashTableHash, compressedReadBuffer);

			fixed (byte* bufferPointer = buffer)
				hashTable = MpqHashTable.FromMemory((uint*)bufferPointer, tableLength);

#if ENFORCE_SAFETY
			if (!hashTable.CheckIntegrity(blockTableSize)) // Check HashTable Integrity (Could be too restrictive, correct if needed)
				throw new ArchiveCorruptException();
#endif
		}

		private unsafe void ReadBlockTable(byte[] buffer, long tableLength, long tableOffset, long dataLength, long highTableOffset, long highTableDataLength, byte[] compressedReadBuffer)
		{
			byte[] highBuffer = null;

			ReadEncryptedUInt32Table(buffer, tableLength, tableOffset, dataLength, BlockTableHash, compressedReadBuffer);

			if (highTableOffset != 0 && highTableDataLength != 0)
				ReadHighBlockTable(highBuffer = new byte[sizeof(ushort) * tableLength], tableLength, highTableOffset, highTableDataLength, compressedReadBuffer);

			uint fileCount;

			fixed (byte* bufferPointer = buffer)
			fixed (byte* highBufferPointer = highBuffer)
				blockTable = MpqBlockTable.FromMemory((uint*)bufferPointer, (ushort*)highBufferPointer, tableLength, out fileCount);

			CreateFiles(fileCount);
		}

		private void CreateFiles(uint fileCount)
		{
			files = new MpqFile[fileCount];

			for (uint i = 0, blockIndex = 0; i < fileCount; i++)
			{
				while ((blockTable.Entries[blockIndex].Flags & MpqFileFlags.Exists) == 0) blockIndex++;

				files[i] = new MpqFile(this, blockIndex++);
			}
		}

		/// <summary>Gets a value that indicate whether the current archive has a listfile.</summary>
		/// <remarks>
		/// Having a listfile is not required for an archive to be readable.
		/// However, you need to know the filenames if you want to read the files.
		/// </remarks>
		/// <value><see langword="true"/> if the current archive has a listfile; otherwise, <see langword="false"/>.</value>
		public bool HasListFile { get { return listFile != null; } }

		/// <summary>Parses the listfile if it has not already been done.</summary>
		/// <remarks>
		/// Once the list file has been parsed, calls this function will just do nothing.
		/// The list file will always be parsed by default, but you can override this behavior using an appropriate constructor.
		/// Please note that parsing the list file can take some time, and is not required if you already know the filenames.
		/// Also, this operation is irreversible. Once the filenames are present in memory, the only way to free the memory is to close the archive.
		/// </remarks>
		public void ParseListFile()
		{
			if (listFileParsed) return;

			using (var listFileReader = new StreamReader(listFile.Open()))
			{
				string line;

				while ((line = listFileReader.ReadLine()) != null)
					TryFilename(line, true);
				listFileParsed = true;
			}
		}

		/// <summary>Parses the (attributes) if it has not already been done.</summary>
		public void ParseAttributeFile()
		{
			const int MPQ_ATTRIBUTE_CRC32 = 0x1;
			const int MPQ_ATTRIBUTE_FILETIME = 0x2;
			const int MPQ_ATTRIBUTE_MD5 = 0x4;
			const int MPQ_ATTRIBUTE_PATCH_BIT = 0x8;

			if (attribFileParsed) return;

			int numEntries = blockTable.Entries.Length;

			using (var reader = new BinaryReader(attribFile.Open())) 
			{
				var version = reader.ReadInt32();
				if (version != 100)
					return;

				var flags = reader.ReadInt32();

				if ((flags & MPQ_ATTRIBUTE_CRC32) != 0) 
				{
					for (int i = 0; i < numEntries; ++i) 
					{
						blockTable.Entries[i].Crc32 = reader.ReadInt32();
					}
				}
				
				if ((flags & MPQ_ATTRIBUTE_FILETIME) != 0) 
				{
					for (int i = 0; i < numEntries; ++i) 
					{
						blockTable.Entries[i].FileTime = reader.ReadInt64();
					}
				}

				if ((flags & MPQ_ATTRIBUTE_MD5) != 0)
				{
					for (int i = 0; i < numEntries; ++i) 
					{
						byte[] bytes = reader.ReadBytes(16);
						blockTable.Entries[i].Md5 = new MpqMd5(bytes);
					}
				}

				if ((flags & MPQ_ATTRIBUTE_PATCH_BIT) != 0)
				{
					// ignore for now.
				}

				attribFileParsed = true;
			}
		}

		/// <summary>Associate the specified filename with files in the archive.</summary>
		/// <remarks>
		/// The filename will only be associated to matching files. If no file corresponds to the specified filename, nothing will happen.
		/// This function may be useful when you don't have a listfile for a given MPQ archive or when you just want to find some hidden files.
		/// </remarks>
		/// <param name="filename">The filename to associate.</param>
		/// <param name="listed">If set to <c>true</c>, the name was found in the listfile.</param>
		private void TryFilename(string filename, bool listed)
		{
			int[] blocks = hashTable.FindMulti(filename);

#if DEBUG
			if (listed) Debug.Assert(blocks.Length > 0);
#endif

			for (int i = 0; i < blocks.Length; i++)
				blockTable.Entries[blocks[i]].OnNameDetected(filename, true, listed);
#if DEBUG
			if (blocks.Length == 0) Debug.WriteLine("File \"" + filename + "\" not found in archive.");
#endif
		}

		/// <summary>Associate the specified filename with files in the archive.</summary>
		/// <remarks>
		/// The filename will only be associated to matching files. If no file corresponds to the specified filename, nothing will happen.
		/// This function may be useful when you don't have a listfile for a given MPQ archive or when you just want to find some hidden files.
		/// </remarks>
		/// <param name="filename">The filename you want to try</param>
		public void TryFilename(string filename) { TryFilename(filename, false); }

		/// <summary>Opens a file with the specified filename.</summary>
		/// <remarks>
		/// This function will only open the first result found.
		/// Modern MPQ archives should never contain more than one entry with the same filename.
		/// This method is perfectly safe for modern games such as WoW, SII or D3.
		/// </remarks>
		/// <param name="filename">The filename of the file to open.</param>
		/// <returns>An <see cref="MpqFileStream"/> instance.</returns>
		/// <exception cref="FileNotFoundException">No file with the specified name could be found in the archive.</exception>
		public MpqFileStream OpenFile(string filename)
		{
			var file = FindFile(filename);

			if (file == null || file.IsDeleted) throw new FileNotFoundException();

			return file.Open();
		}

		/// <summary>Opens a file with the specified filename and LCID.</summary>
		/// <param name="filename">The filename of the file to open.</param>
		/// <param name="lcid">The LCID of file to open.</param>
		/// <returns>An <see cref="MpqFileStream"/> instance.</returns>
		/// <exception cref="FileNotFoundException">No file with the specified name could be found in the archive.</exception>
		public MpqFileStream OpenFile(string filename, int lcid)
		{
			var file = FindFile(filename);

			if (file == null || file.IsDeleted) throw new FileNotFoundException();

			return file.Open();
		}

		/// <summary>Finds files with the specified filename.</summary>
		/// <remarks>
		/// This function will return all <see cref="MpqFile"/>s matching the given filename.
		/// There might be more than one <see cref="MpqFile"/> returned because of the localization.
		/// </remarks>
		/// <param name="filename">The filename of the files to find.</param>
		/// <returns>Returns an array of <see cref="MpqFile"/>, containing zero or more <see cref="MpqFile"/>.</returns>
		[Obsolete]
		public MpqFile[] FindFiles(string filename)
		{
			int[] blocks = hashTable.FindMulti(filename);
			MpqFile[] files = new MpqFile[blocks.Length];

			for (int i = 0; i < blocks.Length; i++)
			{
				blockTable.Entries[blocks[i]].OnNameDetected(filename);
				files[i] = files[blockTable.Entries[blocks[i]].FileIndex];
			}
			return files;
		}

		/// <summary>Finds one file the specified filename and LCID.</summary>
		/// <param name="filename">The filename of the file to find.</param>
		/// <param name="lcid">The LCID of file to find.</param>
		/// <returns>Returns an <see cref="MpqFile"/> object if file is found, or <c>null</c> otherwise.</returns>
		[Obsolete]
		public MpqFile FindFile(string filename, int lcid)
		{
			int block = hashTable.Find(filename, lcid);

			if (block >= 0)
			{
				blockTable.Entries[block].OnNameDetected(filename);

				return files[blockTable.Entries[block].FileIndex];
			}
			else return null;
		}

		/// <summary>Finds a file with the specified filename.</summary>
		/// <remarks>This function will only return the first result found.</remarks>
		/// <param name="filename">The filename of the file to find.</param>
		/// <returns>Returns an <see cref="MpqFile"/> object if file is found, or <c>null</c> otherwise.</returns>
		public MpqFile FindFile(string filename)
		{
			int block = hashTable.Find(filename);

			if (block >= 0)
			{
				blockTable.Entries[block].OnNameDetected(filename);

				return files[blockTable.Entries[block].FileIndex];
			}
			else return null;
		}

		/// <summary>Sets the preferred culture to use when searching files in the archive.</summary>
		/// <remarks>It might happen that a given file exists for different culture in the same MPQ archive, but it is more likely that your MPQ archive is already localized itself…</remarks>
		/// <param name="lcid">The LCID for the desired culture.</param>
		[Obsolete]
		public void SetPreferredCulture(int lcid) { hashTable.SetPreferredCulture(lcid); }

		/// <summary>Resolves the data corresponding to the base file of a given patch <see cref="MpqFile"/>.</summary>
		/// <param name="file">The patch file.</param>
		/// <returns>A <see cref="Stream"/> containing the data for the base file if it was found; otherwise <c>null</c>.</returns>
		internal Stream ResolveBaseFileInternal(MpqFile file)
		{
			if (ResolveBaseFile != null)
				lock (resolveStreamEventArgs)
				{
					ResolveBaseFile(file, resolveStreamEventArgs);
					return resolveStreamEventArgs.TransferStreamOwnership();
				}
			else return null;
		}

		protected internal object SyncRoot { get { return syncRoot; } }

		protected internal int ReadUserData(byte[] buffer, int index, long offset, int count) { return Read(buffer, index, userDataOffset + offset, count); }

		protected internal int ReadArchiveData(byte[] buffer, int index, long offset, int count) { return Read(buffer, index, archiveDataOffset + offset, count); }

		/// <summary>Reads a sequence of bytes from the inner stream and advances the position within the stream by the number of bytes read.</summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="index"/> and (<paramref name="index"/> + <paramref name="length"/> - 1) replaced by the bytes read from the current source.</param>
		/// <param name="index">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
		/// <param name="absoluteOffset">The position in the stream where reading should start.</param>
		/// <param name="length">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
		protected internal int Read(byte[] buffer, int index, long absoluteOffset, int length)
		{
			lock (syncRoot) // Allow multithreaded read access
			{
				stream.Seek(absoluteOffset, SeekOrigin.Begin);
				return stream.Read(buffer, index, length);
			}
		}

		/// <summary>Gets the size of blocks in the archive.</summary>
		public int BlockSize { get { return blockSize; } }

		/// <summary>Gets a collection containing reference to all the files in the archive.</summary>
		public MpqFileCollection Files { get { return fileCollection; } }

		/// <summary>Gets the size of the MPQ archive.</summary>
		protected long UserDataOffset { get { return userDataOffset; } }

		/// <summary>Gets the size of the MPQ archive.</summary>
		public long UserDataLength { get { return userDataLength; } }

		/// <summary>Gets the size of the MPQ archive.</summary>
		protected long ArchiveDataOffset { get { return archiveDataOffset; } }

		/// <summary>Gets the size of the MPQ archive.</summary>
		public long ArchiveDataLength { get { return archiveDataLength; } }

		/// <summary>Gets a flag indicating the format of the archive.</summary>
		public MpqFormat Format { get { return archiveFormat; } }
	}
}
