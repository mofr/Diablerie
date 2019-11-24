
// It's generated file. DO NOT MODIFY IT!
class MagicAffixModLoader : Datasheet.Loader<MagicAffix.Mod>
{

    public void LoadRecord(ref MagicAffix.Mod record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.code);
                Datasheet.Parse(stream.NextString(), ref record.param);
                Datasheet.Parse(stream.NextString(), ref record.min);
                Datasheet.Parse(stream.NextString(), ref record.max);
    }
}
