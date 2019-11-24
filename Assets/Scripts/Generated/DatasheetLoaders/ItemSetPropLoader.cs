
// It's generated file. DO NOT MODIFY IT!
class ItemSetPropLoader : Datasheet.Loader<ItemSet.Prop>
{

    public void LoadRecord(ref ItemSet.Prop record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.prop);
                Datasheet.Parse(stream.NextString(), ref record.param);
                Datasheet.Parse(stream.NextString(), ref record.min);
                Datasheet.Parse(stream.NextString(), ref record.max);
    }
}
