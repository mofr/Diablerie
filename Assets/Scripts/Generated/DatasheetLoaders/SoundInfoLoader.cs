
// It's generated file. DO NOT MODIFY IT!
class SoundInfoLoader : Datasheet.Loader<SoundInfo>
{
    public void LoadRecord(ref SoundInfo record, string[] values)
    {
        int index = 0;
                Datasheet.Parse(values[index++], ref record.sound);
                Datasheet.Parse(values[index++], ref record.index);
                Datasheet.Parse(values[index++], ref record._filename);
                Datasheet.Parse(values[index++], ref record._volume);
                Datasheet.Parse(values[index++], ref record.groupSize);
                Datasheet.Parse(values[index++], ref record.loop);
                Datasheet.Parse(values[index++], ref record._fadeIn);
                Datasheet.Parse(values[index++], ref record._fadeOut);
                Datasheet.Parse(values[index++], ref record.deferInst);
                Datasheet.Parse(values[index++], ref record.stopInst);
                Datasheet.Parse(values[index++], ref record.duration);
                Datasheet.Parse(values[index++], ref record.compound);
                Datasheet.Parse(values[index++], ref record.reverb);
                Datasheet.Parse(values[index++], ref record.falloff);
                Datasheet.Parse(values[index++], ref record.cache);
                Datasheet.Parse(values[index++], ref record.asyncOnly);
                Datasheet.Parse(values[index++], ref record.priority);
                Datasheet.Parse(values[index++], ref record.stream);
                Datasheet.Parse(values[index++], ref record.stereo);
                Datasheet.Parse(values[index++], ref record.tracking);
                Datasheet.Parse(values[index++], ref record.solo);
                Datasheet.Parse(values[index++], ref record.musicVol);
                Datasheet.Parse(values[index++], ref record.block1);
                Datasheet.Parse(values[index++], ref record.block2);
                Datasheet.Parse(values[index++], ref record.block3);
    }
}
