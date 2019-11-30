
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class ItemTypeLoader : Datasheet.Loader<ItemType>
{

    public void LoadRecord(ref ItemType record, Datasheet.Stream stream)
    {
                stream.Read(ref record.name);
                stream.Read(ref record.code);
                stream.Read(ref record._equiv1);
                stream.Read(ref record._equiv2);
                stream.Read(ref record.repair);
                stream.Read(ref record.body);
                stream.Read(ref record.bodyLoc1Code);
                stream.Read(ref record.bodyLoc2Code);
                stream.Read(ref record.shoots);
                stream.Read(ref record.quiver);
                stream.Read(ref record.throwable);
                stream.Read(ref record.reload);
                stream.Read(ref record.reEquip);
                stream.Read(ref record.autoStack);
                stream.Read(ref record.alwaysMagic);
                stream.Read(ref record.canBeRare);
                stream.Read(ref record.alwaysNormal);
                stream.Read(ref record.charm);
                stream.Read(ref record.gem);
                stream.Read(ref record.beltable);
                stream.Read(ref record.maxSock1);
                stream.Read(ref record.maxSock25);
                stream.Read(ref record.maxSock40);
                stream.Read(ref record.treasureClass);
                stream.Read(ref record.rarity);
                stream.Read(ref record.staffMods);
                stream.Read(ref record.costFormula);
                stream.Read(ref record.classCode);
                stream.Read(ref record.varInvGfx);
                record.invGfx = new string[6];
                    stream.Read(ref record.invGfx[0]);
                    stream.Read(ref record.invGfx[1]);
                    stream.Read(ref record.invGfx[2]);
                    stream.Read(ref record.invGfx[3]);
                    stream.Read(ref record.invGfx[4]);
                    stream.Read(ref record.invGfx[5]);
                stream.Read(ref record.storePage);
                stream.Read(ref record.eol);
    }
}
