public class SoundInfoLoader : Datasheet.Loader<SoundInfo>
{
    public void LoadRecord(ref SoundInfo record, string[] fields)
    {
        record.sound = fields[0];
        record.index = Datasheet.ParseInt(fields[1]);
        record._filename = fields[2];
        record._volume = Datasheet.ParseInt(fields[3]);
        record.groupSize = Datasheet.ParseInt(fields[4]);
        record.loop = Datasheet.ParseBool(fields[5]);
        record._fadeIn = Datasheet.ParseInt(fields[6]);
        record._fadeOut = Datasheet.ParseInt(fields[7]);
        record.deferInst = Datasheet.ParseBool(fields[8]);
        record.stopInst = Datasheet.ParseBool(fields[9]);
        record.duration = Datasheet.ParseInt(fields[10]);
        record.compound = Datasheet.ParseInt(fields[11]);
        record.reverb = Datasheet.ParseInt(fields[12]);
        record.falloff = Datasheet.ParseInt(fields[13]);
        record.cache = Datasheet.ParseBool(fields[14]);
        record.asyncOnly = Datasheet.ParseBool(fields[15]);
        record.priority = Datasheet.ParseInt(fields[16]);
        record.stream = Datasheet.ParseBool(fields[17]);
        record.stereo = Datasheet.ParseInt(fields[18]);
        record.tracking = Datasheet.ParseInt(fields[19]);
        record.solo = Datasheet.ParseInt(fields[20]);
        record.musicVol = Datasheet.ParseInt(fields[21]);
        record.block1 = Datasheet.ParseInt(fields[22]);
        record.block2 = Datasheet.ParseInt(fields[23]);
        record.block3 = Datasheet.ParseInt(fields[24]);
    }
}
