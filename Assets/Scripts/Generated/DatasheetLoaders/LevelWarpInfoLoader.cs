
// It's generated file. DO NOT MODIFY IT!
class LevelWarpInfoLoader : Datasheet.Loader<LevelWarpInfo>
{

    public void LoadRecord(ref LevelWarpInfo record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.name);
                Datasheet.Parse(stream.NextString(), ref record.id);
                Datasheet.Parse(stream.NextString(), ref record.selectX);
                Datasheet.Parse(stream.NextString(), ref record.selectY);
                Datasheet.Parse(stream.NextString(), ref record.selectDX);
                Datasheet.Parse(stream.NextString(), ref record.selectDY);
                Datasheet.Parse(stream.NextString(), ref record.exitWalkX);
                Datasheet.Parse(stream.NextString(), ref record.exitWalkY);
                Datasheet.Parse(stream.NextString(), ref record.offsetX);
                Datasheet.Parse(stream.NextString(), ref record.offsetY);
                Datasheet.Parse(stream.NextString(), ref record.litVersion);
                Datasheet.Parse(stream.NextString(), ref record.tiles);
                Datasheet.Parse(stream.NextString(), ref record.direction);
                Datasheet.Parse(stream.NextString(), ref record.beta);
    }
}
