
// It's generated file. DO NOT MODIFY IT!
class ItemSetPropLoader : Datasheet.Loader<ItemSet.Prop>
{

    public void LoadRecord(ref ItemSet.Prop record, Datasheet.Stream stream)
    {
                stream.Read(ref record.prop);
                stream.Read(ref record.param);
                stream.Read(ref record.min);
                stream.Read(ref record.max);
    }
}
