#region Copyright Notice
// This file is part of CrystalMPQ.
// 
// Copyright (C) 2007-2011 Fabien BARBIER
// 
// CrystalMPQ is licenced under the Microsoft Reciprocal License.
// You should find the licence included with the source of the program,
// or at this URL: http://www.microsoft.com/opensource/licenses.mspx#Ms-RL
#endregion

// Using a filename cache based on Dictionary actually consumes much more memory (i measured over 25MB !) than what it could save…
// Will have to find another way to cut down memory usage.
#if false
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CrystalMpq
{
	/// <summary>Represents a cache of file names for <see cref="MpqArchive"/>.</summary>
	/// <remarks>
	/// Multiple <see cref="MpqArchive"/> in the same file system are likely to share the same filename.
	/// Using a common cache for these archives will allow a reduction in memory footprint, and possibly an increase in cache efficiency.
	/// </remarks>
	public sealed class MpqNameCache
	{
		private readonly Dictionary<string, string> cacheDictionary;
		private int readerCount;
		private volatile bool writeAccessRequired;

		/// <summary>Initializes a new instance of the <see cref="MpqNameCache"/> class.</summary>
		public MpqNameCache() { cacheDictionary = new Dictionary<string, string>(); }

		/// <summary>Determines whether the specified filename is cached.</summary>
		/// <param name="value">The value to look up in the cache.</param>
		/// <returns>A reference to the cached value, or <see langword="null"/> if the value was not in the cache.</returns>
		public string IsFilenameCached(string value) { return GetCachedFilenameInternal(value, false); }
		internal string GetCachedFilename(string value) { return GetCachedFilenameInternal(value, true); }

		private string GetCachedFilenameInternal(string value, bool allowCaching)
		{
			// The implementation of this method would certainly be better with such a thing as a ConcurrentDictionary, but it doesn't exist in .NET 2.0.
			// My goal still is to keep .NET 2.0 binary compatibility (the code already isn't C# 2.0 anymore).
			// This implementation should work fine, provided I did no mistake…

			string cachedValue;

			// Handle basic cases
			if (value == null) return null; // We can't cache null values anyway…
			if (value.Length == 0) return string.Empty; // String.Empty is always interned… (At least it should be)

			// This conditional lock avoids costly operations and possible deadlocks.
			// We need to check for write requests twice, and this lock is not the most important one, but:
			//  - This avoids incrementing and decrementing the reader counter uselessly in the cases where we already know a write operation is pending.
			//  - This prevents the writer thread to wait indefinitely for the reader count to drop to zero in cases where there are lots of readers.
			if (writeAccessRequired) lock (cacheDictionary) { }

			// Request read access to the dictionary.
			Interlocked.Increment(ref readerCount);

			// If write access has been requested, wait for the requesting thread to do its work.
			// This conditional lock is strictly needed for data coherence.
			if (writeAccessRequired)
			{
				// For the time being, we are not a reader anymore
				Interlocked.Decrement(ref readerCount);

				// Take an exclusive lock on the dictionary
				lock (cacheDictionary)
					Interlocked.Increment(ref readerCount); // We cannot increment the reader counter 
			}

			// Once we secured our read access to the dictionary, lookup the value
			bool found = cacheDictionary.TryGetValue(value, out cachedValue);

			// Now that we're done reading
			Interlocked.Decrement(ref readerCount);

			if (!found)
			{
				// If the value is not found, we require acces to the dictionary, but this time for writing
				lock (cacheDictionary)
				{
					// Check the dictionary again, just to be sure
					if (!cacheDictionary.TryGetValue(value, out cachedValue))
					{
						writeAccessRequired = true;

						do Thread.MemoryBarrier();
						while (readerCount != 0);

						cacheDictionary.Add(value, cachedValue = value);

						writeAccessRequired = false;
					}
				}
			}

			return cachedValue;
		}
	}
}
#endif
