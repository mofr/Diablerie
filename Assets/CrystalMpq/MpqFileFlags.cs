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
	/// <summary>Flags which can be applied to files in a MPQ archive.</summary>
	[Flags]
	public enum MpqFileFlags
	{
		/// <summary>The file is compressed using DCL compression only.</summary>
		DclCompressed = 0x100,
		/// <summary>The file is compressed using Blizzard's multiple compression system.</summary>
		MultiCompressed = 0x200,
		/// <summary>The file is compressed using either method.</summary>
		Compressed = 0x300,
		/// <summary>The file is encrypted with a seed.</summary>
		Encrypted = 0x10000,
		/// <summary>The encryption seed is altered with the file offset.</summary>
		PositionEncrypted = 0x20000,
		/// <summary>The file is a patch.</summary>
		/// <remarks>This should only be found whithin patch archives.</remarks>
		Patch = 0x100000,
		/// <summary>The file is stored as a single unit and not in multiple blocks.</summary>
		/// <remarks>This was added with Burning Crusade.</remarks>
		SingleBlock = 0x1000000,
		/// <summary>The file was deleted in a patch.</summary>
		Deleted = 0x2000000,
		/// <summary>The file has a CRC for each sector.</summary>
		SectorCrc = 0x4000000,
		/// <summary>The file exists. This flag should always be set for valid files.</summary>
		Exists = -0x7FFFFFFF
	}
}