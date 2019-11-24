
// It's generated file. DO NOT MODIFY IT!
class ItemTypeLoader : Datasheet.Loader<ItemType>
{

    public void LoadRecord(ref ItemType record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.name);
                Datasheet.Parse(stream.NextString(), ref record.code);
                Datasheet.Parse(stream.NextString(), ref record._equiv1);
                Datasheet.Parse(stream.NextString(), ref record._equiv2);
                Datasheet.Parse(stream.NextString(), ref record.repair);
                Datasheet.Parse(stream.NextString(), ref record.body);
                Datasheet.Parse(stream.NextString(), ref record.bodyLoc1Code);
                Datasheet.Parse(stream.NextString(), ref record.bodyLoc2Code);
                Datasheet.Parse(stream.NextString(), ref record.shoots);
                Datasheet.Parse(stream.NextString(), ref record.quiver);
                Datasheet.Parse(stream.NextString(), ref record.throwable);
                Datasheet.Parse(stream.NextString(), ref record.reload);
                Datasheet.Parse(stream.NextString(), ref record.reEquip);
                Datasheet.Parse(stream.NextString(), ref record.autoStack);
                Datasheet.Parse(stream.NextString(), ref record.alwaysMagic);
                Datasheet.Parse(stream.NextString(), ref record.canBeRare);
                Datasheet.Parse(stream.NextString(), ref record.alwaysNormal);
                Datasheet.Parse(stream.NextString(), ref record.charm);
                Datasheet.Parse(stream.NextString(), ref record.gem);
                Datasheet.Parse(stream.NextString(), ref record.beltable);
                Datasheet.Parse(stream.NextString(), ref record.maxSock1);
                Datasheet.Parse(stream.NextString(), ref record.maxSock25);
                Datasheet.Parse(stream.NextString(), ref record.maxSock40);
                Datasheet.Parse(stream.NextString(), ref record.treasureClass);
                Datasheet.Parse(stream.NextString(), ref record.rarity);
                Datasheet.Parse(stream.NextString(), ref record.staffMods);
                Datasheet.Parse(stream.NextString(), ref record.costFormula);
                Datasheet.Parse(stream.NextString(), ref record.classCode);
                Datasheet.Parse(stream.NextString(), ref record.varInvGfx);
                record.invGfx = new string[6];
                    Datasheet.Parse(stream.NextString(), ref record.invGfx[0]);
                    Datasheet.Parse(stream.NextString(), ref record.invGfx[1]);
                    Datasheet.Parse(stream.NextString(), ref record.invGfx[2]);
                    Datasheet.Parse(stream.NextString(), ref record.invGfx[3]);
                    Datasheet.Parse(stream.NextString(), ref record.invGfx[4]);
                    Datasheet.Parse(stream.NextString(), ref record.invGfx[5]);
                Datasheet.Parse(stream.NextString(), ref record.storePage);
                Datasheet.Parse(stream.NextString(), ref record.eol);
    }
}
