
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class MonStatTreasureClassInfoLoader : Datasheet.Loader<MonStat.TreasureClassInfo>
{

    public void LoadRecord(ref MonStat.TreasureClassInfo record, DatasheetStream stream)
    {
                stream.Read(ref record._normal);
                stream.Read(ref record._champion);
                stream.Read(ref record._unique);
                stream.Read(ref record._quest);
    }
}
