
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class TreasureClassLoader : Datasheet.Loader<TreasureClass>
{
    private TreasureClassNodeLoader treasureclassnodeloader = new TreasureClassNodeLoader();

    public void LoadRecord(ref TreasureClass record, Datasheet.Stream stream)
    {
                stream.Read(ref record.name);
                stream.Read(ref record.group);
                stream.Read(ref record.level);
                stream.Read(ref record.picks);
                stream.Read(ref record.unique);
                stream.Read(ref record.set);
                stream.Read(ref record.rare);
                stream.Read(ref record.magic);
                stream.Read(ref record.noDrop);
                record.nodeArray = new TreasureClass.Node[10];
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[0], stream);
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[1], stream);
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[2], stream);
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[3], stream);
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[4], stream);
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[5], stream);
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[6], stream);
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[7], stream);
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[8], stream);
                    treasureclassnodeloader.LoadRecord(ref record.nodeArray[9], stream);
                stream.Read(ref record.sumItems);
                stream.Read(ref record.totalProb);
                stream.Read(ref record.dropChance);
                stream.Read(ref record.term);
    }
}
