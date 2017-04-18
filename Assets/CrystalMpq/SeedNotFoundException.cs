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
	/// <summary>Thrown when the seed for a file is not known.</summary>
	/// <remarks>The seed is needed for reading encrypted files.</remarks>
	public sealed class SeedNotFoundException : Exception
	{
		internal SeedNotFoundException(long block)
			: base(string.Format(ErrorMessages.GetString("SeedNotFound"), block)) { }
	}
}
