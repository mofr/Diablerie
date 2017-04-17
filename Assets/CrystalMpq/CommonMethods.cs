using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace CrystalMpq
{
	internal static partial class CommonMethods
	{
		[ThreadStatic]
		private static byte[] sharedBuffer;

		/// <summary>Gets a buffer of at least <paramref name="minLength"/> bytes.</summary>
		/// <remarks>
		/// While actively using the buffer, you must <c>make sure</c> to not call any other method using the same shared buffer.
		/// Also, no references to the buffer should be leaked after the method requesting the buffer has returned.
		/// <c>Not following these rules carefully will likely lead to a crash.</c>
		/// </remarks>
		/// <param name="minLength">Minimum required length.</param>
		/// <returns>A buffer of at least <paramref name="minLength"/> bytes.</returns>
		public static byte[] GetSharedBuffer(int minLength) { return sharedBuffer = (sharedBuffer == null || sharedBuffer.Length < minLength) ? new byte[minLength] : sharedBuffer; }

		[ThreadStatic]
		private static MD5 sharedMD5;

		/// <summary>Gets a shared <see cref="MD5"/> implementation.</summary>
		/// <remarks>The shared <see cref="MD5"/> object should be used with care, with the same rules as the shared buffer.</remarks>
		/// <value>A <see cref="MD5"/> object that can be used to compute a hash.</value>
		public static MD5 SharedMD5
		{
			get
			{
				if (sharedMD5 == null) sharedMD5 = MD5.Create();

				sharedMD5.Initialize();

				return sharedMD5;
			}
		}

		[ThreadStatic]
		private static SHA1 sharedSHA1;

		/// <summary>Gets a shared <see cref="SHA1"/> implementation.</summary>
		/// <remarks>The shared <see cref="SHA1"/> object should be used with care, with the same rules as the shared buffer.</remarks>
		/// <value>A <see cref="SHA1"/> object that can be used to compute a hash.</value>
		public static SHA1 SharedSHA1
		{
			get
			{
				if (sharedSHA1 == null) sharedSHA1 = SHA1.Create();

				sharedSHA1.Initialize();

				return sharedSHA1;
			}
		}

		public static bool CompareData(byte[] a, byte[] b)
		{
			if (a == null) throw new ArgumentNullException("a");
			if (b == null) throw new ArgumentNullException("b");

			if (a.Length != b.Length) throw new ArgumentException();

			for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;

			return true;
		}

		public static unsafe bool CompareData(byte[] a, byte* b)
		{
			if (a == null) throw new ArgumentNullException("a");
			if (b == null) throw new ArgumentNullException("b");

			for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;

			return true;
		}

		public static unsafe bool CompareData(byte* a, byte* b, uint length)
		{
			if (a == null) throw new ArgumentNullException("a");
			if (b == null) throw new ArgumentNullException("b");

			for (int i = 0; i < length; i++) if (a[i] != b[i]) return false;

			return true;
		}

		public static unsafe string FourCCToString(byte[] fourCC)
		{
			if (fourCC == null) throw new ArgumentNullException();
			if (fourCC.Length != 4) throw new ArgumentException();

			fixed (byte* fourCCPointer = fourCC)
				return FourCCToString(fourCCPointer);
		}

		public static unsafe string FourCCToString(byte* fourCC)
		{
			if (fourCC == null) throw new ArgumentNullException();

			var buffer = stackalloc char[4];

			buffer[0] = (char)fourCC[0];
			buffer[1] = (char)fourCC[1];
			buffer[2] = (char)fourCC[2];
			buffer[3] = (char)fourCC[3];

			return new string(buffer);
		}

		public static unsafe string FourCCToString(uint fourCC)
		{
			var buffer = stackalloc char[4];

			buffer[0] = (char)(fourCC & 0xFF);
			buffer[1] = (char)((fourCC >> 8) & 0xFF);
			buffer[2] = (char)((fourCC >> 16) & 0xFF);
			buffer[3] = (char)(fourCC >> 24);

			return new string(buffer);
		}

		#region SwapBytes

		#region UInt16

		public unsafe static void SwapBytes16(byte[] buffer)
		{
			fixed (byte* bufferPointer = buffer)
				SwapBytes((ushort*)bufferPointer, buffer.Length >> 1);
		}

		public static void SwapBytes(ushort[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
				buffer[i] = SwapBytes(buffer[i]);
		}

		public unsafe static void SwapBytes(ushort* buffer, int length)
		{
			for (int i = 0; i < length; i++)
				buffer[i] = SwapBytes(buffer[i]);
		}

		public static ushort SwapBytes(ushort value) { return (ushort)((value >> 8) | (value << 8)); }

		#endregion

		#region UInt32

		public unsafe static void SwapBytes32(byte[] buffer)
		{
			fixed (byte* bufferPointer = buffer)
				SwapBytes((uint*)bufferPointer, buffer.Length >> 2);
		}

		public static void SwapBytes(uint[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
				buffer[i] = SwapBytes(buffer[i]);
		}

		public unsafe static void SwapBytes(uint* buffer, int length)
		{
			for (int i = 0; i < length; i++)
				buffer[i] = SwapBytes(buffer[i]);
		}

		public unsafe static void SwapBytes(uint* buffer, ulong length)
		{
			for (ulong i = 0; i < length; i++)
				buffer[i] = SwapBytes(buffer[i]);
		}

		public static uint SwapBytes(uint value) { return (value >> 24) | (value >> 8) & 0x0000FF00 | (value << 8) & 0x00FF0000 | (value << 24); }

		#endregion

		#region UInt64

		public unsafe static void SwapBytes64(byte[] buffer)
		{
			fixed (byte* bufferPointer = buffer)
				SwapBytes((ulong*)bufferPointer, buffer.Length >> 3);
		}

		public static void SwapBytes(ulong[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
				buffer[i] = SwapBytes(buffer[i]);
		}

		public unsafe static void SwapBytes(ulong* buffer, int length)
		{
			for (int i = 0; i < length; i++)
				buffer[i] = SwapBytes(buffer[i]);
		}

		public static ulong SwapBytes(ulong value)
		{
			return (value >> 56) |
				(value >> 40) & 0x000000000000FF00 |
				(value >> 24) & 0x0000000000FF0000 |
				(value >> 8) & 0x00000000FF000000 |
				(value << 8) & 0x000000FF00000000 |
				(value << 24) & 0x0000FF0000000000 |
				(value << 40) & 0x00FF000000000000 |
				(value << 56);
		}

		public static uint SwapBytesIfNeeded(uint value) { return BitConverter.IsLittleEndian ? value : SwapBytes(value); }

		#endregion

		#endregion
	}
}
