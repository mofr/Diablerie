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
using System.Security.Cryptography;
using System.Text;

namespace CrystalMpq
{
	partial class MpqArchive
	{
		/// <summary>Gets a value that indicate whether the current archive has a weak siganture.</summary>
		/// <remarks>Some archives may be signed, which allows us to check archive integrity.</remarks>
		/// <value><see langword="true"/> if the current archive has a weak siganture; otherwise, <see langword="false"/>.</value>
		public bool HasWeakSignature { get { return FindFile("(signature)") != null; } }

		public bool CheckBlizzardWeakSignature()
		{
			var publicKey = new RSACryptoServiceProvider();

			using (var stream = typeof(MpqArchive).Assembly.GetManifestResourceStream("CrystalMpq.Keys.Blizzard Weak.xml"))
			using (var reader = new StreamReader(stream))
				publicKey.FromXmlString(reader.ReadToEnd());

			return CheckWeakSignature(publicKey);
		}

		public bool CheckWeakSignature(RSAParameters publicKey)
		{
			var rsa = RSA.Create();

			rsa.ImportParameters(publicKey);

			return CheckWeakSignature(rsa);
		}

		public bool CheckWeakSignature(RSA publicKey)
		{
			var file = FindFile(WeakSignatureFileName);

			if (file == null) throw new InvalidOperationException();

			// As of now, the signature file can only be 72 bytes long (8 unused bytes + 64 signature bytes)
			if (file.Size != 72 || file.CompressedSize > file.Size) throw new InvalidDataException();

			// Read the signature from the internal file
			byte[] signature = new byte[64];

			using (var stream = file.Open())
			{
				stream.Seek(8, SeekOrigin.Begin);

				if (stream.Read(signature, 0, signature.Length) != signature.Length) throw new EndOfStreamException();
			}

			// Hash the whole archive with the MD5 algorithm
			var md5 = CommonMethods.SharedMD5;

			lock (syncRoot)
			{
				var buffer = new byte[4096];
				long bytesToSignature = file.Offset;
				long bytesRemaining = archiveDataLength;

				stream.Seek(archiveDataOffset, SeekOrigin.Begin);

				do
				{
					int count = stream.Read(buffer, 0, bytesRemaining > buffer.Length ? buffer.Length : (int)bytesRemaining);

					// Null out the signature from the data (Note that the signature should normally not be compressed, but just in case, we use CompressedSize)
					if (bytesToSignature < count && bytesToSignature + file.CompressedSize > 0)
						Array.Clear(buffer, Math.Min((int)bytesToSignature, 0), (int)Math.Min(file.CompressedSize + bytesToSignature, count) + (bytesToSignature > 0 ? (int)bytesToSignature : 0));

					bytesToSignature -= count;
					bytesRemaining -= count;

					if (bytesRemaining <= 0)
					{
						md5.TransformFinalBlock(buffer, 0, count);
						break;
					}
					else md5.TransformBlock(buffer, 0, count, null, 0);
				}
				while (true);
			}

			return new RSAPKCS1SignatureDeformatter(publicKey).VerifySignature(md5, signature);
		}

		public bool HasStrongSignature { get { return hasStrongSignature; } }

		public bool CheckBlizzardStrongSignature()
		{
			var publicKey = new RSACryptoServiceProvider();

			using (var stream = typeof(MpqArchive).Assembly.GetManifestResourceStream("CrystalMpq.Keys.Blizzard Strong.xml"))
			using (var reader = new StreamReader(stream))
				publicKey.FromXmlString(reader.ReadToEnd());

			return CheckStrongSignature(publicKey, null as byte[]);
		}

		public bool CheckStrongSignature(RSAParameters publicKey, string tail) { return CheckStrongSignature(publicKey, tail != null ? Encoding.ASCII.GetBytes(tail) : null); }

		public bool CheckStrongSignature(RSAParameters publicKey, byte[] tail)
		{
			var rsa = RSA.Create();

			rsa.ImportParameters(publicKey);

			return CheckStrongSignature(rsa, tail);
		}

		public bool CheckStrongSignature(RSA publicKey, string tail) { return CheckStrongSignature(publicKey, tail != null ? Encoding.ASCII.GetBytes(tail) : null); }

		public bool CheckStrongSignature(RSA publicKey, byte[] tail)
		{
			if (!hasStrongSignature) throw new InvalidOperationException();

			// Hash the whole archive with the SHA1 algorithm
			var sha1 = CommonMethods.SharedSHA1;
			byte[] signature = new byte[2048];

			lock (syncRoot)
			{
				var buffer = new byte[4096];
				long bytesRemaining = archiveDataLength;

				stream.Seek(archiveDataOffset, SeekOrigin.Begin);

				do
				{
					int count = stream.Read(buffer, 0, bytesRemaining > buffer.Length ? buffer.Length : (int)bytesRemaining);

					sha1.TransformBlock(buffer, 0, count, null, 0);

					bytesRemaining -= count;
				}
				while (bytesRemaining > 0);

				sha1.TransformFinalBlock(tail, 0, tail != null ? tail.Length : 0);

				stream.Seek(sizeof(uint), SeekOrigin.Current); // Skip the strong signature header, as it has already been verified earlier

				if (stream.Read(signature, 0, signature.Length) != signature.Length) throw new EndOfStreamException();
			}

			return new RSAPKCS1SignatureDeformatter(publicKey).VerifySignature(sha1, signature);
		}
	}
}
