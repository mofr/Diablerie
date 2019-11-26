
// It's generated file. DO NOT MODIFY IT!
class OverlayInfoLoader : Datasheet.Loader<OverlayInfo>
{

    public void LoadRecord(ref OverlayInfo record, Datasheet.Stream stream)
    {
                stream.Read(ref record.id);
                stream.Read(ref record.filename);
                record.unused = new string[3];
                    stream.Read(ref record.unused[0]);
                    stream.Read(ref record.unused[1]);
                    stream.Read(ref record.unused[2]);
                stream.Read(ref record.preDraw);
                record.unused2 = new string[4];
                    stream.Read(ref record.unused2[0]);
                    stream.Read(ref record.unused2[1]);
                    stream.Read(ref record.unused2[2]);
                    stream.Read(ref record.unused2[3]);
                stream.Read(ref record.xOffset);
                stream.Read(ref record.yOffset);
                stream.Read(ref record.height1);
                stream.Read(ref record.height2);
                stream.Read(ref record.height3);
                stream.Read(ref record.height4);
                stream.Read(ref record.fps);
                stream.Read(ref record.loopWaitTime);
                stream.Read(ref record.trans);
                stream.Read(ref record.initRadius);
                stream.Read(ref record.radius);
                stream.Read(ref record.red);
                stream.Read(ref record.green);
                stream.Read(ref record.blue);
                stream.Read(ref record.numDirections);
                stream.Read(ref record.localBlood);
    }
}
