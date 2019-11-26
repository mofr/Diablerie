
// It's generated file. DO NOT MODIFY IT!
class MagicAffixModLoader : Datasheet.Loader<MagicAffix.Mod>
{

    public void LoadRecord(ref MagicAffix.Mod record, Datasheet.Stream stream)
    {
                stream.Read(ref record.code);
                stream.Read(ref record.param);
                stream.Read(ref record.min);
                stream.Read(ref record.max);
    }
}
