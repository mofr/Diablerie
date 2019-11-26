
// It's generated file. DO NOT MODIFY IT!
class TreasureClassNodeLoader : Datasheet.Loader<TreasureClass.Node>
{

    public void LoadRecord(ref TreasureClass.Node record, Datasheet.Stream stream)
    {
                stream.Read(ref record.code);
                stream.Read(ref record.prob);
    }
}
