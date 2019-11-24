
// It's generated file. DO NOT MODIFY IT!
class BodyLocLoader : Datasheet.Loader<BodyLoc>
{
    public void LoadRecord(ref BodyLoc record, string[] values)
    {
        int index = 0;
            record.name = values[index++];
            record.code = values[index++];
    }
}
