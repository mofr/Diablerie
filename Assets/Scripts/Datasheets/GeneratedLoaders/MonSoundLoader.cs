
// It's generated file. DO NOT MODIFY IT!
class MonSoundLoader : Datasheet.Loader<MonSound>
{
    public void LoadRecord(ref MonSound record, string[] values)
    {
        int index = 0;
            Datasheet.Parse(values[index++], ref record.id);
            Datasheet.Parse(values[index++], ref record._attack1);
            Datasheet.Parse(values[index++], ref record._weapon1);
            Datasheet.Parse(values[index++], ref record._attack1Delay);
            Datasheet.Parse(values[index++], ref record._weapon1Delay);
            Datasheet.Parse(values[index++], ref record.attack1Prob);
            Datasheet.Parse(values[index++], ref record._weapon1Volume);
            Datasheet.Parse(values[index++], ref record._attack2);
            Datasheet.Parse(values[index++], ref record._weapon2);
            Datasheet.Parse(values[index++], ref record._attack2Delay);
            Datasheet.Parse(values[index++], ref record._weapon2Delay);
            Datasheet.Parse(values[index++], ref record.attack2Prob);
            Datasheet.Parse(values[index++], ref record._weapon2Volume);
            Datasheet.Parse(values[index++], ref record._hitSound);
            Datasheet.Parse(values[index++], ref record._deathSound);
            Datasheet.Parse(values[index++], ref record._hitDelay);
            Datasheet.Parse(values[index++], ref record._deathDelay);
            Datasheet.Parse(values[index++], ref record._skill1);
            Datasheet.Parse(values[index++], ref record._skill2);
            Datasheet.Parse(values[index++], ref record._skill3);
            Datasheet.Parse(values[index++], ref record._skill4);
            Datasheet.Parse(values[index++], ref record._footstep);
            Datasheet.Parse(values[index++], ref record._footstepLayer);
            Datasheet.Parse(values[index++], ref record._fsCnt);
            Datasheet.Parse(values[index++], ref record._fsOff);
            Datasheet.Parse(values[index++], ref record._fsPrb);
            Datasheet.Parse(values[index++], ref record._neutral);
            Datasheet.Parse(values[index++], ref record._neuTime);
            Datasheet.Parse(values[index++], ref record._init);
            Datasheet.Parse(values[index++], ref record._taunt);
            Datasheet.Parse(values[index++], ref record._flee);
            Datasheet.Parse(values[index++], ref record._cvtMo1);
            Datasheet.Parse(values[index++], ref record._cvtSk1);
            Datasheet.Parse(values[index++], ref record._cvtTgt1);
            Datasheet.Parse(values[index++], ref record._cvtMo2);
            Datasheet.Parse(values[index++], ref record._cvtSk2);
            Datasheet.Parse(values[index++], ref record._cvtTgt2);
            Datasheet.Parse(values[index++], ref record._cvtMo3);
            Datasheet.Parse(values[index++], ref record._cvtSk3);
            Datasheet.Parse(values[index++], ref record._cvtTgt3);
            Datasheet.Parse(values[index++], ref record.eol);
    }
}
