
// It's generated file. DO NOT MODIFY IT!
class BodyLocLoader : Datasheet.Loader<BodyLoc>
{
    public void LoadRecord(ref BodyLoc record, string[] values)
    {
        int index = 0;
                Datasheet.Parse(values[index++], ref record.name);
                Datasheet.Parse(values[index++], ref record.code);
    }
}
