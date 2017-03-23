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
using System.Threading;

namespace CrystalMpq
{
	/// <summary>This class represents a file stored in an <see cref="MpqArchive"/>.</summary>
	public class MpqFile
	{
		private readonly MpqArchive archive;
		private uint blockIndex;

		internal MpqFile(MpqArchive archive, uint index)
		{
			if (archive == null) throw new ArgumentNullException("archive");

			this.archive = archive;
			this.blockIndex = index;
		}

		//internal void BindHashTableEntry(MpqHashTable.HashEntry hashEntry) { this.hashEntry = hashEntry; }

		/// <summary>Gets the archive to whom this file belongs.</summary>
		public MpqArchive Archive { get { return archive; } }

		/// <summary>Gets the name for this file, or null if the filename is not known.</summary>
		public string Name { get { return archive.blockTable.Entries[blockIndex].Name; } }

		/// <summary>Gets the offset of this file in the archive.</summary>
		public long Offset { get { return archive.blockTable.Entries[blockIndex].Offset; } }

		/// <summary>Gets the size of this file.</summary>
		public long Size { get { return archive.blockTable.Entries[blockIndex].UncompressedSize; } }

		/// <summary>Gets the compressed size of this file.</summary>
		/// <remarks>If the file is not compressed, CompressedSize will return the same value than Size.</remarks>
		public long CompressedSize { get { return archive.blockTable.Entries[blockIndex].CompressedSize; } }

		/// <summary>Gets the flags that apply to this file.</summary>
		public MpqFileFlags Flags { get { return archive.blockTable.Entries[blockIndex].Flags; } }

		/// <summary>Gets a value indicating whether this file is encrypted.</summary>
		/// <value><c>true</c> if this file is encrypted; otherwise, <c>false</c>.</value>
		public bool IsEncrypted { get { return (archive.blockTable.Entries[blockIndex].Flags & MpqFileFlags.Encrypted) != 0; } }

		/// <summary>Gets a value indicating whether this file is compressed.</summary>
		/// <value><c>true</c> if this file is compressed; otherwise, <c>false</c>.</value>
		public bool IsCompressed { get { return (archive.blockTable.Entries[blockIndex].Flags & MpqFileFlags.Compressed) != 0; } }

		/// <summary>Gets a value indicating whether this file is a patch.</summary>
		/// <value><c>true</c> if this file is a patch; otherwise, <c>false</c>.</value>
		public bool IsPatch { get { return (archive.blockTable.Entries[blockIndex].Flags & MpqFileFlags.Patch) != 0; } }

		/// <summary>Gets a value indicating whether this file has been deleted.</summary>
		/// <remarks>The deleted status indicates that the file has been deleted in the current mpq patch archive.</remarks>
		/// <value><c>true</c> if this file has been deleted; otherwise, <c>false</c>.</value>
		public bool IsDeleted { get { return (archive.blockTable.Entries[blockIndex].Flags & MpqFileFlags.Deleted) != 0; } }

		///// <summary>Gets the LCID associated with this file.</summary>
		//public int Lcid { get { return hashEntry.Locale; } }

		/// <summary>Gets the CRC32 of this file.</summary>
		public int Crc32 { get { return archive.blockTable.Entries[blockIndex].Crc32; } }

		/// <summary>Gets the MD5 of this file.</summary>
		public MpqMd5 Md5 { get { return archive.blockTable.Entries[blockIndex].Md5; } }

		/// <summary>Gets the index of the file in the block table.</summary>
		/// <remarks>This property is for internal use only.</remarks>
		internal uint BlockIndex { get { return blockIndex; } }

		/// <summary>Gets the seed associated with this file.</summary>
		/// <remarks>The seed is a value that is used internally to decrypt some files.</remarks>
		/// <value>The seed associated with this file.</value>
		internal uint Seed { get { return archive.blockTable.Entries[blockIndex].Seed; } }

		/// <summary>Gets a value indicating whether the file was found in the list file of the archive.</summary>
		/// <remarks>This can only be true if the list file was parsed.</remarks>
		/// <value><c>true</c> if this instance is listed; otherwise, <c>false</c>.</value>
		public bool IsListed { get { return archive.blockTable.Entries[blockIndex].Listed; } }

		/// <summary>Opens the file for reading.</summary>
		/// <returns>Returns a Stream object which can be used to read data in the file.</returns>
		/// <remarks>Files can only be opened once, so don't forget to close the stream after you've used it.</remarks>
		public MpqFileStream Open() 
		{
			if (IsDeleted) throw new InvalidOperationException(ErrorMessages.GetString("MpqFileDeleted"));

			return new MpqFileStream(this);
		}

		/// <summary>Opens a patched file for reading.</summary>
		/// <param name="baseStream">A base stream.</param>
		/// <returns>Returns a Stream object which can be used to read data in the file.</returns>
		/// <remarks>
		/// This method should only be used for explicitly providing a base stream when the <see cref="MpqFile"/> is a patch.
		/// Files can only be opened once, so don't forget to close the stream after you've used it.
		/// </remarks>
		/// <exception cref="InvalidOperationException">This instance of <see cref="MpqFile"/> is not a patch.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="baseStream"/> is <c>null</c>.</exception>
		public MpqFileStream Open(Stream baseStream)
		{
			if (!IsPatch) throw new InvalidOperationException();

			if (baseStream == null) throw new ArgumentNullException("baseStream");

			return new MpqFileStream(this);
		}
	}
}
