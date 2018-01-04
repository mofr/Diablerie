using System;
using System.Collections.Generic;
using System.IO;

namespace StormLib
{
    public class MpqFileSystem : IDisposable
    {
        private List<MpqArchive> archives = new List<MpqArchive>();

        public IList<MpqArchive> Archives
        {
            get
            {
                return archives;
            }
        }

        public bool HasFile(string filename)
        {
            foreach (var archive in archives)
            {
                if (archive.HasFile(filename))
                    return true;
            }
            return false;
        }
        
        public MpqFileStream OpenFile(string filename)
        {
            foreach (var archive in archives)
            {
                if (archive.HasFile(filename))
                    return archive.OpenFile(filename);
            }

            throw new FileNotFoundException(filename);
        }

        public void Dispose()
        {
            foreach (var archive in archives)
                archive.Dispose();
        }
    }
}
