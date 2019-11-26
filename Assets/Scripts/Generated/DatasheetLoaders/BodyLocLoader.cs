
// It's generated file. DO NOT MODIFY IT!
class BodyLocLoader : Datasheet.Loader<BodyLoc>
{

    public void LoadRecord(ref BodyLoc record, Datasheet.Stream stream)
    {
                stream.Read(ref record.name);
                stream.Read(ref record.code);
    }
}
