
// It's generated file. DO NOT MODIFY IT!
class SoundInfoLoader : Datasheet.Loader<SoundInfo>
{

    public void LoadRecord(ref SoundInfo record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.sound);
                Datasheet.Parse(stream.NextString(), ref record.index);
                Datasheet.Parse(stream.NextString(), ref record._filename);
                Datasheet.Parse(stream.NextString(), ref record._volume);
                Datasheet.Parse(stream.NextString(), ref record.groupSize);
                Datasheet.Parse(stream.NextString(), ref record.loop);
                Datasheet.Parse(stream.NextString(), ref record._fadeIn);
                Datasheet.Parse(stream.NextString(), ref record._fadeOut);
                Datasheet.Parse(stream.NextString(), ref record.deferInst);
                Datasheet.Parse(stream.NextString(), ref record.stopInst);
                Datasheet.Parse(stream.NextString(), ref record.duration);
                Datasheet.Parse(stream.NextString(), ref record.compound);
                Datasheet.Parse(stream.NextString(), ref record.reverb);
                Datasheet.Parse(stream.NextString(), ref record.falloff);
                Datasheet.Parse(stream.NextString(), ref record.cache);
                Datasheet.Parse(stream.NextString(), ref record.asyncOnly);
                Datasheet.Parse(stream.NextString(), ref record.priority);
                Datasheet.Parse(stream.NextString(), ref record.stream);
                Datasheet.Parse(stream.NextString(), ref record.stereo);
                Datasheet.Parse(stream.NextString(), ref record.tracking);
                Datasheet.Parse(stream.NextString(), ref record.solo);
                Datasheet.Parse(stream.NextString(), ref record.musicVol);
                Datasheet.Parse(stream.NextString(), ref record.block1);
                Datasheet.Parse(stream.NextString(), ref record.block2);
                Datasheet.Parse(stream.NextString(), ref record.block3);
    }
}
