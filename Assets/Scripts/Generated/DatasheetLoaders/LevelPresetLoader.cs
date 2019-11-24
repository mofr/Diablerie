
// It's generated file. DO NOT MODIFY IT!
class LevelPresetLoader : Datasheet.Loader<LevelPreset>
{

    public void LoadRecord(ref LevelPreset record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.name);
                Datasheet.Parse(stream.NextString(), ref record.def);
                Datasheet.Parse(stream.NextString(), ref record.levelId);
                Datasheet.Parse(stream.NextString(), ref record.populate);
                Datasheet.Parse(stream.NextString(), ref record.logicals);
                Datasheet.Parse(stream.NextString(), ref record.outdoors);
                Datasheet.Parse(stream.NextString(), ref record.animate);
                Datasheet.Parse(stream.NextString(), ref record.killEdge);
                Datasheet.Parse(stream.NextString(), ref record.fillBlanks);
                Datasheet.Parse(stream.NextString(), ref record.sizeX);
                Datasheet.Parse(stream.NextString(), ref record.sizeY);
                Datasheet.Parse(stream.NextString(), ref record.autoMap);
                Datasheet.Parse(stream.NextString(), ref record.scan);
                Datasheet.Parse(stream.NextString(), ref record.pops);
                Datasheet.Parse(stream.NextString(), ref record.popPad);
                Datasheet.Parse(stream.NextString(), ref record.fileCount);
                record.files = new string[6];
                    Datasheet.Parse(stream.NextString(), ref record.files[0]);
                    Datasheet.Parse(stream.NextString(), ref record.files[1]);
                    Datasheet.Parse(stream.NextString(), ref record.files[2]);
                    Datasheet.Parse(stream.NextString(), ref record.files[3]);
                    Datasheet.Parse(stream.NextString(), ref record.files[4]);
                    Datasheet.Parse(stream.NextString(), ref record.files[5]);
                Datasheet.Parse(stream.NextString(), ref record.dt1Mask);
                Datasheet.Parse(stream.NextString(), ref record.beta);
    }
}
