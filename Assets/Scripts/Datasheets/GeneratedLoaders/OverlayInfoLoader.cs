
// It's generated file. DO NOT MODIFY IT!
class OverlayInfoLoader : Datasheet.Loader<OverlayInfo>
{
    public void LoadRecord(ref OverlayInfo record, string[] values)
    {
        int index = 0;
            if (values[index] != "")
                record.id = values[index];
            index++;
            if (values[index] != "")
                record.filename = values[index];
            index++;
            record.unused = new string[3];
            index += 3;  // TODO implement arrays
            if (values[index] != "")
                record.preDraw = Datasheet.ParseBool(values[index]);
            index++;
            record.unused2 = new string[4];
            index += 4;  // TODO implement arrays
            if (values[index] != "")
                record.xOffset = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.yOffset = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.height1 = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.height2 = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.height3 = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.height4 = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.fps = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.loopWaitTime = values[index];
            index++;
            if (values[index] != "")
                record.trans = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.initRadius = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.radius = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.red = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.green = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.blue = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.numDirections = Datasheet.ParseInt(values[index]);
            index++;
            if (values[index] != "")
                record.localBlood = Datasheet.ParseBool(values[index]);
            index++;
    }
}
