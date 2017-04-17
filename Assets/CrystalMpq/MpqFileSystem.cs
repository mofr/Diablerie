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
using System.Collections.ObjectModel;
using System.Text;
using CrystalMpq;

namespace CrystalMpq
{
	/// <summary>Represents a file system composed of multiple MPQ archives.</summary>
	/// <remarks>When searching a file, the first archives are always searched first.</remarks>
	public class MpqFileSystem : IMpqFileSystem
	{
		public sealed class MpqArchiveCollection : Collection<MpqArchive>
		{
			private readonly MpqFileSystem fileSystem;
			private readonly EventHandler<ResolveStreamEventArgs> baseFileResolver;

			internal MpqArchiveCollection(MpqFileSystem fileSystem)
				: base(fileSystem.archiveList)
			{
				this.fileSystem = fileSystem;
				this.baseFileResolver = fileSystem.ResolveBaseFile;
			}

			protected sealed override void InsertItem(int index, MpqArchive item)
			{
				base.InsertItem(index, item);
				item.ResolveBaseFile += baseFileResolver;
			}

			protected sealed override void SetItem(int index, MpqArchive item)
			{
				fileSystem.archiveList[index].ResolveBaseFile -= baseFileResolver;
				base.SetItem(index, item);
				item.ResolveBaseFile += baseFileResolver;
			}

			protected sealed override void RemoveItem(int index)
			{
				fileSystem.archiveList[index].ResolveBaseFile -= baseFileResolver;
				base.RemoveItem(index);
			}

			protected sealed override void ClearItems()
			{
				foreach (var archive in fileSystem.archiveList)
					archive.ResolveBaseFile -= baseFileResolver;
				base.ClearItems();
			}
		}

		private readonly List<MpqArchive> archiveList;
		private readonly MpqArchiveCollection archiveCollection;
		private readonly bool shouldRetrieveDeletedFiles;

		/// <summary>Initializes a new instance of the <see cref="MpqFileSystem"/> class.</summary>
		/// <remarks>
		/// MPQ patches have the possibility to delete files, making them unavailable in further versions.
		/// This constructor will enforce the restriction and prevent you from getting access to deleted files.
		/// The files marked as deleted are likely to go away after a major game update, so accessing them is not recommended.
		/// If you really need to get access to those files, you can use the other constructor and access them as if they were never deleted.
		/// </remarks>
		public MpqFileSystem() : this(false) { }

		/// <summary>Initializes a new instance of the <see cref="MpqFileSystem"/> class.</summary>
		/// <param name="shouldRetrieveDeletedFiles">if set to <see langword="true"/>, files deleted in a patch will be retrieved.</param>
		public MpqFileSystem(bool shouldRetrieveDeletedFiles)
		{
			this.archiveList = new List<MpqArchive>();
			this.archiveCollection = new MpqArchiveCollection(this);
			this.shouldRetrieveDeletedFiles = shouldRetrieveDeletedFiles;
		}

		public void Dispose()
		{
			foreach (var archive in archiveList)
				archive.Dispose();
		}

		/// <summary>Gets the collection of <see cref="MpqArchive"/>.</summary>
		/// <remarks>Archives should be added to this collection for being searched.</remarks>
		/// <value>The archive list.</value>
		public MpqArchiveCollection Archives { get { return archiveCollection; } }
		IList<MpqArchive> IMpqFileSystem.Archives { get { return archiveCollection; } }

		private void ResolveBaseFile(object sender, ResolveStreamEventArgs e)
		{
			var file = sender as MpqFile;

			if (file == null) throw new InvalidOperationException();

			bool archiveFound = false;

			foreach (var archive in archiveList)
			{
				if (!archiveFound)
				{
					if (archive == file.Archive) archiveFound = true;
					continue;
				}

				var foundFile = archive.FindFile(file.Name);

				if (foundFile != null)
				{
					e.Stream = foundFile.Open();
					return;
				}
			}
		}

		[Obsolete]
		public MpqFile[] FindFiles(string filename)
		{
			foreach (var archive in archiveList)
			{
				var files = archive.FindFiles(filename);

				if (files.Length > 0)
				{
					int deletedFileCount = 0;

					foreach (var file in files)
						if ((file.Flags & MpqFileFlags.Deleted) != 0)
							deletedFileCount++;

					if (deletedFileCount == 0) return files;
					else if (deletedFileCount == files.Length) break;

					var existingFiles = new MpqFile[files.Length - deletedFileCount];

					int i = 0;

					foreach (var file in files)
						if ((file.Flags & MpqFileFlags.Deleted) == 0)
							existingFiles[i++] = file;

					return existingFiles;
				}
			}
			return new MpqFile[0];
		}

		[Obsolete]
		public MpqFile FindFile(string filename, int lcid)
		{
			foreach (var archive in archiveList)
			{
				var file = archive.FindFile(filename, lcid);

				if (file != null)
					if (!file.IsDeleted) return file;
					else if (!shouldRetrieveDeletedFiles) return null;
			}
			return null;
		}

		public MpqFile FindFile(string filename)
		{
			foreach (var archive in archiveList)
			{
				var file = archive.FindFile(filename);

				if (file != null)
					if (!file.IsDeleted) return file;
					else if (!shouldRetrieveDeletedFiles) return null;
			}
			return null;
		}
	}
}
