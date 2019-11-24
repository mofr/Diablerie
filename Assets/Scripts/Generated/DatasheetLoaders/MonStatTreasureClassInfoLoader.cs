
// It's generated file. DO NOT MODIFY IT!
class MonStatTreasureClassInfoLoader : Datasheet.Loader<MonStat.TreasureClassInfo>
{

    public void LoadRecord(ref MonStat.TreasureClassInfo record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record._normal);
                Datasheet.Parse(stream.NextString(), ref record._champion);
                Datasheet.Parse(stream.NextString(), ref record._unique);
                Datasheet.Parse(stream.NextString(), ref record._quest);
    }
}
