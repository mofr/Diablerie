
// It's generated file. DO NOT MODIFY IT!
class SoundInfoLoader : Datasheet.Loader<SoundInfo>
{
    public void LoadRecord(ref SoundInfo record, string[] values)
    {
        int index = 0;
            if (values[index] != "")
                record.sound = values[index];
            index++;
            if (values[index] != "")
                record.index = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record._filename = values[index];
            index++;
            if (values[index] != "")
                record._volume = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.groupSize = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.loop = Datasheet.ParseBool(values[index]);
            index++;
            if (values[index] != "")
                record._fadeIn = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record._fadeOut = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.deferInst = Datasheet.ParseBool(values[index]);
            index++;
            if (values[index] != "")
                record.stopInst = Datasheet.ParseBool(values[index]);
            index++;
            if (values[index] != "")
                record.duration = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.compound = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.reverb = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.falloff = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.cache = Datasheet.ParseBool(values[index]);
            index++;
            if (values[index] != "")
                record.asyncOnly = Datasheet.ParseBool(values[index]);
            index++;
            if (values[index] != "")
                record.priority = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.stream = Datasheet.ParseBool(values[index]);
            index++;
            if (values[index] != "")
                record.stereo = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.tracking = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.solo = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.musicVol = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.block1 = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.block2 = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.block3 = Datasheet.ParseInt(values[index]);
            index++;
    }
}
