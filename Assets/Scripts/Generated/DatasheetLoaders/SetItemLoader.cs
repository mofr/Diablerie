
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class SetItemLoader : Datasheet.Loader<SetItem>
{
    private SetItemPropLoader setitemproploader = new SetItemPropLoader();

    public void LoadRecord(ref SetItem record, DatasheetStream stream)
    {
                stream.Read(ref record.id);
                stream.Read(ref record.setId);
                stream.Read(ref record.itemCode);
                stream.Read(ref record._item);
                stream.Read(ref record.rarity);
                stream.Read(ref record.level);
                stream.Read(ref record.levelReq);
                stream.Read(ref record.chrTransform);
                stream.Read(ref record.invTransform);
                stream.Read(ref record.invFile);
                stream.Read(ref record.flippyFile);
                stream.Read(ref record._dropSound);
                stream.Read(ref record._dropSoundFrame);
                stream.Read(ref record._useSound);
                stream.Read(ref record.costMult);
                stream.Read(ref record.costAdd);
                stream.Read(ref record.addFunc);
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
                stream.Read(ref record.eol);
    }
}
