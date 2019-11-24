
// It's generated file. DO NOT MODIFY IT!
class ItemPropertyInfoLoader : Datasheet.Loader<ItemPropertyInfo>
{
    private ItemPropertyInfoBlockLoader itempropertyinfoblockloader = new ItemPropertyInfoBlockLoader();

    public void LoadRecord(ref ItemPropertyInfo record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.code);
                Datasheet.Parse(stream.NextString(), ref record._done);
                record._blocks = new ItemPropertyInfo.Block[7];
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[0], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[1], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[2], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[3], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[4], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[5], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[6], stream);
                Datasheet.Parse(stream.NextString(), ref record._desc);
                Datasheet.Parse(stream.NextString(), ref record._param);
                Datasheet.Parse(stream.NextString(), ref record._min);
                Datasheet.Parse(stream.NextString(), ref record._max);
                Datasheet.Parse(stream.NextString(), ref record._notes);
                Datasheet.Parse(stream.NextString(), ref record._eol);
    }
}
