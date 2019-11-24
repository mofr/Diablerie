
// It's generated file. DO NOT MODIFY IT!
class SoundInfoLoader : Datasheet.Loader<SoundInfo>
{
    public void LoadRecord(ref SoundInfo record, string[] values)
    {
        int index = 0;
            Datasheet.Parse(values[index], ref record.sound);
            index++;
            Datasheet.Parse(values[index], ref record.index);
            index++;
            Datasheet.Parse(values[index], ref record._filename);
            index++;
            Datasheet.Parse(values[index], ref record._volume);
            index++;
            Datasheet.Parse(values[index], ref record.groupSize);
            index++;
            Datasheet.Parse(values[index], ref record.loop);
            index++;
            Datasheet.Parse(values[index], ref record._fadeIn);
            index++;
            Datasheet.Parse(values[index], ref record._fadeOut);
            index++;
            Datasheet.Parse(values[index], ref record.deferInst);
            index++;
            Datasheet.Parse(values[index], ref record.stopInst);
            index++;
            Datasheet.Parse(values[index], ref record.duration);
            index++;
            Datasheet.Parse(values[index], ref record.compound);
            index++;
            Datasheet.Parse(values[index], ref record.reverb);
            index++;
            Datasheet.Parse(values[index], ref record.falloff);
            index++;
            Datasheet.Parse(values[index], ref record.cache);
            index++;
            Datasheet.Parse(values[index], ref record.asyncOnly);
            index++;
            Datasheet.Parse(values[index], ref record.priority);
            index++;
            Datasheet.Parse(values[index], ref record.stream);
            index++;
            Datasheet.Parse(values[index], ref record.stereo);
            index++;
            Datasheet.Parse(values[index], ref record.tracking);
            index++;
            Datasheet.Parse(values[index], ref record.solo);
            index++;
            Datasheet.Parse(values[index], ref record.musicVol);
            index++;
            Datasheet.Parse(values[index], ref record.block1);
            index++;
            Datasheet.Parse(values[index], ref record.block2);
            index++;
            Datasheet.Parse(values[index], ref record.block3);
            index++;
    }
}
