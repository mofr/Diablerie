
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class MonLvlLoader : Datasheet.Loader<MonLvl>
{

    public void LoadRecord(ref MonLvl record, DatasheetStream stream)
    {
                stream.Read(ref record.level);
                record.armor = new int[6];
                    stream.Read(ref record.armor[0]);
                    stream.Read(ref record.armor[1]);
                    stream.Read(ref record.armor[2]);
                    stream.Read(ref record.armor[3]);
                    stream.Read(ref record.armor[4]);
                    stream.Read(ref record.armor[5]);
                record.toHit = new int[6];
                    stream.Read(ref record.toHit[0]);
                    stream.Read(ref record.toHit[1]);
                    stream.Read(ref record.toHit[2]);
                    stream.Read(ref record.toHit[3]);
                    stream.Read(ref record.toHit[4]);
                    stream.Read(ref record.toHit[5]);
                record.hp = new int[6];
                    stream.Read(ref record.hp[0]);
                    stream.Read(ref record.hp[1]);
                    stream.Read(ref record.hp[2]);
                    stream.Read(ref record.hp[3]);
                    stream.Read(ref record.hp[4]);
                    stream.Read(ref record.hp[5]);
                record.damage = new int[6];
                    stream.Read(ref record.damage[0]);
                    stream.Read(ref record.damage[1]);
                    stream.Read(ref record.damage[2]);
                    stream.Read(ref record.damage[3]);
                    stream.Read(ref record.damage[4]);
                    stream.Read(ref record.damage[5]);
                record.experience = new int[6];
                    stream.Read(ref record.experience[0]);
                    stream.Read(ref record.experience[1]);
                    stream.Read(ref record.experience[2]);
                    stream.Read(ref record.experience[3]);
                    stream.Read(ref record.experience[4]);
                    stream.Read(ref record.experience[5]);
    }
}
