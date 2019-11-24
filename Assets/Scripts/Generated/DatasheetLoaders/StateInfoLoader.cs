
// It's generated file. DO NOT MODIFY IT!
class StateInfoLoader : Datasheet.Loader<StateInfo>
{

    public void LoadRecord(ref StateInfo record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.code);
                Datasheet.Parse(stream.NextString(), ref record.id);
                Datasheet.Parse(stream.NextString(), ref record.group);
                Datasheet.Parse(stream.NextString(), ref record.remhit);
                Datasheet.Parse(stream.NextString(), ref record.nosend);
                Datasheet.Parse(stream.NextString(), ref record.transform);
                Datasheet.Parse(stream.NextString(), ref record.aura);
                Datasheet.Parse(stream.NextString(), ref record.curable);
                Datasheet.Parse(stream.NextString(), ref record.curse);
                Datasheet.Parse(stream.NextString(), ref record.active);
                Datasheet.Parse(stream.NextString(), ref record.immed);
                Datasheet.Parse(stream.NextString(), ref record.restrict);
                Datasheet.Parse(stream.NextString(), ref record.disguise);
                Datasheet.Parse(stream.NextString(), ref record.blue);
                Datasheet.Parse(stream.NextString(), ref record.attblue);
                Datasheet.Parse(stream.NextString(), ref record.damblue);
                Datasheet.Parse(stream.NextString(), ref record.armblue);
                Datasheet.Parse(stream.NextString(), ref record.rfblue);
                Datasheet.Parse(stream.NextString(), ref record.rlblue);
                Datasheet.Parse(stream.NextString(), ref record.rcblue);
                Datasheet.Parse(stream.NextString(), ref record.stambarblue);
                Datasheet.Parse(stream.NextString(), ref record.rpblue);
                Datasheet.Parse(stream.NextString(), ref record.attred);
                Datasheet.Parse(stream.NextString(), ref record.damred);
                Datasheet.Parse(stream.NextString(), ref record.armred);
                Datasheet.Parse(stream.NextString(), ref record.rfred);
                Datasheet.Parse(stream.NextString(), ref record.rlred);
                Datasheet.Parse(stream.NextString(), ref record.rcred);
                Datasheet.Parse(stream.NextString(), ref record.rpred);
                Datasheet.Parse(stream.NextString(), ref record.exp);
                Datasheet.Parse(stream.NextString(), ref record.plrstaydeath);
                Datasheet.Parse(stream.NextString(), ref record.monstaydeath);
                Datasheet.Parse(stream.NextString(), ref record.bossstaydeath);
                Datasheet.Parse(stream.NextString(), ref record.hide);
                Datasheet.Parse(stream.NextString(), ref record.shatter);
                Datasheet.Parse(stream.NextString(), ref record.udead);
                Datasheet.Parse(stream.NextString(), ref record.life);
                Datasheet.Parse(stream.NextString(), ref record.green);
                Datasheet.Parse(stream.NextString(), ref record.pgsv);
                Datasheet.Parse(stream.NextString(), ref record.nooverlays);
                Datasheet.Parse(stream.NextString(), ref record.noclear);
                Datasheet.Parse(stream.NextString(), ref record.bossinv);
                Datasheet.Parse(stream.NextString(), ref record.meleeonly);
                Datasheet.Parse(stream.NextString(), ref record.notondead);
                Datasheet.Parse(stream.NextString(), ref record.overlay1);
                Datasheet.Parse(stream.NextString(), ref record.overlay2);
                Datasheet.Parse(stream.NextString(), ref record.overlay3);
                Datasheet.Parse(stream.NextString(), ref record.overlay4);
                Datasheet.Parse(stream.NextString(), ref record.pgsvoverlay);
                Datasheet.Parse(stream.NextString(), ref record.castoverlay);
                Datasheet.Parse(stream.NextString(), ref record.removerlay);
                Datasheet.Parse(stream.NextString(), ref record.stat);
                Datasheet.Parse(stream.NextString(), ref record.setfunc);
                Datasheet.Parse(stream.NextString(), ref record.remfunc);
                Datasheet.Parse(stream.NextString(), ref record.missile);
                Datasheet.Parse(stream.NextString(), ref record.skill);
                Datasheet.Parse(stream.NextString(), ref record.itemtype);
                Datasheet.Parse(stream.NextString(), ref record.itemtrans);
                Datasheet.Parse(stream.NextString(), ref record.colorpri);
                Datasheet.Parse(stream.NextString(), ref record.colorshift);
                Datasheet.Parse(stream.NextString(), ref record.lightr);
                Datasheet.Parse(stream.NextString(), ref record.lightg);
                Datasheet.Parse(stream.NextString(), ref record.lightb);
                Datasheet.Parse(stream.NextString(), ref record.onsound);
                Datasheet.Parse(stream.NextString(), ref record.offsound);
                Datasheet.Parse(stream.NextString(), ref record.gfxtype);
                Datasheet.Parse(stream.NextString(), ref record.gfxclass);
                Datasheet.Parse(stream.NextString(), ref record.cltevent);
                Datasheet.Parse(stream.NextString(), ref record.clteventfunc);
                Datasheet.Parse(stream.NextString(), ref record.cltactivefunc);
                Datasheet.Parse(stream.NextString(), ref record.srvactivefunc);
                Datasheet.Parse(stream.NextString(), ref record.eol);
    }
}
