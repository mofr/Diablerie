
// It's generated file. DO NOT MODIFY IT!
class MonSoundLoader : Datasheet.Loader<MonSound>
{

    public void LoadRecord(ref MonSound record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.id);
                Datasheet.Parse(stream.NextString(), ref record._attack1);
                Datasheet.Parse(stream.NextString(), ref record._weapon1);
                Datasheet.Parse(stream.NextString(), ref record._attack1Delay);
                Datasheet.Parse(stream.NextString(), ref record._weapon1Delay);
                Datasheet.Parse(stream.NextString(), ref record.attack1Prob);
                Datasheet.Parse(stream.NextString(), ref record._weapon1Volume);
                Datasheet.Parse(stream.NextString(), ref record._attack2);
                Datasheet.Parse(stream.NextString(), ref record._weapon2);
                Datasheet.Parse(stream.NextString(), ref record._attack2Delay);
                Datasheet.Parse(stream.NextString(), ref record._weapon2Delay);
                Datasheet.Parse(stream.NextString(), ref record.attack2Prob);
                Datasheet.Parse(stream.NextString(), ref record._weapon2Volume);
                Datasheet.Parse(stream.NextString(), ref record._hitSound);
                Datasheet.Parse(stream.NextString(), ref record._deathSound);
                Datasheet.Parse(stream.NextString(), ref record._hitDelay);
                Datasheet.Parse(stream.NextString(), ref record._deathDelay);
                Datasheet.Parse(stream.NextString(), ref record._skill1);
                Datasheet.Parse(stream.NextString(), ref record._skill2);
                Datasheet.Parse(stream.NextString(), ref record._skill3);
                Datasheet.Parse(stream.NextString(), ref record._skill4);
                Datasheet.Parse(stream.NextString(), ref record._footstep);
                Datasheet.Parse(stream.NextString(), ref record._footstepLayer);
                Datasheet.Parse(stream.NextString(), ref record._fsCnt);
                Datasheet.Parse(stream.NextString(), ref record._fsOff);
                Datasheet.Parse(stream.NextString(), ref record._fsPrb);
                Datasheet.Parse(stream.NextString(), ref record._neutral);
                Datasheet.Parse(stream.NextString(), ref record._neuTime);
                Datasheet.Parse(stream.NextString(), ref record._init);
                Datasheet.Parse(stream.NextString(), ref record._taunt);
                Datasheet.Parse(stream.NextString(), ref record._flee);
                Datasheet.Parse(stream.NextString(), ref record._cvtMo1);
                Datasheet.Parse(stream.NextString(), ref record._cvtSk1);
                Datasheet.Parse(stream.NextString(), ref record._cvtTgt1);
                Datasheet.Parse(stream.NextString(), ref record._cvtMo2);
                Datasheet.Parse(stream.NextString(), ref record._cvtSk2);
                Datasheet.Parse(stream.NextString(), ref record._cvtTgt2);
                Datasheet.Parse(stream.NextString(), ref record._cvtMo3);
                Datasheet.Parse(stream.NextString(), ref record._cvtSk3);
                Datasheet.Parse(stream.NextString(), ref record._cvtTgt3);
                Datasheet.Parse(stream.NextString(), ref record.eol);
    }
}
