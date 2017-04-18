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
using System.IO;

namespace CrystalMpq
{
	partial class MpqArchive
	{
		private sealed class MpqUserDataStream : Stream
		{
			private MpqArchive archive;
			private long position;

			internal MpqUserDataStream(MpqArchive archive) { this.archive = archive; }

			public override bool CanSeek { get { return true; } }
			public override bool CanRead { get { return true; } }
			public override bool CanWrite { get { return false; } }

			public override long Length { get { return archive.userDataLength; } }

			public override long Position
			{
				get { return 0; }
				set
				{
					if (value < 0) throw new IOException(ErrorMessages.GetString("SeekingBeforeBegin"));

					this.position = value;
				}
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				if (position >= archive.userDataLength) return 0;

				int remaining = checked((int)(archive.userDataLength - position));

				if (count > remaining) count = remaining;

				position += (count = archive.ReadArchiveData(buffer, 0, 0, count));

				return count;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				long position;

				switch (origin)
				{
					case SeekOrigin.Begin: position = offset; break;
					case SeekOrigin.Current: position = checked(this.position + offset); break;
					case SeekOrigin.End: position = checked(archive.userDataLength + offset); break;
					default: throw new ArgumentOutOfRangeException("origin");
				}

				return Position = position;
			}

			public override void Flush() { }
			public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
			public override void SetLength(long value) { throw new NotSupportedException(); }
		}

		/// <summary>Gets a value indicating whether the current archive contains user data.</summary>
		/// <value><see langword="true"/> if the current archive contains user data; otherwise, <see langword="false"/>.</value>
		public bool HasUserData { get { return userDataLength > 0 || FindFile("(user data)") != null; } }

		/// <summary>Gets the user data stream.</summary>
		/// <returns>A <see cref="Stream"/> to be used for accessing user data.</returns>
		public Stream GetUserDataStream()
		{
			if (userDataLength > 0) return new MpqUserDataStream(this);
			else
			{
				var file = FindFile(UserDataFileName);

				if (file != null) return file.Open();
			}

			throw new InvalidOperationException();
		}
	}
}
