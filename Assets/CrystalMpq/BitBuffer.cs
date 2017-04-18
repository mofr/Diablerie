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
using System.Runtime.InteropServices;

namespace CrystalMpq
{
	/// <summary>Represents a bit buffer.</summary>
	/// <remarks>
	/// This structure is designed for internal use in CrystalMpq.
	/// Incorrect usage of the structure will lead to bugs or even worse, memory leaks.
	/// The structure shall always be initialized using this constructor.
	/// Once created, there shall never be more than one living copy of the structure.
	/// The structure shall always be passed as a reference parameter and never as a value parameter.
	/// Once the structure have been used for its purposes, it shall be released using the <see cref="Dispose"/> method.
	/// After the structure has been disposed, the instance shall never be used again.
	/// </remarks>
	internal unsafe struct BitBuffer : IDisposable
	{
		private readonly GCHandle bufferHandle;
		private readonly byte* bufferPointer;
		private int pos, count, length;
		private byte b;

		/// <summary>Initializes a new instance of the <see cref="BitBuffer"/> struct.</summary>
		/// <remarks>The structure shall always be initialized using this constructor.</remarks>
		/// <param name="buffer">Array of bit containing the data</param>
		/// <param name="index">Position of data in the array</param>
		/// <param name="count">Size of data in the array</param>
		internal BitBuffer(byte[] buffer, int index, int count)
		{
			// Since we'll be dealing with unsafe code, we should do the bounds checking at least once to ensure nothing bad will happen.
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (index < 0 || index > buffer.Length) throw new ArgumentOutOfRangeException("index");
			if (count < 0 || checked(index + count) > buffer.Length) throw new ArgumentOutOfRangeException("count");

			this.bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			this.bufferPointer = (byte*)this.bufferHandle.AddrOfPinnedObject();
			this.count = 8;
			this.pos = index;
			this.length = index + count;
			this.b = buffer[this.pos++];
		}

		public void Dispose() { this.bufferHandle.Free(); }

		public int GetBit()
		{
			int r;

			if (count-- == 0)
			{
				if (pos < length) b = bufferPointer[pos++];
				else return 0;
				count = 7;
			}

			r = b & 0x1;
			b >>= 1;

			return r;
		}

		// This should return a sequence of 'count' bits when possible
		public int GetBits(int count)
		{
			int r = 0, n = 0, d;

			while (count > 0)
			{
				d = this.count - count;
				if (d >= 0)
					do
					{
						r = r | ((b & 0x1) << n++);
						b >>= 1;
						count--;
						this.count--;
					} while (count > 0);
				else
					while (this.count > 0)
					{
						r = r | ((b & 0x1) << n++);
						b >>= 1;
						count--;
						this.count--;
					}
				if (this.count == 0)
				{
					if (pos < length)
						b = bufferPointer[pos++];
					else
						return r;
					this.count = 8;
				}
			}

			return r;
		}

		public byte GetByte() { return (byte)GetBits(8); }

		public bool Eof { get { return (pos == length - 1 && count == 0) || (pos >= length); } }
	}
}
