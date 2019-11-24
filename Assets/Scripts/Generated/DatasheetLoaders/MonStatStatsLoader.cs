
// It's generated file. DO NOT MODIFY IT!
class MonStatStatsLoader : Datasheet.Loader<MonStat.Stats>
{

    public void LoadRecord(ref MonStat.Stats record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.minHP);
                Datasheet.Parse(stream.NextString(), ref record.maxHP);
                Datasheet.Parse(stream.NextString(), ref record.armorClass);
                Datasheet.Parse(stream.NextString(), ref record.exp);
                Datasheet.Parse(stream.NextString(), ref record.A1MinDamage);
                Datasheet.Parse(stream.NextString(), ref record.A1MaxDamage);
                Datasheet.Parse(stream.NextString(), ref record.A1ToHit);
                Datasheet.Parse(stream.NextString(), ref record.A2MinDamage);
                Datasheet.Parse(stream.NextString(), ref record.A2MaxDamage);
                Datasheet.Parse(stream.NextString(), ref record.A2ToHit);
                Datasheet.Parse(stream.NextString(), ref record.S1MinDamage);
                Datasheet.Parse(stream.NextString(), ref record.S1MaxDamage);
                Datasheet.Parse(stream.NextString(), ref record.S1ToHit);
    }
}
