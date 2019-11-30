
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class ItemSetLoader : Datasheet.Loader<ItemSet>
{
    private ItemSetPropLoader itemsetproploader = new ItemSetPropLoader();

    public void LoadRecord(ref ItemSet record, Datasheet.Stream stream)
    {
                stream.Read(ref record.id);
                stream.Read(ref record.nameStr);
                stream.Read(ref record.version);
                stream.Read(ref record.level);
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
                stream.Read(ref record.eol);
    }
}
