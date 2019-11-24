
// It's generated file. DO NOT MODIFY IT!
class MonLvlLoader : Datasheet.Loader<MonLvl>
{

    public void LoadRecord(ref MonLvl record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.level);
                record.armor = new int[6];
                    Datasheet.Parse(stream.NextString(), ref record.armor[0]);
                    Datasheet.Parse(stream.NextString(), ref record.armor[1]);
                    Datasheet.Parse(stream.NextString(), ref record.armor[2]);
                    Datasheet.Parse(stream.NextString(), ref record.armor[3]);
                    Datasheet.Parse(stream.NextString(), ref record.armor[4]);
                    Datasheet.Parse(stream.NextString(), ref record.armor[5]);
                record.toHit = new int[6];
                    Datasheet.Parse(stream.NextString(), ref record.toHit[0]);
                    Datasheet.Parse(stream.NextString(), ref record.toHit[1]);
                    Datasheet.Parse(stream.NextString(), ref record.toHit[2]);
                    Datasheet.Parse(stream.NextString(), ref record.toHit[3]);
                    Datasheet.Parse(stream.NextString(), ref record.toHit[4]);
                    Datasheet.Parse(stream.NextString(), ref record.toHit[5]);
                record.hp = new int[6];
                    Datasheet.Parse(stream.NextString(), ref record.hp[0]);
                    Datasheet.Parse(stream.NextString(), ref record.hp[1]);
                    Datasheet.Parse(stream.NextString(), ref record.hp[2]);
                    Datasheet.Parse(stream.NextString(), ref record.hp[3]);
                    Datasheet.Parse(stream.NextString(), ref record.hp[4]);
                    Datasheet.Parse(stream.NextString(), ref record.hp[5]);
                record.damage = new int[6];
                    Datasheet.Parse(stream.NextString(), ref record.damage[0]);
                    Datasheet.Parse(stream.NextString(), ref record.damage[1]);
                    Datasheet.Parse(stream.NextString(), ref record.damage[2]);
                    Datasheet.Parse(stream.NextString(), ref record.damage[3]);
                    Datasheet.Parse(stream.NextString(), ref record.damage[4]);
                    Datasheet.Parse(stream.NextString(), ref record.damage[5]);
                record.experience = new int[6];
                    Datasheet.Parse(stream.NextString(), ref record.experience[0]);
                    Datasheet.Parse(stream.NextString(), ref record.experience[1]);
                    Datasheet.Parse(stream.NextString(), ref record.experience[2]);
                    Datasheet.Parse(stream.NextString(), ref record.experience[3]);
                    Datasheet.Parse(stream.NextString(), ref record.experience[4]);
                    Datasheet.Parse(stream.NextString(), ref record.experience[5]);
    }
}
