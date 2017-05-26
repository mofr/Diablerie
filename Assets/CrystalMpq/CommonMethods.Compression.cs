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
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.BZip2;
using LZMA = SevenZip.Compression.LZMA;
using Foole.Mpq;

namespace CrystalMpq
{
	partial class CommonMethods
	{
		[ThreadStatic]
		private static Inflater inflater;
		private static Inflater Inflater { get { return inflater = inflater ?? new Inflater(); } }
		[ThreadStatic]
		private static LZMA.Decoder lzmaDecoder;
		private static LZMA.Decoder LzmaDecoder { get { return lzmaDecoder = lzmaDecoder ?? new LZMA.Decoder(); } }

		public static int CompressBlock(byte[] inBuffer, byte[] outBuffer, bool multi)
		{
			return 0;
		}

		public static int DecompressBlock(byte[] inBuffer, int inLength, byte[] outBuffer, bool multi)
		{
			if (!multi) return DclCompression.DecompressBlock(inBuffer, 0, inLength, outBuffer);
			else // Examinate first byte for finding compression methods used
			{
				switch (inBuffer[0])
				{
					case 0x01: // Huffman
						throw new MpqCompressionNotSupportedException(0x01, "Huffman");
					case 0x02: // Zlib (Deflate/Inflate)
						Inflater.Reset(); // The first property read will initialize the field…
						inflater.SetInput(inBuffer, 1, inLength - 1);
						return inflater.Inflate(outBuffer);
					case 0x08: // PKWare DCL (Implode/Explode)
						return DclCompression.DecompressBlock(inBuffer, 1, inLength - 1, outBuffer);
					case 0x10: // BZip2
						using (var inStream = new MemoryStream(inBuffer, 1, inLength - 1, false, false))
						using (var outStream = new BZip2InputStream(inStream))
							return outStream.Read(outBuffer, 0, outBuffer.Length);
					case 0x12: // LZMA
						using (var inStream = new MemoryStream(inBuffer, 1, inLength - 1, false, false))
						using (var outStream = new MemoryStream(outBuffer, true))
						{
							lzmaDecoder.Code(inStream, outStream, inStream.Length, outStream.Length, null);
							return checked((int)outStream.Position);
						}
					case 0x20: // Sparse
						return SparseCompression.DecompressBlock(inBuffer, 1, inLength - 1, outBuffer);
					case 0x22: // Sparse + Deflate
#if USE_SHARPZIPLIB // Use SharpZipLib's Deflate implementation
						Inflater.Reset(); // The first property read will initialize the field…
						inflater.SetInput(inBuffer, 1, inLength - 1);
						tempBuffer = CommonMethods.GetSharedBuffer(outBuffer.Length);
						return SparseCompression.DecompressBlock(tempBuffer, 0, inflater.Inflate(tempBuffer), outBuffer);
#else // Use .NET 2.0's built-in inflate algorithm
						using (var inStream = new MemoryStream(inBuffer, 3, inLength - 7, false, false))
						using (var inoutStream = new DeflateStream(inStream, CompressionMode.Decompress))
						using (var outStream = new SparseInputStream(inoutStream))
							return outStream.Read(outBuffer, 0, outBuffer.Length);
#endif
					case 0x30: // Sparse + BZip2
						using (var inStream = new MemoryStream(inBuffer, 1, inLength - 1, false, false))
						using (var inoutStream = new BZip2InputStream(inStream))
						using (var outStream = new SparseInputStream(inoutStream))
							return outStream.Read(outBuffer, 0, outBuffer.Length);
					case 0x40: // Mono IMA ADPCM
                        using (var inStream = new MemoryStream(inBuffer, 1, inLength - 1, false, false))
                        {
                            var decompressedStream = MpqHuffman.Decompress(inStream);
                            byte[] decompressed = MpqWavCompression.Decompress(decompressedStream, 1);
                            Buffer.BlockCopy(decompressed, 0, outBuffer, 0, decompressed.Length);
                            return decompressed.Length;
                        }
                    case 0x41: // Mono IMA ADPCM + Huffman
                        using (var inStream = new MemoryStream(inBuffer, 1, inLength - 1, false, false))
                        {
                            var decompressedStream = MpqHuffman.Decompress(inStream);
                            byte[] decompressed = MpqWavCompression.Decompress(decompressedStream, 1);
                            Buffer.BlockCopy(decompressed, 0, outBuffer, 0, decompressed.Length);
                            return decompressed.Length;
                        }
					case 0x48: // Mono IMA ADPCM + Implode
						throw new MpqCompressionNotSupportedException(0x48, "Mono IMA ADPCM + Implode");
					case 0x80: // Stereo IMA ADPCM
                        using (var inStream = new MemoryStream(inBuffer, 1, inLength - 1, false, false))
                        {
                            byte[] decompressed = MpqWavCompression.Decompress(inStream, 2);
                            Buffer.BlockCopy(decompressed, 0, outBuffer, 0, decompressed.Length);
                            return decompressed.Length;
                        }
					case 0x81: // Stereo IMA ADPCM + Huffman
                        using (var inStream = new MemoryStream(inBuffer, 1, inLength - 1, false, false))
                        {
                            var decompressedStream = MpqHuffman.Decompress(inStream);
                            byte[] decompressed = MpqWavCompression.Decompress(decompressedStream, 2);
                            Buffer.BlockCopy(decompressed, 0, outBuffer, 0, decompressed.Length);
                            return decompressed.Length;
                        }
                    case 0x88: // Stereo IMA ADPCM + Implode
						throw new MpqCompressionNotSupportedException(0x88, "Stereo IMA ADPCM + Implode");
					default:
						throw new MpqCompressionNotSupportedException(inBuffer[0]);
				}
			}
		}
	}
}
