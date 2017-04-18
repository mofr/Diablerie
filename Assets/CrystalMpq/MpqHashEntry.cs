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
using System.Runtime.InteropServices;

namespace CrystalMpq
{
	[StructLayout(LayoutKind.Auto)]
	internal struct MpqHashEntry
	{
		public static readonly MpqHashEntry Invalid = new MpqHashEntry();

		private uint hashA;
		private uint hashB;
		private int locale;
		private int block;

		public MpqHashEntry(uint hashA, uint hashB, int locale, int block)
		{
			this.hashA = hashA;
			this.hashB = hashB;
			this.locale = locale;
			this.block = block;
		}

		public bool Test(uint hashA, uint hashB) { return hashA == this.hashA && hashB == this.hashB; }

		public int Locale { get { return locale; } }

		public int Block { get { return block; } }

		public bool IsValid { get { return block != -1 && hashA != 0xFFFFFFFF && hashA != 0xFFFFFFFF; } }
	}
}
