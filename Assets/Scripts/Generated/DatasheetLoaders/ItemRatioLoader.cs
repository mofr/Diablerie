
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class ItemRatioLoader : Datasheet.Loader<ItemRatio>
{

    public void LoadRecord(ref ItemRatio record, Datasheet.Stream stream)
    {
                stream.Read(ref record.function);
                stream.Read(ref record.version);
                stream.Read(ref record.uber);
                stream.Read(ref record.classSpecific);
                stream.Read(ref record.unique);
                stream.Read(ref record.uniqueDivisor);
                stream.Read(ref record.uniqueMin);
                stream.Read(ref record.rare);
                stream.Read(ref record.rareDivisor);
                stream.Read(ref record.rareMin);
                stream.Read(ref record.set);
                stream.Read(ref record.setDivisor);
                stream.Read(ref record.setMin);
                stream.Read(ref record.magic);
                stream.Read(ref record.magicDivisor);
                stream.Read(ref record.magicMin);
                stream.Read(ref record.hiQuality);
                stream.Read(ref record.hiQualityDivisor);
                stream.Read(ref record.normal);
                stream.Read(ref record.normalDivisor);
    }
}
