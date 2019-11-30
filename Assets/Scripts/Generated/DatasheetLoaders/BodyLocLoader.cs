
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class BodyLocLoader : Datasheet.Loader<BodyLoc>
{

    public void LoadRecord(ref BodyLoc record, Datasheet.Stream stream)
    {
                stream.Read(ref record.name);
                stream.Read(ref record.code);
    }
}
