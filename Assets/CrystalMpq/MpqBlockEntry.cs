#region Copyright Notice
// This file is part of CrystalMPQ.
// 
// Copyright (C) 2007-2011 Fabien BARBIER
// 
// CrystalMPQ is licenced under the Microsoft Reciprocal License.
// You should find the licence included with the source of the program,
// or at this URL: http://www.microsoft.com/opensource/licenses.mspx#Ms-RL
#endregion

namespace CrystalMpq
{
	//[StructLayout(LayoutKind.Auto)]
	internal struct MpqBlockEntry
	{
		public string Name;
		public long Offset;
		public uint CompressedSize;
		public uint UncompressedSize;
		public MpqFileFlags Flags;
		public uint FileIndex;
		public uint Seed;
		public bool Listed;

		public int Crc32;
		public long FileTime;
		public MpqMd5 Md5;

		internal MpqBlockEntry(long offset, uint compressedSize, uint uncompressedSize, uint flags, ref uint fileIndex)
		{
			this.Offset = offset;
			this.CompressedSize = compressedSize;
			this.UncompressedSize = uncompressedSize;
			this.Flags = unchecked((MpqFileFlags)flags);
			this.Name = "";
			this.FileIndex = (this.Flags & MpqFileFlags.Exists) != 0 ? fileIndex++ : 0;
			this.Seed = 0;
			this.Listed = false;
			this.Crc32 = 0;
			this.FileTime = 0;
			this.Md5 = default(MpqMd5);
		}

		/// <summary>Called internally when the name has been detected.</summary>
		/// <param name="name">Detected filename.</param>
		/// <param name="cache">If set to <c>true</c>, remember the filename.</param>
		/// <param name="listed">If set to <c>true</c>, the name was detected from the listfile.</param>
		/// <remarks>Right now, the method will only update the seed when needed.</remarks>
		internal void OnNameDetected(string name, bool cache = false, bool listed = false)
		{
			if (!string.IsNullOrEmpty(this.Name)) return;

			// TODO: Improve the name caching mechanism (Global hash table for MPQ archives ?)
			if (cache || (this.Flags & MpqFileFlags.Encrypted) != 0)
				this.Seed = ComputeSeed(name);
			if (cache || (this.Flags & MpqFileFlags.Patch) != 0)
				this.Name = name; // Always cache the filename if the file is a patch… This is needed for base file lookup.
			if (cache) this.Listed = listed;
		}

		private static uint ComputeSeed(string filename)
		{
			// Calculate the seed based on the file name and not the full path.
			// I really don't know why but it worked with the full path for a lot of files…
			// But now it's fixed at least
			int index = filename.LastIndexOf('\\');
			return CommonMethods.Hash(index >= 0 ? filename.Substring(index + 1) : filename, 0x300);
		}
	}
}
