//
// BitStream.cs
//
// Authors:
//		Foole (fooleau@gmail.com)
//
// (C) 2006 Foole (fooleau@gmail.com)
// Based on code from StormLib by Ladislav Zezula
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.IO;

namespace Foole.Mpq
{
	/// <summary>
	/// A utility class for reading groups of bits from a stream
	/// </summary>
	internal class BitStream
	{
        private Stream _baseStream;
        private int _current;
		private int _bitCount;

		public BitStream(Stream sourceStream)
		{
            _baseStream = sourceStream;
		}
		
		public int ReadBits(int bitCount)
		{
			if (bitCount > 16)
				throw new ArgumentOutOfRangeException("BitCount", "Maximum BitCount is 16");
			if (EnsureBits(bitCount) == false) return -1;
            int result = _current & (0xffff >> (16 - bitCount));
			WasteBits(bitCount);
			return result;
		}

		public int PeekByte()
		{
			if (EnsureBits(8) == false) return -1;
            return _current & 0xff;
		}

		public bool EnsureBits(int bitCount)
		{
			if (bitCount <= _bitCount) return true;

            if (_baseStream.Position >= _baseStream.Length) return false;
            int nextvalue = _baseStream.ReadByte();
            _current |= nextvalue << _bitCount;
			_bitCount += 8;
			return true;
		}
		
		private bool WasteBits(int bitCount)
		{
            _current >>= bitCount;
			_bitCount -= bitCount;
			return true;
		}
	}
}
