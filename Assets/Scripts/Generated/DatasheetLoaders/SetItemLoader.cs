
// It's generated file. DO NOT MODIFY IT!
class SetItemLoader : Datasheet.Loader<SetItem>
{
    private SetItemPropLoader setitemproploader = new SetItemPropLoader();

    public void LoadRecord(ref SetItem record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.id);
                Datasheet.Parse(stream.NextString(), ref record.setId);
                Datasheet.Parse(stream.NextString(), ref record.itemCode);
                Datasheet.Parse(stream.NextString(), ref record._item);
                Datasheet.Parse(stream.NextString(), ref record.rarity);
                Datasheet.Parse(stream.NextString(), ref record.level);
                Datasheet.Parse(stream.NextString(), ref record.levelReq);
                Datasheet.Parse(stream.NextString(), ref record.chrTransform);
                Datasheet.Parse(stream.NextString(), ref record.invTransform);
                Datasheet.Parse(stream.NextString(), ref record.invFile);
                Datasheet.Parse(stream.NextString(), ref record.flippyFile);
                Datasheet.Parse(stream.NextString(), ref record._dropSound);
                Datasheet.Parse(stream.NextString(), ref record._dropSoundFrame);
                Datasheet.Parse(stream.NextString(), ref record._useSound);
                Datasheet.Parse(stream.NextString(), ref record.costMult);
                Datasheet.Parse(stream.NextString(), ref record.costAdd);
                Datasheet.Parse(stream.NextString(), ref record.addFunc);
                record.props = new SetItem.Prop[9];
                    setitemproploader.LoadRecord(ref record.props[0], stream);
                    setitemproploader.LoadRecord(ref record.props[1], stream);
                    setitemproploader.LoadRecord(ref record.props[2], stream);
                    setitemproploader.LoadRecord(ref record.props[3], stream);
                    setitemproploader.LoadRecord(ref record.props[4], stream);
                    setitemproploader.LoadRecord(ref record.props[5], stream);
                    setitemproploader.LoadRecord(ref record.props[6], stream);
                    setitemproploader.LoadRecord(ref record.props[7], stream);
                    setitemproploader.LoadRecord(ref record.props[8], stream);
                record.additionalProps = new SetItem.Prop[10];
                    setitemproploader.LoadRecord(ref record.additionalProps[0], stream);
                    setitemproploader.LoadRecord(ref record.additionalProps[1], stream);
                    setitemproploader.LoadRecord(ref record.additionalProps[2], stream);
                    setitemproploader.LoadRecord(ref record.additionalProps[3], stream);
                    setitemproploader.LoadRecord(ref record.additionalProps[4], stream);
                    setitemproploader.LoadRecord(ref record.additionalProps[5], stream);
                    setitemproploader.LoadRecord(ref record.additionalProps[6], stream);
                    setitemproploader.LoadRecord(ref record.additionalProps[7], stream);
                    setitemproploader.LoadRecord(ref record.additionalProps[8], stream);
                    setitemproploader.LoadRecord(ref record.additionalProps[9], stream);
                Datasheet.Parse(stream.NextString(), ref record.eol);
    }
}
