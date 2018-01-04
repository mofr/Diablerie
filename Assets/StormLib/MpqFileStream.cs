using System;
using System.IO;

namespace StormLib
{
    public class MpqFileStream : Stream
    {
        IntPtr handle;
        long position = 0;

        internal MpqFileStream(IntPtr handle)
        {
            this.handle = handle;
        }

        public sealed override bool CanRead { get { return true; } }
        public sealed override bool CanSeek { get { return true; } }
        public sealed override bool CanWrite { get { return false; } }

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
                return position;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
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
                position += bytesRead;
                return (int)bytesRead;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            MoveMethod moveMethod = MoveMethod.Begin;
            switch (origin)
            {
                case SeekOrigin.Begin: moveMethod = MoveMethod.Begin; break;
                case SeekOrigin.Current: moveMethod = MoveMethod.Current; break;
                case SeekOrigin.End: moveMethod = MoveMethod.End; break;
            }

            var result = StormLib.SFileSetFilePointer(handle, offset, IntPtr.Zero, moveMethod);
            if (result == StormLib.SFILE_INVALID_SIZE)
                throw new IOException("SFileSetFilePointer failed");
            position = result;
            return position;
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
                handle = IntPtr.Zero;
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
