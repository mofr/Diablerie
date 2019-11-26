
// It's generated file. DO NOT MODIFY IT!
class SetItemPropLoader : Datasheet.Loader<SetItem.Prop>
{

    public void LoadRecord(ref SetItem.Prop record, Datasheet.Stream stream)
    {
                stream.Read(ref record.prop);
                stream.Read(ref record.param);
                stream.Read(ref record.min);
                stream.Read(ref record.max);
    }
}
