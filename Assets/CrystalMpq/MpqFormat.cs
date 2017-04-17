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
	/// <summary>
	/// This enumeraction gives information about the format of a given MPQ archive
	/// </summary>
	public enum MpqFormat
	{
		/// <summary>Original MPQ format.</summary>
		Original = 0,
		/// <summary>Extended MPQ format introduced with WoW - Brurning Crusade.</summary>
		/// <remarks>These archives can exceed the file size of 2 Gb, and possesses additionnal attributes for the contained files.</remarks>
		BurningCrusade = 1,
		/// <summary>Enhanced MPQ format introduced with WoW - Cataclysm.</summary>
		/// <remarks>These archives can provide increased performance via the new hash table format.</remarks>
		CataclysmFirst = 2,
		/// <summary>Enhanced MPQ format introduced with WoW - Cataclysm.</summary>
		/// <remarks>These archives build upon the previous format, providing more reliability and a potentially reduced file size.</remarks>
		CataclysmSecond = 3
	}
}
