
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class SoundInfoLoader : Datasheet.Loader<SoundInfo>
{

    public void LoadRecord(ref SoundInfo record, DatasheetStream stream)
    {
                stream.Read(ref record.sound);
                stream.Read(ref record.index);
                stream.Read(ref record._filename);
                stream.Read(ref record._volume);
                stream.Read(ref record.groupSize);
                stream.Read(ref record.loop);
                stream.Read(ref record._fadeIn);
                stream.Read(ref record._fadeOut);
                stream.Read(ref record.deferInst);
                stream.Read(ref record.stopInst);
                stream.Read(ref record.duration);
                stream.Read(ref record.compound);
                stream.Read(ref record.reverb);
                stream.Read(ref record.falloff);
                stream.Read(ref record.cache);
                stream.Read(ref record.asyncOnly);
                stream.Read(ref record.priority);
                stream.Read(ref record.stream);
                stream.Read(ref record.stereo);
                stream.Read(ref record.tracking);
                stream.Read(ref record.solo);
                stream.Read(ref record.musicVol);
                stream.Read(ref record.block1);
                stream.Read(ref record.block2);
                stream.Read(ref record.block3);
    }
}
