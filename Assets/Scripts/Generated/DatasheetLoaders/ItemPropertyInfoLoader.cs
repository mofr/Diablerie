
// It's generated file. DO NOT MODIFY IT!
class ItemPropertyInfoLoader : Datasheet.Loader<ItemPropertyInfo>
{
    private ItemPropertyInfoBlockLoader itempropertyinfoblockloader = new ItemPropertyInfoBlockLoader();

    public void LoadRecord(ref ItemPropertyInfo record, Datasheet.Stream stream)
    {
                stream.Read(ref record.code);
                stream.Read(ref record._done);
                record._blocks = new ItemPropertyInfo.Block[7];
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[0], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[1], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[2], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[3], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[4], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[5], stream);
                    itempropertyinfoblockloader.LoadRecord(ref record._blocks[6], stream);
                stream.Read(ref record._desc);
                stream.Read(ref record._param);
                stream.Read(ref record._min);
                stream.Read(ref record._max);
                stream.Read(ref record._notes);
                stream.Read(ref record._eol);
    }
}
