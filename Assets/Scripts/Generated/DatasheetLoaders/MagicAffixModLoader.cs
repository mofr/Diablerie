
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class MagicAffixModLoader : Datasheet.Loader<MagicAffix.Mod>
{

    public void LoadRecord(ref MagicAffix.Mod record, DatasheetStream stream)
    {
                stream.Read(ref record.code);
                stream.Read(ref record.param);
                stream.Read(ref record.min);
                stream.Read(ref record.max);
    }
}
