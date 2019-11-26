
// It's generated file. DO NOT MODIFY IT!
class CharStatsInfoStartingItemLoader : Datasheet.Loader<CharStatsInfo.StartingItem>
{

    public void LoadRecord(ref CharStatsInfo.StartingItem record, Datasheet.Stream stream)
    {
                stream.Read(ref record.code);
                stream.Read(ref record.loc);
                stream.Read(ref record.count);
    }
}
