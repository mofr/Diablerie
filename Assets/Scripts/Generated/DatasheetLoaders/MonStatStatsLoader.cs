
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class MonStatStatsLoader : Datasheet.Loader<MonStat.Stats>
{

    public void LoadRecord(ref MonStat.Stats record, Datasheet.Stream stream)
    {
                stream.Read(ref record.minHP);
                stream.Read(ref record.maxHP);
                stream.Read(ref record.armorClass);
                stream.Read(ref record.exp);
                stream.Read(ref record.A1MinDamage);
                stream.Read(ref record.A1MaxDamage);
                stream.Read(ref record.A1ToHit);
                stream.Read(ref record.A2MinDamage);
                stream.Read(ref record.A2MaxDamage);
                stream.Read(ref record.A2ToHit);
                stream.Read(ref record.S1MinDamage);
                stream.Read(ref record.S1MaxDamage);
                stream.Read(ref record.S1ToHit);
    }
}
