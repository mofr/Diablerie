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

namespace CrystalMpq
{
	public interface IMpqFileSystem : IDisposable
	{
		MpqFile FindFile(string filename);

		IList<MpqArchive> Archives { get; }
	}
}
