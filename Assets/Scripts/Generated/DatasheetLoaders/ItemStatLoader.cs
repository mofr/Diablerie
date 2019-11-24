
// It's generated file. DO NOT MODIFY IT!
class ItemStatLoader : Datasheet.Loader<ItemStat>
{

    public void LoadRecord(ref ItemStat record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.code);
                Datasheet.Parse(stream.NextString(), ref record.id);
                Datasheet.Parse(stream.NextString(), ref record.sendOther);
                Datasheet.Parse(stream.NextString(), ref record.signed);
                Datasheet.Parse(stream.NextString(), ref record.sendBits);
                Datasheet.Parse(stream.NextString(), ref record.sendParamBits);
                Datasheet.Parse(stream.NextString(), ref record.updateAnimRate);
                Datasheet.Parse(stream.NextString(), ref record.saved);
                Datasheet.Parse(stream.NextString(), ref record.csvSigned);
                Datasheet.Parse(stream.NextString(), ref record.csvBits);
                Datasheet.Parse(stream.NextString(), ref record.csvParam);
                Datasheet.Parse(stream.NextString(), ref record.fCallback);
                Datasheet.Parse(stream.NextString(), ref record.fMin);
                Datasheet.Parse(stream.NextString(), ref record.minAccr);
                Datasheet.Parse(stream.NextString(), ref record.encode);
                Datasheet.Parse(stream.NextString(), ref record.add);
                Datasheet.Parse(stream.NextString(), ref record.multiply);
                Datasheet.Parse(stream.NextString(), ref record.divide);
                Datasheet.Parse(stream.NextString(), ref record.valShift);
                Datasheet.Parse(stream.NextString(), ref record._1_09_SaveBits);
                Datasheet.Parse(stream.NextString(), ref record._1_09_SaveAdd);
                Datasheet.Parse(stream.NextString(), ref record.saveBits);
                Datasheet.Parse(stream.NextString(), ref record.saveAdd);
                Datasheet.Parse(stream.NextString(), ref record.saveParamBits);
                Datasheet.Parse(stream.NextString(), ref record.keepzero);
                Datasheet.Parse(stream.NextString(), ref record.op);
                Datasheet.Parse(stream.NextString(), ref record.opParam);
                Datasheet.Parse(stream.NextString(), ref record.opBase);
                Datasheet.Parse(stream.NextString(), ref record.opStat1);
                Datasheet.Parse(stream.NextString(), ref record.opStat2);
                Datasheet.Parse(stream.NextString(), ref record.opStat3);
                Datasheet.Parse(stream.NextString(), ref record.direct);
                Datasheet.Parse(stream.NextString(), ref record.maxStat);
                Datasheet.Parse(stream.NextString(), ref record.itemSpecific);
                Datasheet.Parse(stream.NextString(), ref record.damageRelated);
                Datasheet.Parse(stream.NextString(), ref record.itemEvent1);
                Datasheet.Parse(stream.NextString(), ref record.itemEventFunc1);
                Datasheet.Parse(stream.NextString(), ref record.itemEvent2);
                Datasheet.Parse(stream.NextString(), ref record.itemEventFunc2);
                Datasheet.Parse(stream.NextString(), ref record.descPriority);
                Datasheet.Parse(stream.NextString(), ref record.descFunc);
                Datasheet.Parse(stream.NextString(), ref record.descVal);
                Datasheet.Parse(stream.NextString(), ref record.descStrPositive);
                Datasheet.Parse(stream.NextString(), ref record.descStrNegative);
                Datasheet.Parse(stream.NextString(), ref record.descStr2);
                Datasheet.Parse(stream.NextString(), ref record.dgrp);
                Datasheet.Parse(stream.NextString(), ref record.dgrpfunc);
                Datasheet.Parse(stream.NextString(), ref record.dgrpval);
                Datasheet.Parse(stream.NextString(), ref record.dgrpStrPositive);
                Datasheet.Parse(stream.NextString(), ref record.dgrpStrNegative);
                Datasheet.Parse(stream.NextString(), ref record.dgrpStr2);
                Datasheet.Parse(stream.NextString(), ref record.stuff);
                Datasheet.Parse(stream.NextString(), ref record.eol);
    }
}
