
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class LevelWarpInfoLoader : Datasheet.Loader<LevelWarpInfo>
{

    public void LoadRecord(ref LevelWarpInfo record, DatasheetStream stream)
    {
                stream.Read(ref record.name);
                stream.Read(ref record.id);
                stream.Read(ref record.selectX);
                stream.Read(ref record.selectY);
                stream.Read(ref record.selectDX);
                stream.Read(ref record.selectDY);
                stream.Read(ref record.exitWalkX);
                stream.Read(ref record.exitWalkY);
                stream.Read(ref record.offsetX);
                stream.Read(ref record.offsetY);
                stream.Read(ref record.litVersion);
                stream.Read(ref record.tiles);
                stream.Read(ref record.direction);
                stream.Read(ref record.beta);
    }
}
