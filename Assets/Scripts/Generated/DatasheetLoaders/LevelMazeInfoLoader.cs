
// It's generated file. DO NOT MODIFY IT!
class LevelMazeInfoLoader : Datasheet.Loader<LevelMazeInfo>
{

    public void LoadRecord(ref LevelMazeInfo record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.name);
                Datasheet.Parse(stream.NextString(), ref record.levelId);
                record.rooms = new int[3];
                    Datasheet.Parse(stream.NextString(), ref record.rooms[0]);
                    Datasheet.Parse(stream.NextString(), ref record.rooms[1]);
                    Datasheet.Parse(stream.NextString(), ref record.rooms[2]);
                Datasheet.Parse(stream.NextString(), ref record.sizeX);
                Datasheet.Parse(stream.NextString(), ref record.sizeY);
                Datasheet.Parse(stream.NextString(), ref record.merge);
                Datasheet.Parse(stream.NextString(), ref record.beta);
    }
}
