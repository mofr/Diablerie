
// It's generated file. DO NOT MODIFY IT!
class TreasureClassLoader : Datasheet.Loader<TreasureClass>
{
    private TreasureClassNodeLoader treasureclassnodeloader = new TreasureClassNodeLoader();

    public void LoadRecord(ref TreasureClass record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.name);
                Datasheet.Parse(stream.NextString(), ref record.group);
                Datasheet.Parse(stream.NextString(), ref record.level);
                Datasheet.Parse(stream.NextString(), ref record.picks);
                Datasheet.Parse(stream.NextString(), ref record.unique);
                Datasheet.Parse(stream.NextString(), ref record.set);
                Datasheet.Parse(stream.NextString(), ref record.rare);
                Datasheet.Parse(stream.NextString(), ref record.magic);
                Datasheet.Parse(stream.NextString(), ref record.noDrop);
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
                Datasheet.Parse(stream.NextString(), ref record.sumItems);
                Datasheet.Parse(stream.NextString(), ref record.totalProb);
                Datasheet.Parse(stream.NextString(), ref record.dropChance);
                Datasheet.Parse(stream.NextString(), ref record.term);
    }
}
