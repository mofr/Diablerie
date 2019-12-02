
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class SuperUniqueLoader : Datasheet.Loader<SuperUnique>
{

    public void LoadRecord(ref SuperUnique record, DatasheetStream stream)
    {
                stream.Read(ref record.superUnique);
                stream.Read(ref record.nameStr);
                stream.Read(ref record.monStatId);
                stream.Read(ref record.hcIdx);
                stream.Read(ref record.monSound);
                stream.Read(ref record.mod1);
                stream.Read(ref record.mod2);
                stream.Read(ref record.mod3);
                stream.Read(ref record.minGrp);
                stream.Read(ref record.maxGrp);
                stream.Read(ref record.eClass);
                stream.Read(ref record.autoPos);
                stream.Read(ref record.stacks);
                stream.Read(ref record.replacable);
                record.uTrans = new int[3];
                    stream.Read(ref record.uTrans[0]);
                    stream.Read(ref record.uTrans[1]);
                    stream.Read(ref record.uTrans[2]);
                record.treasureClass = new string[3];
                    stream.Read(ref record.treasureClass[0]);
                    stream.Read(ref record.treasureClass[1]);
                    stream.Read(ref record.treasureClass[2]);
                stream.Read(ref record.eol);
    }
}
