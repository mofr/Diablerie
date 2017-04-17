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
using System.Text;

namespace CrystalMpq
{
	internal struct MpqBlockTable
	{
		internal static unsafe MpqBlockTable FromMemory(uint* table, ushort* highTable, long tableLength, out uint fileCount)
		{
			var entries = new MpqBlockEntry[tableLength];

			fileCount = 0;

			if (highTable == null)
				for (int i = 0; i < entries.Length; i++) // Fill MpqHashTable object
					entries[i] = new MpqBlockEntry(*table++, *table++, *table++, *table++, ref fileCount);
			else
				for (int i = 0; i < entries.Length; i++) // Fill MpqHashTable object
					entries[i] = new MpqBlockEntry(*table++ | ((long)*highTable++ << 32), *table++, *table++, *table++, ref fileCount);

			return new MpqBlockTable(entries);
		}

		internal readonly MpqBlockEntry[] Entries;

		private MpqBlockTable(MpqBlockEntry[] entries) { this.Entries = entries; }
	}
}
