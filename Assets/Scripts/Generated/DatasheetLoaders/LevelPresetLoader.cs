
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class LevelPresetLoader : Datasheet.Loader<LevelPreset>
{

    public void LoadRecord(ref LevelPreset record, Datasheet.Stream stream)
    {
                stream.Read(ref record.name);
                stream.Read(ref record.def);
                stream.Read(ref record.levelId);
                stream.Read(ref record.populate);
                stream.Read(ref record.logicals);
                stream.Read(ref record.outdoors);
                stream.Read(ref record.animate);
                stream.Read(ref record.killEdge);
                stream.Read(ref record.fillBlanks);
                stream.Read(ref record.sizeX);
                stream.Read(ref record.sizeY);
                stream.Read(ref record.autoMap);
                stream.Read(ref record.scan);
                stream.Read(ref record.pops);
                stream.Read(ref record.popPad);
                stream.Read(ref record.fileCount);
                record.files = new string[6];
                    stream.Read(ref record.files[0]);
                    stream.Read(ref record.files[1]);
                    stream.Read(ref record.files[2]);
                    stream.Read(ref record.files[3]);
                    stream.Read(ref record.files[4]);
                    stream.Read(ref record.files[5]);
                stream.Read(ref record.dt1Mask);
                stream.Read(ref record.beta);
    }
}
