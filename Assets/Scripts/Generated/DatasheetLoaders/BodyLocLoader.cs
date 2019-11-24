
// It's generated file. DO NOT MODIFY IT!
class BodyLocLoader : Datasheet.Loader<BodyLoc>
{

    public void LoadRecord(ref BodyLoc record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.name);
                Datasheet.Parse(stream.NextString(), ref record.code);
    }
}
