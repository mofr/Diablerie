
// It's generated file. DO NOT MODIFY IT!
class OverlayInfoLoader : Datasheet.Loader<OverlayInfo>
{
    public void LoadRecord(ref OverlayInfo record, string[] values)
    {
        int index = 0;
                Datasheet.Parse(values[index++], ref record.id);
                Datasheet.Parse(values[index++], ref record.filename);
                record.unused = new string[3];
                    Datasheet.Parse(values[index++], ref record.unused[0]);
                    Datasheet.Parse(values[index++], ref record.unused[1]);
                    Datasheet.Parse(values[index++], ref record.unused[2]);
                Datasheet.Parse(values[index++], ref record.preDraw);
                record.unused2 = new string[4];
                    Datasheet.Parse(values[index++], ref record.unused2[0]);
                    Datasheet.Parse(values[index++], ref record.unused2[1]);
                    Datasheet.Parse(values[index++], ref record.unused2[2]);
                    Datasheet.Parse(values[index++], ref record.unused2[3]);
                Datasheet.Parse(values[index++], ref record.xOffset);
                Datasheet.Parse(values[index++], ref record.yOffset);
                Datasheet.Parse(values[index++], ref record.height1);
                Datasheet.Parse(values[index++], ref record.height2);
                Datasheet.Parse(values[index++], ref record.height3);
                Datasheet.Parse(values[index++], ref record.height4);
                Datasheet.Parse(values[index++], ref record.fps);
                Datasheet.Parse(values[index++], ref record.loopWaitTime);
                Datasheet.Parse(values[index++], ref record.trans);
                Datasheet.Parse(values[index++], ref record.initRadius);
                Datasheet.Parse(values[index++], ref record.radius);
                Datasheet.Parse(values[index++], ref record.red);
                Datasheet.Parse(values[index++], ref record.green);
                Datasheet.Parse(values[index++], ref record.blue);
                Datasheet.Parse(values[index++], ref record.numDirections);
                Datasheet.Parse(values[index++], ref record.localBlood);
    }
}
