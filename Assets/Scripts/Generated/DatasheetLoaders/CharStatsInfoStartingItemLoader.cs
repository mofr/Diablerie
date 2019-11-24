
// It's generated file. DO NOT MODIFY IT!
class CharStatsInfoStartingItemLoader : Datasheet.Loader<CharStatsInfo.StartingItem>
{

    public void LoadRecord(ref CharStatsInfo.StartingItem record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.code);
                Datasheet.Parse(stream.NextString(), ref record.loc);
                Datasheet.Parse(stream.NextString(), ref record.count);
    }
}
