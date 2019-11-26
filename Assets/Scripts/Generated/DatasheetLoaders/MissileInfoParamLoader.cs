
// It's generated file. DO NOT MODIFY IT!
class MissileInfoParamLoader : Datasheet.Loader<MissileInfo.Param>
{

    public void LoadRecord(ref MissileInfo.Param record, Datasheet.Stream stream)
    {
                stream.Read(ref record.value);
                stream.Read(ref record.description);
    }
}
