
// It's generated file. DO NOT MODIFY IT!
class OverlayInfoLoader : Datasheet.Loader<OverlayInfo>
{
    public void LoadRecord(ref OverlayInfo record, string[] values)
    {
        int index = 0;
            Datasheet.Parse(values[index], ref record.id);
            index++;
            Datasheet.Parse(values[index], ref record.filename);
            index++;
            record.unused = new string[3];
            index += 3;  // TODO implement arrays
            Datasheet.Parse(values[index], ref record.preDraw);
            index++;
            record.unused2 = new string[4];
            index += 4;  // TODO implement arrays
            Datasheet.Parse(values[index], ref record.xOffset);
            index++;
            Datasheet.Parse(values[index], ref record.yOffset);
            index++;
            Datasheet.Parse(values[index], ref record.height1);
            index++;
            Datasheet.Parse(values[index], ref record.height2);
            index++;
            Datasheet.Parse(values[index], ref record.height3);
            index++;
            Datasheet.Parse(values[index], ref record.height4);
            index++;
            Datasheet.Parse(values[index], ref record.fps);
            index++;
            Datasheet.Parse(values[index], ref record.loopWaitTime);
            index++;
            Datasheet.Parse(values[index], ref record.trans);
            index++;
            Datasheet.Parse(values[index], ref record.initRadius);
            index++;
            Datasheet.Parse(values[index], ref record.radius);
            index++;
            Datasheet.Parse(values[index], ref record.red);
            index++;
            Datasheet.Parse(values[index], ref record.green);
            index++;
            Datasheet.Parse(values[index], ref record.blue);
            index++;
            Datasheet.Parse(values[index], ref record.numDirections);
            index++;
            Datasheet.Parse(values[index], ref record.localBlood);
            index++;
    }
}
