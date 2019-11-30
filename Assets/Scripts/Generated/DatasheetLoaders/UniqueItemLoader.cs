
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class UniqueItemLoader : Datasheet.Loader<UniqueItem>
{
    private UniqueItemPropLoader uniqueitemproploader = new UniqueItemPropLoader();

    public void LoadRecord(ref UniqueItem record, Datasheet.Stream stream)
    {
                stream.Read(ref record.nameStr);
                stream.Read(ref record.version);
                stream.Read(ref record.enabled);
                stream.Read(ref record.ladder);
                stream.Read(ref record.rarity);
                stream.Read(ref record.noLimit);
                stream.Read(ref record.level);
                stream.Read(ref record.levelReq);
                stream.Read(ref record.code);
                stream.Read(ref record.type);
                stream.Read(ref record.uber);
                stream.Read(ref record.carry1);
                stream.Read(ref record.costMult);
                stream.Read(ref record.costAdd);
                stream.Read(ref record.chrTransform);
                stream.Read(ref record.invTransform);
                stream.Read(ref record.flippyFile);
                stream.Read(ref record.invFile);
                stream.Read(ref record._dropSound);
                stream.Read(ref record._dropSoundFrame);
                stream.Read(ref record._useSound);
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
                stream.Read(ref record.eol);
    }
}
