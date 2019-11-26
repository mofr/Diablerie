
// It's generated file. DO NOT MODIFY IT!
class MonPresetLoader : Datasheet.Loader<MonPreset>
{

    public void LoadRecord(ref MonPreset record, Datasheet.Stream stream)
    {
                stream.Read(ref record.act);
                stream.Read(ref record.place);
    }
}
