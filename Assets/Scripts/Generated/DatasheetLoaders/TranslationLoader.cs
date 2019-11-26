
// It's generated file. DO NOT MODIFY IT!
class TranslationLoader : Datasheet.Loader<Translation>
{

    public void LoadRecord(ref Translation record, Datasheet.Stream stream)
    {
                stream.Read(ref record.key);
                stream.Read(ref record.value);
    }
}
