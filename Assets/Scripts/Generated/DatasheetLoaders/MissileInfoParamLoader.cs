
// It's generated file. DO NOT MODIFY IT!
class MissileInfoParamLoader : Datasheet.Loader<MissileInfo.Param>
{

    public void LoadRecord(ref MissileInfo.Param record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.value);
                Datasheet.Parse(stream.NextString(), ref record.description);
    }
}
