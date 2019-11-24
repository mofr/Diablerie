
// It's generated file. DO NOT MODIFY IT!
class ExpTableLoader : Datasheet.Loader<ExpTable>
{

    public void LoadRecord(ref ExpTable record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.maxLevel);
                Datasheet.Parse(stream.NextString(), ref record.amazon);
                Datasheet.Parse(stream.NextString(), ref record.sorceress);
                Datasheet.Parse(stream.NextString(), ref record.necromancer);
                Datasheet.Parse(stream.NextString(), ref record.paladin);
                Datasheet.Parse(stream.NextString(), ref record.barbarian);
                Datasheet.Parse(stream.NextString(), ref record.druid);
                Datasheet.Parse(stream.NextString(), ref record.assassin);
    }
}
