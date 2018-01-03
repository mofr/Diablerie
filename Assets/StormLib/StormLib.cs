using System;
using System.Runtime.InteropServices;

namespace StormLib
{
    public class StormLib
    {
        [DllImport("storm")]
        // Returns a handle to the MPQ Search Object
        public static extern IntPtr SFileFindFirstFile(
            IntPtr archiveHandle,
            [MarshalAs(UnmanagedType.LPStr)] string mask,
            ref _SFILE_FIND_DATA findFileData,
            [MarshalAs(UnmanagedType.LPStr)] string listFile = null);

        [DllImport("storm")]
        public static extern bool SFileFindNextFile(IntPtr findHandle, ref _SFILE_FIND_DATA findFileData);

        [DllImport("storm")]
        public static extern bool SFileFindClose(IntPtr findHandle);

        [DllImport("storm")]
        public static extern bool SFileOpenFileEx(
            IntPtr archiveHandle,
            [MarshalAs(UnmanagedType.LPStr)] string fileName,
            [MarshalAs(UnmanagedType.U4)] OpenFileFlags searchScope,
            out IntPtr fileHandle);

        [DllImport("storm")]
        // Returns low 32 bits of file size
        public static extern uint SFileGetFileSize(IntPtr fileHandle, out Int64 fileSizeHigh);

        [DllImport("storm")]
        public static unsafe extern bool SFileReadFile(
            IntPtr fileHandle,
            [MarshalAs(UnmanagedType.LPArray)] byte* buffer,
            [MarshalAs(UnmanagedType.I8)] Int64 toRead,
            out Int64 read);

        [DllImport("storm")]
        public static extern bool SFileCloseFile(IntPtr fileHandle);
        
        [DllImport("storm")]
        public static extern bool SFileOpenArchive(
            [MarshalAs(UnmanagedType.LPStr)] string mpqFileName,
            uint priority,
            [MarshalAs(UnmanagedType.U4)] OpenArchiveFlags flags,
            out IntPtr archiveHandle);

        [DllImport("storm")]
        public static extern bool SFileCloseArchive(IntPtr archiveHandle);

        [DllImport("storm")]
        public static extern bool SFileHasFile(
            IntPtr archiveHandle,
            [MarshalAs(UnmanagedType.LPStr)] string filename);

        [DllImport("storm")]
        public static extern bool SFileExtractFile(
            IntPtr archiveHandle,
            [MarshalAs(UnmanagedType.LPStr)] string toExtract,
            [MarshalAs(UnmanagedType.LPStr)] string extracted,
            [MarshalAs(UnmanagedType.U4)] OpenFileFlags searchScope);

        [DllImport("storm")]
        public static extern bool SFileOpenPatchArchive(
            IntPtr archiveHandle,
            [MarshalAs(UnmanagedType.LPStr)] string mpqFileName,
            [MarshalAs(UnmanagedType.LPStr)] string patchPathPrefix,
            uint flags);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct _SFILE_FIND_DATA
    {
        /// char[260]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string fileName;

        /// char*
        public IntPtr plainName;

        /// DWORD->unsigned int
        public uint hashIndex;

        /// DWORD->unsigned int
        public uint blockIndex;

        /// DWORD->unsigned int
        public uint fileSize;

        /// DWORD->unsigned int
        public uint fileFlags;

        /// DWORD->unsigned int
        public uint compSize;

        /// DWORD->unsigned int
        public uint fileTimeLo;

        /// DWORD->unsigned int
        public uint fileTimeHi;

        /// LCID->DWORD->unsigned int
        public uint locale;
    }

    public enum OpenArchiveFlags : uint
    {
        NO_LISTFILE = 0x0010,   // Don't load the internal listfile
        NO_ATTRIBUTES = 0x0020,   // Don't open the attributes
        MFORCE_MPQ_V1 = 0x0040,   // Always open the archive as MPQ v 1.00, ignore the "wFormatVersion" variable in the header
        MCHECK_SECTOR_CRC = 0x0080,   // On files with MPQ_FILE_SECTOR_CRC, the CRC will be checked when reading file
        READ_ONLY = 0x0100,   // Open the archive for read-only access
        ENCRYPTED = 0x0200,   // Opens an encrypted MPQ archive (Example: Starcraft II installation)
    };
    
    public enum OpenFileFlags : uint
    {
        FROM_MPQ = 0x00000000,   // Open the file from the MPQ archive
        PATCHED_FILE = 0x00000001,   // Open the file from the MPQ archive
        BY_INDEX = 0x00000002,   // The 'szFileName' parameter is actually the file index
        ANY_LOCALE = 0xFFFFFFFE,   // Reserved for StormLib internal use
        LOCAL_FILE = 0xFFFFFFFF,   // Open the file from the MPQ archive
    };
}
