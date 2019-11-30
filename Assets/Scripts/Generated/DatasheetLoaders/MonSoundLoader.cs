
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class MonSoundLoader : Datasheet.Loader<MonSound>
{

    public void LoadRecord(ref MonSound record, Datasheet.Stream stream)
    {
                stream.Read(ref record.id);
                stream.Read(ref record._attack1);
                stream.Read(ref record._weapon1);
                stream.Read(ref record._attack1Delay);
                stream.Read(ref record._weapon1Delay);
                stream.Read(ref record.attack1Prob);
                stream.Read(ref record._weapon1Volume);
                stream.Read(ref record._attack2);
                stream.Read(ref record._weapon2);
                stream.Read(ref record._attack2Delay);
                stream.Read(ref record._weapon2Delay);
                stream.Read(ref record.attack2Prob);
                stream.Read(ref record._weapon2Volume);
                stream.Read(ref record._hitSound);
                stream.Read(ref record._deathSound);
                stream.Read(ref record._hitDelay);
                stream.Read(ref record._deathDelay);
                stream.Read(ref record._skill1);
                stream.Read(ref record._skill2);
                stream.Read(ref record._skill3);
                stream.Read(ref record._skill4);
                stream.Read(ref record._footstep);
                stream.Read(ref record._footstepLayer);
                stream.Read(ref record._fsCnt);
                stream.Read(ref record._fsOff);
                stream.Read(ref record._fsPrb);
                stream.Read(ref record._neutral);
                stream.Read(ref record._neuTime);
                stream.Read(ref record._init);
                stream.Read(ref record._taunt);
                stream.Read(ref record._flee);
                stream.Read(ref record._cvtMo1);
                stream.Read(ref record._cvtSk1);
                stream.Read(ref record._cvtTgt1);
                stream.Read(ref record._cvtMo2);
                stream.Read(ref record._cvtSk2);
                stream.Read(ref record._cvtTgt2);
                stream.Read(ref record._cvtMo3);
                stream.Read(ref record._cvtSk3);
                stream.Read(ref record._cvtTgt3);
                stream.Read(ref record.eol);
    }
}
