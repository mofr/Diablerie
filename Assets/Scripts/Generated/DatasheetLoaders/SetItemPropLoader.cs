
// It's generated file. DO NOT MODIFY IT!
class SetItemPropLoader : Datasheet.Loader<SetItem.Prop>
{

    public void LoadRecord(ref SetItem.Prop record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.prop);
                Datasheet.Parse(stream.NextString(), ref record.param);
                Datasheet.Parse(stream.NextString(), ref record.min);
                Datasheet.Parse(stream.NextString(), ref record.max);
    }
}
