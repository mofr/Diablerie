
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class MagicAffixLoader : Datasheet.Loader<MagicAffix>
{
    private MagicAffixModLoader magicaffixmodloader = new MagicAffixModLoader();

    public void LoadRecord(ref MagicAffix record, Datasheet.Stream stream)
    {
                stream.Read(ref record.nameStr);
                stream.Read(ref record.version);
                stream.Read(ref record.spawnable);
                stream.Read(ref record.rare);
                stream.Read(ref record.level);
                stream.Read(ref record.maxlevel);
                stream.Read(ref record.levelReq);
                stream.Read(ref record.classSpecific);
                stream.Read(ref record.classCode);
                stream.Read(ref record.classlevelreq);
                stream.Read(ref record.frequency);
                stream.Read(ref record.group);
                record.mods = new MagicAffix.Mod[3];
                    magicaffixmodloader.LoadRecord(ref record.mods[0], stream);
                    magicaffixmodloader.LoadRecord(ref record.mods[1], stream);
                    magicaffixmodloader.LoadRecord(ref record.mods[2], stream);
                stream.Read(ref record.transform);
                stream.Read(ref record.transformcolor);
                record.itemTypes = new string[7];
                    stream.Read(ref record.itemTypes[0]);
                    stream.Read(ref record.itemTypes[1]);
                    stream.Read(ref record.itemTypes[2]);
                    stream.Read(ref record.itemTypes[3]);
                    stream.Read(ref record.itemTypes[4]);
                    stream.Read(ref record.itemTypes[5]);
                    stream.Read(ref record.itemTypes[6]);
                record.excludeItemTypes = new string[5];
                    stream.Read(ref record.excludeItemTypes[0]);
                    stream.Read(ref record.excludeItemTypes[1]);
                    stream.Read(ref record.excludeItemTypes[2]);
                    stream.Read(ref record.excludeItemTypes[3]);
                    stream.Read(ref record.excludeItemTypes[4]);
                stream.Read(ref record.divide);
                stream.Read(ref record.multiply);
                stream.Read(ref record.add);
    }
}
