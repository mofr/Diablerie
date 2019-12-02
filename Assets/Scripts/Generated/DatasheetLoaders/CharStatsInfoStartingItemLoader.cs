
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class CharStatsInfoStartingItemLoader : Datasheet.Loader<CharStatsInfo.StartingItem>
{

    public void LoadRecord(ref CharStatsInfo.StartingItem record, DatasheetStream stream)
    {
                stream.Read(ref record.code);
                stream.Read(ref record.loc);
                stream.Read(ref record.count);
    }
}
