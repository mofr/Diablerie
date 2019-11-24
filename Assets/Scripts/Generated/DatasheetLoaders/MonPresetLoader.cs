
// It's generated file. DO NOT MODIFY IT!
class MonPresetLoader : Datasheet.Loader<MonPreset>
{

    public void LoadRecord(ref MonPreset record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.act);
                Datasheet.Parse(stream.NextString(), ref record.place);
    }
}
