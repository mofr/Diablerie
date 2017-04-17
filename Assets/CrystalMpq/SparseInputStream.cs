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
using System.IO;

namespace CrystalMpq
{
	internal sealed class SparseInputStream : Stream
	{
		[ThreadStatic]
		private static byte[] initialBuffer;
		private static byte[] InitialBuffer { get { return initialBuffer = initialBuffer ?? new byte[5]; } }

		private Stream baseStream;
		private long position;
		private long length;
		private int initialWord;
		private bool chunkIsData;
		private byte chunkLength;

		public SparseInputStream(Stream baseStream)
		{
			if (!baseStream.CanRead) throw new ArgumentException();

			this.baseStream = baseStream;

			int byteCount = baseStream.Read(InitialBuffer, 0, 5);

			for (int i = Math.Min(byteCount, 4); i != 0; i--) initialWord = initialBuffer[4 - i] | (initialWord << 8);

			if (byteCount < 5) length = byteCount;
			else
			{
				length = initialWord;
				chunkIsData = (initialBuffer[5] & 0x80) != 0;
				chunkLength = (byte)((initialBuffer[5] & 0x7F) + (chunkIsData ? 1 : 3));
			}
		}

		public override bool CanRead { get { return true; } }

		public override bool CanSeek { get { return false; } }

		public override bool CanWrite { get { return false; } }

		public override void Flush() { }

		public override long Length
		{
			get { return length; }
		}

		public override long Position
		{
			get { return 0; }
			set { throw new NotSupportedException(); }
		}

		public unsafe override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length) throw new ArgumentOutOfRangeException("offset");
			if (count < 0 || checked(offset + count) > buffer.Length) throw new ArgumentOutOfRangeException("count");

			fixed (byte* bufferPointer = buffer)
			{
				return Read(bufferPointer + offset, count);
			}
		}

		public unsafe int Read(byte* buffer, int count)
		{
			int n = count;

			while (n != 0 && position < length)
			{
				if (length < 5)
				{
					*buffer++ = (byte)((initialWord >> ((3 - (int)position++) << 3)) & 0xFF);
					n--;
					continue;
				}

				if (chunkLength == 0)
				{
					int chunkInfo = baseStream.ReadByte();

					if (chunkInfo == -1)
					{
						length = position;
						throw new EndOfStreamException();
					}
					chunkIsData = (chunkInfo & 0x80) != 0;
					chunkLength = (byte)((chunkInfo & 0x7F) + (chunkIsData ? 1 : 3));
				}

				int m = Math.Min(chunkLength, (int)Math.Min(length - position, (long)n));

				position += m;
				n -= m;

				if (!chunkIsData) while (m-- != 0) *buffer++ = 0;
				else
					while (m-- != 0)
					{
						int data = baseStream.ReadByte();

						if (data == -1)
						{
							length = position;
							throw new EndOfStreamException();
						}
						*buffer++ = (byte)data;
					}
			}

			return count - n;
		}

		public override int ReadByte()
		{
			if (position >= length) return -1;

			if (length < 5) return (initialWord >> ((3 - (int)position++) << 3)) & 0xFF;

			if (chunkLength == 0)
			{
				int chunkInfo = baseStream.ReadByte();

				if (chunkInfo == -1)
				{
					length = position;
					throw new EndOfStreamException();
				}
				chunkIsData = (chunkInfo & 0x80) != 0;
				chunkLength = (byte)((chunkInfo & 0x7F) + (chunkIsData ? 1 : 3));
			}

			position++;
			chunkLength--;

			if (!chunkIsData) return 0;
			else
			{
				int data = baseStream.ReadByte();

				if (data == -1)
				{
					length = position;
					throw new EndOfStreamException();
				}
				else return (byte)data;
			}
		}

		public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }

		public override void SetLength(long value) { throw new NotSupportedException(); }

		public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
	}
}
