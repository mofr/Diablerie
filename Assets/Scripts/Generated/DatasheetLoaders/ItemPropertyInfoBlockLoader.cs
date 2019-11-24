
// It's generated file. DO NOT MODIFY IT!
class ItemPropertyInfoBlockLoader : Datasheet.Loader<ItemPropertyInfo.Block>
{

    public void LoadRecord(ref ItemPropertyInfo.Block record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.set);
                Datasheet.Parse(stream.NextString(), ref record.value);
                Datasheet.Parse(stream.NextString(), ref record.func);
                Datasheet.Parse(stream.NextString(), ref record.statId);
    }
}
