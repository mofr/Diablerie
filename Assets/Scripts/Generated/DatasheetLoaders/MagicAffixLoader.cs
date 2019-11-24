
// It's generated file. DO NOT MODIFY IT!
class MagicAffixLoader : Datasheet.Loader<MagicAffix>
{
    private MagicAffixModLoader magicaffixmodloader = new MagicAffixModLoader();

    public void LoadRecord(ref MagicAffix record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.nameStr);
                Datasheet.Parse(stream.NextString(), ref record.version);
                Datasheet.Parse(stream.NextString(), ref record.spawnable);
                Datasheet.Parse(stream.NextString(), ref record.rare);
                Datasheet.Parse(stream.NextString(), ref record.level);
                Datasheet.Parse(stream.NextString(), ref record.maxlevel);
                Datasheet.Parse(stream.NextString(), ref record.levelReq);
                Datasheet.Parse(stream.NextString(), ref record.classSpecific);
                Datasheet.Parse(stream.NextString(), ref record.classCode);
                Datasheet.Parse(stream.NextString(), ref record.classlevelreq);
                Datasheet.Parse(stream.NextString(), ref record.frequency);
                Datasheet.Parse(stream.NextString(), ref record.group);
                record.mods = new MagicAffix.Mod[3];
                    magicaffixmodloader.LoadRecord(ref record.mods[0], stream);
                    magicaffixmodloader.LoadRecord(ref record.mods[1], stream);
                    magicaffixmodloader.LoadRecord(ref record.mods[2], stream);
                Datasheet.Parse(stream.NextString(), ref record.transform);
                Datasheet.Parse(stream.NextString(), ref record.transformcolor);
                record.itemTypes = new string[7];
                    Datasheet.Parse(stream.NextString(), ref record.itemTypes[0]);
                    Datasheet.Parse(stream.NextString(), ref record.itemTypes[1]);
                    Datasheet.Parse(stream.NextString(), ref record.itemTypes[2]);
                    Datasheet.Parse(stream.NextString(), ref record.itemTypes[3]);
                    Datasheet.Parse(stream.NextString(), ref record.itemTypes[4]);
                    Datasheet.Parse(stream.NextString(), ref record.itemTypes[5]);
                    Datasheet.Parse(stream.NextString(), ref record.itemTypes[6]);
                record.excludeItemTypes = new string[5];
                    Datasheet.Parse(stream.NextString(), ref record.excludeItemTypes[0]);
                    Datasheet.Parse(stream.NextString(), ref record.excludeItemTypes[1]);
                    Datasheet.Parse(stream.NextString(), ref record.excludeItemTypes[2]);
                    Datasheet.Parse(stream.NextString(), ref record.excludeItemTypes[3]);
                    Datasheet.Parse(stream.NextString(), ref record.excludeItemTypes[4]);
                Datasheet.Parse(stream.NextString(), ref record.divide);
                Datasheet.Parse(stream.NextString(), ref record.multiply);
                Datasheet.Parse(stream.NextString(), ref record.add);
    }
}
