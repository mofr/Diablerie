using System;

namespace CrystalMpq
{
	public struct MpqMd5 : IComparable<MpqMd5>, IEquatable<MpqMd5>
	{
		private const int c_length = 16;

		private readonly byte[] m_bytes;

		public MpqMd5(byte[] bytes)
		{
			if (ReferenceEquals(bytes, null))
				throw new NullReferenceException("bytes");

			if (bytes.Length != c_length)
				throw new InvalidOperationException(string.Format("MD5 needs to be {} bytes.", c_length));

			m_bytes = bytes;
		}

		public int CompareTo(MpqMd5 other)
		{
			if (ReferenceEquals(m_bytes, null))
				return -1;
			if (ReferenceEquals(other.m_bytes, null))
				return 1;

			for (var i = 0; i < c_length; ++i)
			{
				int s = m_bytes[i].CompareTo(other.m_bytes[i]);
				if (s != 0)
					return s;
			}
			return 0;
		}

		public bool Equals(MpqMd5 other)
		{
			return CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return false;
			return Equals((MpqMd5)obj);
		}

		public override int GetHashCode()
		{
			var hash = 2166136261u;
			for (var i = 0; i < c_length; ++i)
			{
				hash = hash ^ m_bytes[i];
				hash = hash * 16777619;
			}
			return (int)hash;
		}

		public override string ToString()
		{
			return BitConverter.ToString(m_bytes).Replace("-", "").ToLower();
		}
	}
}
