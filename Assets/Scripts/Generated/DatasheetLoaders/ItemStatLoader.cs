
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class ItemStatLoader : Datasheet.Loader<ItemStat>
{

    public void LoadRecord(ref ItemStat record, Datasheet.Stream stream)
    {
                stream.Read(ref record.code);
                stream.Read(ref record.id);
                stream.Read(ref record.sendOther);
                stream.Read(ref record.signed);
                stream.Read(ref record.sendBits);
                stream.Read(ref record.sendParamBits);
                stream.Read(ref record.updateAnimRate);
                stream.Read(ref record.saved);
                stream.Read(ref record.csvSigned);
                stream.Read(ref record.csvBits);
                stream.Read(ref record.csvParam);
                stream.Read(ref record.fCallback);
                stream.Read(ref record.fMin);
                stream.Read(ref record.minAccr);
                stream.Read(ref record.encode);
                stream.Read(ref record.add);
                stream.Read(ref record.multiply);
                stream.Read(ref record.divide);
                stream.Read(ref record.valShift);
                stream.Read(ref record._1_09_SaveBits);
                stream.Read(ref record._1_09_SaveAdd);
                stream.Read(ref record.saveBits);
                stream.Read(ref record.saveAdd);
                stream.Read(ref record.saveParamBits);
                stream.Read(ref record.keepzero);
                stream.Read(ref record.op);
                stream.Read(ref record.opParam);
                stream.Read(ref record.opBase);
                stream.Read(ref record.opStat1);
                stream.Read(ref record.opStat2);
                stream.Read(ref record.opStat3);
                stream.Read(ref record.direct);
                stream.Read(ref record.maxStat);
                stream.Read(ref record.itemSpecific);
                stream.Read(ref record.damageRelated);
                stream.Read(ref record.itemEvent1);
                stream.Read(ref record.itemEventFunc1);
                stream.Read(ref record.itemEvent2);
                stream.Read(ref record.itemEventFunc2);
                stream.Read(ref record.descPriority);
                stream.Read(ref record.descFunc);
                stream.Read(ref record.descVal);
                stream.Read(ref record.descStrPositive);
                stream.Read(ref record.descStrNegative);
                stream.Read(ref record.descStr2);
                stream.Read(ref record.dgrp);
                stream.Read(ref record.dgrpfunc);
                stream.Read(ref record.dgrpval);
                stream.Read(ref record.dgrpStrPositive);
                stream.Read(ref record.dgrpStrNegative);
                stream.Read(ref record.dgrpStr2);
                stream.Read(ref record.stuff);
                stream.Read(ref record.eol);
    }
}
