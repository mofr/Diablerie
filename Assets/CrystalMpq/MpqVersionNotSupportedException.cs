using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalMpq
{
	/// <summary>Exception thrown when the MPQ version is not recognized by the library.</summary>
	public sealed class MpqVersionNotSupportedException : Exception
	{
		/// <summary>Initializes a new instance of the <see cref="MpqVersionNotSupportedException"/> class.</summary>
		internal MpqVersionNotSupportedException(ushort version)
			: base(ErrorMessages.GetString(string.Format("MpqVersionNotSupported", version))) { }
	}
}
