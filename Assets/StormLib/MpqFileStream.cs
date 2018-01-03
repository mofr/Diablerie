using System;
using System.IO;

namespace StormLib
{
    public class MpqFileStream : Stream
    {
        IntPtr handle;

        internal MpqFileStream(IntPtr handle)
        {
            this.handle = handle;
        }

        public sealed override bool CanRead { get { return true; } }
        public sealed override bool CanWrite { get { return false; } }
        public sealed override bool CanSeek { get { return true; } }

        public override long Length
        {
            get
            {
                long fileSizeHigh;
                long fileSize = StormLib.SFileGetFileSize(handle, out fileSizeHigh);
                return fileSize;
            }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Flush() { }

        public unsafe override int Read(byte[] buffer, int offset, int count)
        {
            fixed (byte* bufferPointer = buffer)
            {
                long bytesRead;
                if (!StormLib.SFileReadFile(handle, bufferPointer + offset, count, out bytesRead))
                    throw new IOException("SFileReadFile failed");
                return (int)bytesRead;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public sealed override void Close()
        {
            base.Close();
            if (handle != IntPtr.Zero)
            {
                StormLib.SFileCloseFile(handle);
            }
        }

        public byte[] ReadAllBytes()
        {
            byte[] bytes = new byte[Length];
            Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
