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
	/// <summary>Thrown when a compression is not handled by the library.</summary>
	/// <remarks>Known but unsupported compressions will be reported by their name.</remarks>
	public sealed class MpqCompressionNotSupportedException : Exception
	{
		internal MpqCompressionNotSupportedException(byte methodId)
			: base(string.Format(ErrorMessages.GetString("CompressionNotSupported_Byte"), methodId)) { }

		internal MpqCompressionNotSupportedException(byte methodId, string methodName)
			: base(string.Format(ErrorMessages.GetString("CompressionNotSupported_Name"), methodName)) { }

		private MpqCompressionNotSupportedException(string errorMessageName, byte methodId, string methodName)
			: base(string.Format(ErrorMessages.GetString(errorMessageName), methodId))
		{
			CompressionMethodId = methodId;
			CompressionMethodName = methodName;
		}

		/// <summary>Gets the id of the unsupported compression method.</summary>
		/// <value>The compression method id.</value>
		public byte CompressionMethodId { get; private set; }
		/// <summary>Gets the name of the unsupported compression method.</summary>
		/// <value>The name of the compression method.</value>
		public string CompressionMethodName { get; private set; }
	}
}
