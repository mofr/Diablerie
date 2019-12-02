
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class StateInfoLoader : Datasheet.Loader<StateInfo>
{

    public void LoadRecord(ref StateInfo record, DatasheetStream stream)
    {
                stream.Read(ref record.code);
                stream.Read(ref record.id);
                stream.Read(ref record.group);
                stream.Read(ref record.remhit);
                stream.Read(ref record.nosend);
                stream.Read(ref record.transform);
                stream.Read(ref record.aura);
                stream.Read(ref record.curable);
                stream.Read(ref record.curse);
                stream.Read(ref record.active);
                stream.Read(ref record.immed);
                stream.Read(ref record.restrict);
                stream.Read(ref record.disguise);
                stream.Read(ref record.blue);
                stream.Read(ref record.attblue);
                stream.Read(ref record.damblue);
                stream.Read(ref record.armblue);
                stream.Read(ref record.rfblue);
                stream.Read(ref record.rlblue);
                stream.Read(ref record.rcblue);
                stream.Read(ref record.stambarblue);
                stream.Read(ref record.rpblue);
                stream.Read(ref record.attred);
                stream.Read(ref record.damred);
                stream.Read(ref record.armred);
                stream.Read(ref record.rfred);
                stream.Read(ref record.rlred);
                stream.Read(ref record.rcred);
                stream.Read(ref record.rpred);
                stream.Read(ref record.exp);
                stream.Read(ref record.plrstaydeath);
                stream.Read(ref record.monstaydeath);
                stream.Read(ref record.bossstaydeath);
                stream.Read(ref record.hide);
                stream.Read(ref record.shatter);
                stream.Read(ref record.udead);
                stream.Read(ref record.life);
                stream.Read(ref record.green);
                stream.Read(ref record.pgsv);
                stream.Read(ref record.nooverlays);
                stream.Read(ref record.noclear);
                stream.Read(ref record.bossinv);
                stream.Read(ref record.meleeonly);
                stream.Read(ref record.notondead);
                stream.Read(ref record.overlay1);
                stream.Read(ref record.overlay2);
                stream.Read(ref record.overlay3);
                stream.Read(ref record.overlay4);
                stream.Read(ref record.pgsvoverlay);
                stream.Read(ref record.castoverlay);
                stream.Read(ref record.removerlay);
                stream.Read(ref record.stat);
                stream.Read(ref record.setfunc);
                stream.Read(ref record.remfunc);
                stream.Read(ref record.missile);
                stream.Read(ref record.skill);
                stream.Read(ref record.itemtype);
                stream.Read(ref record.itemtrans);
                stream.Read(ref record.colorpri);
                stream.Read(ref record.colorshift);
                stream.Read(ref record.lightr);
                stream.Read(ref record.lightg);
                stream.Read(ref record.lightb);
                stream.Read(ref record.onsound);
                stream.Read(ref record.offsound);
                stream.Read(ref record.gfxtype);
                stream.Read(ref record.gfxclass);
                stream.Read(ref record.cltevent);
                stream.Read(ref record.clteventfunc);
                stream.Read(ref record.cltactivefunc);
                stream.Read(ref record.srvactivefunc);
                stream.Read(ref record.eol);
    }
}
