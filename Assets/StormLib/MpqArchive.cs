using System;
using System.IO;

namespace StormLib
{
    public class MpqArchive : IDisposable
    {
        IntPtr handle = IntPtr.Zero;

        public MpqArchive(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);
            if (!StormLib.SFileOpenArchive(filename, 0, OpenArchiveFlags.READ_ONLY, out handle))
                throw new IOException("SFileOpenArchive failed");
        }

        public bool HasFile(string filename)
        {
            return StormLib.SFileHasFile(handle, filename);
        }

        public MpqFileStream OpenFile(string filename)
        {
            if (!HasFile(filename))
                throw new FileNotFoundException();

            IntPtr fileHandle;
            if (!StormLib.SFileOpenFileEx(handle, filename, OpenFileFlags.FROM_MPQ, out fileHandle))
                throw new IOException("SFileOpenFileEx failed");

            return new MpqFileStream(fileHandle);
        }

        public void Close()
        {
            if (handle != IntPtr.Zero)
            {
                StormLib.SFileCloseArchive(handle);
                handle = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
