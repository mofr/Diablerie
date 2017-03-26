using CrystalMpq;

public class Mpq
{
    static public MpqArchive d2exp = new MpqArchive("d2exp.mpq");
    static public MpqArchive d2data = new MpqArchive("d2data.mpq");
    static public MpqArchive d2char = new MpqArchive("d2char.mpq");
    static public MpqFileSystem fs = new MpqFileSystem();

    static Mpq()
    {
        fs.Archives.Add(d2exp);
        fs.Archives.Add(d2data);
        fs.Archives.Add(d2char);
    }

    public static byte[] ReadAllBytes(string filename)
    {
        var file = fs.FindFile(filename);
        return ReadAllBytes(file);
    }

    public static byte[] ReadAllBytes(MpqFile file)
    {
        UnityEngine.Profiling.Profiler.BeginSample("Mpq.ReadAllBytes");
        using (var stream = file.Open())
        {
            byte[] bytes = new byte[file.Size];
            stream.Read(bytes, 0, bytes.Length);
            UnityEngine.Profiling.Profiler.EndSample();
            return bytes;
        }
    }

    public unsafe static string ReadAllText(string filename)
    {
        UnityEngine.Profiling.Profiler.BeginSample("Mpq.ReadAllText");
        var file = fs.FindFile(filename);
        using (var stream = file.Open())
        {
            byte[] bytes = new byte[file.Size];
            stream.Read(bytes, 0, bytes.Length);
            string result;
            fixed (byte * pointer = bytes)
            {
                result = new string((sbyte*)pointer);
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return result;
        }
    }
}
