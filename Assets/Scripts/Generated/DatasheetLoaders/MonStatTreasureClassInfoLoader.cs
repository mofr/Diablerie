
// It's generated file. DO NOT MODIFY IT!
class MonStatTreasureClassInfoLoader : Datasheet.Loader<MonStat.TreasureClassInfo>
{

    public void LoadRecord(ref MonStat.TreasureClassInfo record, Datasheet.Stream stream)
    {
                stream.Read(ref record._normal);
                stream.Read(ref record._champion);
                stream.Read(ref record._unique);
                stream.Read(ref record._quest);
    }
}
