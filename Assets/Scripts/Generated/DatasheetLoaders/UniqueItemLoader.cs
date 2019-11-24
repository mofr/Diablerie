
// It's generated file. DO NOT MODIFY IT!
class UniqueItemLoader : Datasheet.Loader<UniqueItem>
{
    private UniqueItemPropLoader uniqueitemproploader = new UniqueItemPropLoader();

    public void LoadRecord(ref UniqueItem record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.nameStr);
                Datasheet.Parse(stream.NextString(), ref record.version);
                Datasheet.Parse(stream.NextString(), ref record.enabled);
                Datasheet.Parse(stream.NextString(), ref record.ladder);
                Datasheet.Parse(stream.NextString(), ref record.rarity);
                Datasheet.Parse(stream.NextString(), ref record.noLimit);
                Datasheet.Parse(stream.NextString(), ref record.level);
                Datasheet.Parse(stream.NextString(), ref record.levelReq);
                Datasheet.Parse(stream.NextString(), ref record.code);
                Datasheet.Parse(stream.NextString(), ref record.type);
                Datasheet.Parse(stream.NextString(), ref record.uber);
                Datasheet.Parse(stream.NextString(), ref record.carry1);
                Datasheet.Parse(stream.NextString(), ref record.costMult);
                Datasheet.Parse(stream.NextString(), ref record.costAdd);
                Datasheet.Parse(stream.NextString(), ref record.chrTransform);
                Datasheet.Parse(stream.NextString(), ref record.invTransform);
                Datasheet.Parse(stream.NextString(), ref record.flippyFile);
                Datasheet.Parse(stream.NextString(), ref record.invFile);
                Datasheet.Parse(stream.NextString(), ref record._dropSound);
                Datasheet.Parse(stream.NextString(), ref record._dropSoundFrame);
                Datasheet.Parse(stream.NextString(), ref record._useSound);
                record.props = new UniqueItem.Prop[12];
                    uniqueitemproploader.LoadRecord(ref record.props[0], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[1], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[2], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[3], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[4], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[5], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[6], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[7], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[8], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[9], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[10], stream);
                    uniqueitemproploader.LoadRecord(ref record.props[11], stream);
                Datasheet.Parse(stream.NextString(), ref record.eol);
    }
}
