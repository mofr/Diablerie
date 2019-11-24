
// It's generated file. DO NOT MODIFY IT!
class TreasureClassNodeLoader : Datasheet.Loader<TreasureClass.Node>
{

    public void LoadRecord(ref TreasureClass.Node record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.code);
                Datasheet.Parse(stream.NextString(), ref record.prob);
    }
}
