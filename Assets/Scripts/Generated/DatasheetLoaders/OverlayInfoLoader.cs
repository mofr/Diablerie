
// It's generated file. DO NOT MODIFY IT!
class OverlayInfoLoader : Datasheet.Loader<OverlayInfo>
{

    public void LoadRecord(ref OverlayInfo record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.id);
                Datasheet.Parse(stream.NextString(), ref record.filename);
                record.unused = new string[3];
                    Datasheet.Parse(stream.NextString(), ref record.unused[0]);
                    Datasheet.Parse(stream.NextString(), ref record.unused[1]);
                    Datasheet.Parse(stream.NextString(), ref record.unused[2]);
                Datasheet.Parse(stream.NextString(), ref record.preDraw);
                record.unused2 = new string[4];
                    Datasheet.Parse(stream.NextString(), ref record.unused2[0]);
                    Datasheet.Parse(stream.NextString(), ref record.unused2[1]);
                    Datasheet.Parse(stream.NextString(), ref record.unused2[2]);
                    Datasheet.Parse(stream.NextString(), ref record.unused2[3]);
                Datasheet.Parse(stream.NextString(), ref record.xOffset);
                Datasheet.Parse(stream.NextString(), ref record.yOffset);
                Datasheet.Parse(stream.NextString(), ref record.height1);
                Datasheet.Parse(stream.NextString(), ref record.height2);
                Datasheet.Parse(stream.NextString(), ref record.height3);
                Datasheet.Parse(stream.NextString(), ref record.height4);
                Datasheet.Parse(stream.NextString(), ref record.fps);
                Datasheet.Parse(stream.NextString(), ref record.loopWaitTime);
                Datasheet.Parse(stream.NextString(), ref record.trans);
                Datasheet.Parse(stream.NextString(), ref record.initRadius);
                Datasheet.Parse(stream.NextString(), ref record.radius);
                Datasheet.Parse(stream.NextString(), ref record.red);
                Datasheet.Parse(stream.NextString(), ref record.green);
                Datasheet.Parse(stream.NextString(), ref record.blue);
                Datasheet.Parse(stream.NextString(), ref record.numDirections);
                Datasheet.Parse(stream.NextString(), ref record.localBlood);
    }
}
