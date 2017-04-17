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
using System.Collections.Generic;
using System.Text;

namespace CrystalMpq
{
	internal static class SparseCompression
	{
		public static int CompressBlock(byte[] inBuffer, byte[] outBuffer)
		{
			throw new NotImplementedException();
		}

		public static unsafe int DecompressBlock(byte[] inBuffer, int offset, int count, byte[] outBuffer)
		{
			if (inBuffer == null) throw new ArgumentNullException("inBuffer");
			if (outBuffer == null) throw new ArgumentNullException("outBuffer");

			if (offset < 0 || offset > inBuffer.Length) throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || checked(offset + count) > inBuffer.Length) throw new ArgumentOutOfRangeException("count");

			fixed (byte* inBufferPointer = inBuffer, outBufferPointer = outBuffer)
				return DecompressBlock(inBufferPointer + offset, count, outBufferPointer, outBuffer.Length);
		}

		public static unsafe int DecompressBlock(byte* inBuffer, int inLength, byte* outBuffer, int outLength)
		{
			if (inLength < 5)
			{
				int chunkLength = Math.Min(inLength, outLength);

				for (int i = chunkLength; i != 0; i--) *outBuffer++ = *inBuffer++;

				return chunkLength;
			}

			outLength = Math.Min((((((*inBuffer++ << 8) | *inBuffer++) << 8) | *inBuffer++) << 8) | *inBuffer++, outLength);

			if (outLength == 0) return 0;

			int length = outLength;

			do
			{
				byte chunkInfo = *inBuffer++;
				bool chunkIsData = (chunkInfo & 0x80) != 0;
				int chunkLength = Math.Min((chunkInfo & 0x7F) + (chunkIsData ? 1 : 3), length);

				length -= chunkLength;

				if (!chunkIsData) while (chunkLength-- != 0) *outBuffer++ = 0;
				else while (chunkLength != 0) *outBuffer++ = *inBuffer++;
			}
			while (length != 0);

			return outLength;
		}
	}
}
