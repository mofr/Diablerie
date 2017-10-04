using CrystalMpq;

public class Mpq
{
    static public MpqFileSystem fs = new MpqFileSystem();

    static Mpq()
    {
        AddArchive("d2exp.mpq");
        AddArchive("d2data.mpq");
        AddArchive("d2char.mpq");
        AddArchive("d2sfx.mpq", optional: true);
        AddArchive("d2music.mpq", optional: true);
        AddArchive("d2xMusic.mpq", optional: true);
        AddArchive("d2xtalk.mpq", optional: true);
        AddArchive("d2speech.mpq", optional: true);
    }

    static private void AddArchive(string filename, bool optional = false)
    {
        try
        {
            var archive = new MpqArchive(filename);
            fs.Archives.Add(archive);
        }
        catch (System.IO.FileNotFoundException)
        {
            if (!optional)
                throw;
        }
    }

    public static byte[] ReadAllBytes(string filename)
    {
        var file = fs.FindFile(filename);
        if (file == null)
            throw new System.IO.FileNotFoundException("file not found " + filename, filename);
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
