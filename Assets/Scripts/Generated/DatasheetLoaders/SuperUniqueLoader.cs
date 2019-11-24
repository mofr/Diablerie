
// It's generated file. DO NOT MODIFY IT!
class SuperUniqueLoader : Datasheet.Loader<SuperUnique>
{

    public void LoadRecord(ref SuperUnique record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.superUnique);
                Datasheet.Parse(stream.NextString(), ref record.nameStr);
                Datasheet.Parse(stream.NextString(), ref record.monStatId);
                Datasheet.Parse(stream.NextString(), ref record.hcIdx);
                Datasheet.Parse(stream.NextString(), ref record.monSound);
                Datasheet.Parse(stream.NextString(), ref record.mod1);
                Datasheet.Parse(stream.NextString(), ref record.mod2);
                Datasheet.Parse(stream.NextString(), ref record.mod3);
                Datasheet.Parse(stream.NextString(), ref record.minGrp);
                Datasheet.Parse(stream.NextString(), ref record.maxGrp);
                Datasheet.Parse(stream.NextString(), ref record.eClass);
                Datasheet.Parse(stream.NextString(), ref record.autoPos);
                Datasheet.Parse(stream.NextString(), ref record.stacks);
                Datasheet.Parse(stream.NextString(), ref record.replacable);
                record.uTrans = new int[3];
                    Datasheet.Parse(stream.NextString(), ref record.uTrans[0]);
                    Datasheet.Parse(stream.NextString(), ref record.uTrans[1]);
                    Datasheet.Parse(stream.NextString(), ref record.uTrans[2]);
                record.treasureClass = new string[3];
                    Datasheet.Parse(stream.NextString(), ref record.treasureClass[0]);
                    Datasheet.Parse(stream.NextString(), ref record.treasureClass[1]);
                    Datasheet.Parse(stream.NextString(), ref record.treasureClass[2]);
                Datasheet.Parse(stream.NextString(), ref record.eol);
    }
}
