using CrystalMpq;

public class Mpq
{
    static MpqArchive patch_d2 = new MpqArchive("patch_d2.mpq");
    static MpqArchive d2exp = new MpqArchive("d2exp.mpq");
    static MpqArchive d2data = new MpqArchive("d2data.mpq");
    static MpqArchive d2char = new MpqArchive("d2char.mpq");
    static public MpqFileSystem fs = new MpqFileSystem();

    static Mpq()
    {
        fs.Archives.Add(patch_d2);
        fs.Archives.Add(d2exp);
        fs.Archives.Add(d2data);
        fs.Archives.Add(d2char);
    }

    public static byte[] ReadAllBytes(string filename)
    {
        //UnityEngine.Profiling.Profiler.BeginSample("File.ReadAllBytes");
        //var bytes = System.IO.File.ReadAllBytes(UnityEngine.Application.streamingAssetsPath + "/d2/" + filename);
        //UnityEngine.Profiling.Profiler.EndSample();
        //return bytes;

        UnityEngine.Profiling.Profiler.BeginSample("Mpq.FindFile");
        var file = fs.FindFile(filename);
        UnityEngine.Profiling.Profiler.EndSample();
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
}
