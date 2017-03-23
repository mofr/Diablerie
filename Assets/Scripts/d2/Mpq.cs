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
}
