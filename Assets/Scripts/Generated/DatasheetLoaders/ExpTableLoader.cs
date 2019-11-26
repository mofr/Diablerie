
// It's generated file. DO NOT MODIFY IT!
class ExpTableLoader : Datasheet.Loader<ExpTable>
{

    public void LoadRecord(ref ExpTable record, Datasheet.Stream stream)
    {
                stream.Read(ref record.maxLevel);
                stream.Read(ref record.amazon);
                stream.Read(ref record.sorceress);
                stream.Read(ref record.necromancer);
                stream.Read(ref record.paladin);
                stream.Read(ref record.barbarian);
                stream.Read(ref record.druid);
                stream.Read(ref record.assassin);
    }
}
