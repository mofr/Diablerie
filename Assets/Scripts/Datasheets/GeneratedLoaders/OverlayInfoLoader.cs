public class OverlayInfoLoader : Datasheet.Loader<OverlayInfo>
{
    public void LoadRecord(ref OverlayInfo record, string[] fields)
    {
        record.id = fields[0];
        record.filename = fields[1];
        record.unused = new string[3];
        record.unused[0] = fields[2];
        record.unused[1] = fields[3];
        record.unused[2] = fields[4];
        if (fields[5] != "")
            record.preDraw = Datasheet.ParseBool(fields[5]);
        record.unused2 = new string[4];
        record.unused2[0] = fields[6];
        record.unused2[1] = fields[7];
        record.unused2[2] = fields[8];
        record.unused2[3] = fields[9];
        record.xOffset = Datasheet.ParseInt(fields[10]);
        record.yOffset = Datasheet.ParseInt(fields[11]);
        record.height1 = Datasheet.ParseInt(fields[12]);
        record.height2 = Datasheet.ParseInt(fields[13]);
        record.height3 = Datasheet.ParseInt(fields[14]);
        record.height4 = Datasheet.ParseInt(fields[15]);
        record.fps = Datasheet.ParseInt(fields[16]);
        record.loopWaitTime = fields[17];
        record.trans = Datasheet.ParseInt(fields[18]);
        record.initRadius = Datasheet.ParseInt(fields[19]);
        record.radius = Datasheet.ParseInt(fields[20]);
        record.red = Datasheet.ParseInt(fields[21]);
        record.green = Datasheet.ParseInt(fields[22]);
        record.blue = Datasheet.ParseInt(fields[23]);
        record.numDirections = Datasheet.ParseInt(fields[24]);
        if (fields[25] != "")
            record.localBlood = Datasheet.ParseBool(fields[25]);
    }
}
