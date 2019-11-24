
// It's generated file. DO NOT MODIFY IT!
class ItemRatioLoader : Datasheet.Loader<ItemRatio>
{

    public void LoadRecord(ref ItemRatio record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.function);
                Datasheet.Parse(stream.NextString(), ref record.version);
                Datasheet.Parse(stream.NextString(), ref record.uber);
                Datasheet.Parse(stream.NextString(), ref record.classSpecific);
                Datasheet.Parse(stream.NextString(), ref record.unique);
                Datasheet.Parse(stream.NextString(), ref record.uniqueDivisor);
                Datasheet.Parse(stream.NextString(), ref record.uniqueMin);
                Datasheet.Parse(stream.NextString(), ref record.rare);
                Datasheet.Parse(stream.NextString(), ref record.rareDivisor);
                Datasheet.Parse(stream.NextString(), ref record.rareMin);
                Datasheet.Parse(stream.NextString(), ref record.set);
                Datasheet.Parse(stream.NextString(), ref record.setDivisor);
                Datasheet.Parse(stream.NextString(), ref record.setMin);
                Datasheet.Parse(stream.NextString(), ref record.magic);
                Datasheet.Parse(stream.NextString(), ref record.magicDivisor);
                Datasheet.Parse(stream.NextString(), ref record.magicMin);
                Datasheet.Parse(stream.NextString(), ref record.hiQuality);
                Datasheet.Parse(stream.NextString(), ref record.hiQualityDivisor);
                Datasheet.Parse(stream.NextString(), ref record.normal);
                Datasheet.Parse(stream.NextString(), ref record.normalDivisor);
    }
}
