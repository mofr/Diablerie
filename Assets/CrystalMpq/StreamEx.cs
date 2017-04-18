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

namespace CrystalMpq
{
	internal static class StreamEx
	{
		[ThreadStatic]
		private static byte[] buffer;

		private static byte[] Buffer { get { return buffer ?? (buffer = new byte[16]); } }

		public static short ReadInt16(this Stream @this)
		{
			var buffer = Buffer;

			if (@this.Read(buffer, 0, sizeof(short)) != sizeof(short)) throw new EndOfStreamException();

			return (short)(buffer[0] | buffer[1] << 8);
		}

		public static ushort ReadUInt16(this Stream @this)
		{
			var buffer = Buffer;

			if (@this.Read(buffer, 0, sizeof(ushort)) != sizeof(ushort)) throw new EndOfStreamException();

			return (ushort)(buffer[0] | buffer[1] << 8);
		}

		public static int ReadInt32(this Stream @this)
		{
			var buffer = Buffer;

			if (@this.Read(buffer, 0, sizeof(int)) != sizeof(int)) throw new EndOfStreamException();

			return buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24;
		}

		public static uint ReadUInt32(this Stream @this)
		{
			var buffer = Buffer;

			if (@this.Read(buffer, 0, sizeof(uint)) != sizeof(uint)) throw new EndOfStreamException();

			return (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
		}

		public static long ReadInt64(this Stream @this)
		{
			var buffer = Buffer;

			if (@this.Read(buffer, 0, sizeof(long)) != sizeof(long)) throw new EndOfStreamException();

			return (long)(uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24) | (long)buffer[4] << 32 | (long)buffer[5] << 40 | (long)buffer[6] << 48 | (long)buffer[7] << 56;
		}

		public static ulong ReadUInt64(this Stream @this)
		{
			var buffer = Buffer;

			if (@this.Read(buffer, 0, sizeof(ulong)) != sizeof(ulong)) throw new EndOfStreamException();

			return (ulong)(uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24) | (ulong)buffer[4] << 32 | (ulong)buffer[5] << 40 | (ulong)buffer[6] << 48 | (ulong)buffer[7] << 56;
		}
	}
}
