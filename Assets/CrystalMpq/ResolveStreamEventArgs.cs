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
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace CrystalMpq
{
	/// <summary><see cref="ResolveStreamEventArgs"/> is used for applying a patch.</summary>
	/// <remarks>It is the responsibility to provide a valid stream.</remarks>
	public sealed class ResolveStreamEventArgs : EventArgs, IDisposable
	{
		private Stream stream;

		internal ResolveStreamEventArgs() { } // Even though this class is quite simple and could have other uses, we don't want anyone to use it for unintended pruposes…

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			stream.Dispose();
			stream = null;
		}

		/// <summary>Gets or sets the stream containing the data for the base file.</summary>
		/// <remarks>The instance of <see cref="ResolveStreamEventArgs"/> will take ownership of the stream.</remarks>
		/// <value>The stream.</value>
		public Stream Stream
		{
			get { return stream; }
			set
			{
				if (value != stream)
				{
					if (stream != null) stream.Dispose();
					stream = value;
				}
			}
		}

		/// <summary>Transfers the stream ownership to the caller.</summary>
		/// <remarks>
		/// This method will return the value of <see cref="Stream"/> and set the property to <c>null</c> afterwards.
		/// After a call to this method, the caller becomes responsible for managing the stream.
		/// </remarks>
		/// <returns>The <see cref="System.IO.Stream"/> previously contained in this instance.</returns>
		internal Stream TransferStreamOwnership()
		{
			var stream = this.stream;
			this.stream = null;
			return stream;
		}
	}
}
