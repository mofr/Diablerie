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

namespace CrystalMpq
{
	partial class CommonMethods
	{
		private static uint[] encryptionTable = BuildEncryptionTable();

		private static uint[] BuildEncryptionTable()
		{
			int q, r = 0x100001;
			uint seed;

			uint[] encryptionTable = new uint[0x500];

			for (int i = 0; i < 0x100; i++)
				for (int j = 0; j < 5; j++)
				{
					unchecked
					{
						q = Math.DivRem(r * 125 + 3, 0x2AAAAB, out r);
						seed = (uint)(r & 0xFFFF) << 16;
						q = Math.DivRem(r * 125 + 3, 0x2AAAAB, out r);
						seed |= (uint)(r & 0xFFFF);
						encryptionTable[0x100 * j + i] = seed;
					}
				}

			return encryptionTable;
		}

		public static uint Hash(string text, uint hashOffset)
		{
			uint hash = 0x7FED7FED, seed = 0xEEEEEEEE;
			byte[] buffer = new byte[text.Length];
			char c;
			byte b;

			for (int i = 0; i < text.Length; i++)
				unchecked
				{
					c = text[i]; // The 128 first Unicode characters are the 128 ASCII characters, so it's fine like this
					if (c >= 128)
						c = '?'; // Replace invalid ascii characters with this...
					b = (byte)c;
					if (b > 0x60 && b < 0x7B)
						b -= 0x20;
					hash = encryptionTable[hashOffset + b] ^ (hash + seed);
					seed += hash + (seed << 5) + b + 3;
				}
			return hash;
		}

		// Old version
		//public static uint Hash(string text, uint hashOffset)
		//{
		//    uint hash = 0x7FED7FED, seed = 0xEEEEEEEE;
		//    byte[] buffer = new byte[text.Length];
		//    byte b;

		//    System.Text.Encoding.ASCII.GetBytes(text, 0, text.Length, buffer, 0);
		//    foreach (byte c in buffer)
		//        unchecked
		//        {
		//            b = c;
		//            if (b > 0x60 && b < 0x7B)
		//                b -= 0x20;
		//            hash = precalc[hashOffset + b] ^ (hash + seed);
		//            seed += hash + (seed << 5) + b + 3;
		//        }
		//    return hash;
		//}

		public static unsafe void Encrypt(uint[] data, uint hash)
		{
			fixed (uint* dataPointer = data)
				Encrypt(dataPointer, hash, data.Length);
		}

		public static unsafe void Encrypt(uint* data, uint hash, int length)
		{
			uint buffer, seed = 0xEEEEEEEE;

			for (int i = length; i-- != 0; )
				unchecked
				{
					seed += encryptionTable[0x400 + hash & 0xFF];
					buffer = *data;
					seed += buffer + (seed << 5) + 3;
					*data++ = buffer ^ (seed + hash);
					hash = (hash >> 11) | (0x11111111 + ((hash ^ 0x7FF) << 21));
				}
		}

		public static unsafe void Decrypt(uint[] data, uint hash)
		{
			fixed (uint* dataPointer = data)
				Decrypt(dataPointer, hash, data.Length);
		}

		public static unsafe void Decrypt(uint[] data, uint hash, int length)
		{
			fixed (uint* dataPointer = data)
				Decrypt(dataPointer, hash, length);
		}

		public static unsafe void Decrypt(byte[] data, uint hash) { Decrypt(data, hash, data.Length); }

		public static unsafe void Decrypt(byte[] data, uint hash, int length)
		{
			fixed (byte* dataPointer = data)
				if (BitConverter.IsLittleEndian) Decrypt((uint*)dataPointer, hash, length >> 2);
				else DecryptWithEndianSwap((uint*)dataPointer, hash, length >> 2);
		}

		public static unsafe void Decrypt(uint* data, uint hash, int length)
		{
			uint buffer, temp = 0xEEEEEEEE;

			for (int i = length; i-- != 0; )
				unchecked
				{
					temp += encryptionTable[0x400 + (hash & 0xFF)];
					buffer = *data ^ (temp + hash);
					temp += buffer + (temp << 5) + 3;
					*data++ = buffer;
					hash = (hash >> 11) | (0x11111111 + ((hash ^ 0x7FF) << 21));
				}
		}

		public static unsafe void DecryptWithEndianSwap(uint* data, uint hash, int length)
		{
			uint buffer, temp = 0xEEEEEEEE;

			for (int i = length; i-- != 0; )
				unchecked
				{
					temp += encryptionTable[0x400 + (hash & 0xFF)];
					buffer = CommonMethods.SwapBytes(*data) ^ (temp + hash);
					temp += buffer + (temp << 5) + 3;
					*data++ = CommonMethods.SwapBytes(buffer);
					hash = (hash >> 11) | (0x11111111 + ((hash ^ 0x7FF) << 21));
				}
		}
	}
}
