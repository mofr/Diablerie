using StormLib;

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
        UnityEngine.Profiling.Profiler.BeginSample("Mpq.ReadAllBytes");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            using (var stream = fs.OpenFile(filename))
            {
                byte[] bytes = stream.ReadAllBytes();
                UnityEngine.Debug.Log("Mpq.ReadAllBytes " + filename + " " + sw.ElapsedMilliseconds + " ms");
                return bytes;
            }
        }
        finally
        {
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }

    public unsafe static string ReadAllText(string filename)
    {
        UnityEngine.Profiling.Profiler.BeginSample("Mpq.ReadAllText");
        try
        {
            using (var stream = fs.OpenFile(filename))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                string result;
                fixed (byte* pointer = bytes)
                {
                    result = new string((sbyte*) pointer);
                }
                return result;
            }
        }
        finally
        {
            UnityEngine.Profiling.Profiler.EndSample();
        }
    }
}
