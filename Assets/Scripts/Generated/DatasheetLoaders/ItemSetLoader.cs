
// It's generated file. DO NOT MODIFY IT!
class ItemSetLoader : Datasheet.Loader<ItemSet>
{
    private ItemSetPropLoader itemsetproploader = new ItemSetPropLoader();

    public void LoadRecord(ref ItemSet record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.id);
                Datasheet.Parse(stream.NextString(), ref record.nameStr);
                Datasheet.Parse(stream.NextString(), ref record.version);
                Datasheet.Parse(stream.NextString(), ref record.level);
                record.props = new ItemSet.Prop[8];
                    itemsetproploader.LoadRecord(ref record.props[0], stream);
                    itemsetproploader.LoadRecord(ref record.props[1], stream);
                    itemsetproploader.LoadRecord(ref record.props[2], stream);
                    itemsetproploader.LoadRecord(ref record.props[3], stream);
                    itemsetproploader.LoadRecord(ref record.props[4], stream);
                    itemsetproploader.LoadRecord(ref record.props[5], stream);
                    itemsetproploader.LoadRecord(ref record.props[6], stream);
                    itemsetproploader.LoadRecord(ref record.props[7], stream);
                record.fullProps = new ItemSet.Prop[8];
                    itemsetproploader.LoadRecord(ref record.fullProps[0], stream);
                    itemsetproploader.LoadRecord(ref record.fullProps[1], stream);
                    itemsetproploader.LoadRecord(ref record.fullProps[2], stream);
                    itemsetproploader.LoadRecord(ref record.fullProps[3], stream);
                    itemsetproploader.LoadRecord(ref record.fullProps[4], stream);
                    itemsetproploader.LoadRecord(ref record.fullProps[5], stream);
                    itemsetproploader.LoadRecord(ref record.fullProps[6], stream);
                    itemsetproploader.LoadRecord(ref record.fullProps[7], stream);
                Datasheet.Parse(stream.NextString(), ref record.eol);
    }
}
