
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class TreasureClassNodeLoader : Datasheet.Loader<TreasureClass.Node>
{

    public void LoadRecord(ref TreasureClass.Node record, Datasheet.Stream stream)
    {
                stream.Read(ref record.code);
                stream.Read(ref record.prob);
    }
}
