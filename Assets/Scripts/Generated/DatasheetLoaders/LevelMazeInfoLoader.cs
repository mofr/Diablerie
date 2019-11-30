
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class LevelMazeInfoLoader : Datasheet.Loader<LevelMazeInfo>
{

    public void LoadRecord(ref LevelMazeInfo record, Datasheet.Stream stream)
    {
                stream.Read(ref record.name);
                stream.Read(ref record.levelId);
                record.rooms = new int[3];
                    stream.Read(ref record.rooms[0]);
                    stream.Read(ref record.rooms[1]);
                    stream.Read(ref record.rooms[2]);
                stream.Read(ref record.sizeX);
                stream.Read(ref record.sizeY);
                stream.Read(ref record.merge);
                stream.Read(ref record.beta);
    }
}
