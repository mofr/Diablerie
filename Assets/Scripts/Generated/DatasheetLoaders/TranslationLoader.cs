
// It's generated file. DO NOT MODIFY IT!
class TranslationLoader : Datasheet.Loader<Translation>
{

    public void LoadRecord(ref Translation record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.key);
                Datasheet.Parse(stream.NextString(), ref record.value);
    }
}
