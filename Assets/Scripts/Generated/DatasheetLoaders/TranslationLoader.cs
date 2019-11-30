
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class TranslationLoader : Datasheet.Loader<Translation>
{

    public void LoadRecord(ref Translation record, Datasheet.Stream stream)
    {
                stream.Read(ref record.key);
                stream.Read(ref record.value);
    }
}
