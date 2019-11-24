
// It's generated file. DO NOT MODIFY IT!
class SpawnPresetLoader : Datasheet.Loader<SpawnPreset>
{

    public void LoadRecord(ref SpawnPreset record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.act);
                Datasheet.Parse(stream.NextString(), ref record.type);
                Datasheet.Parse(stream.NextString(), ref record.id);
                Datasheet.Parse(stream.NextString(), ref record.description);
                Datasheet.Parse(stream.NextString(), ref record.objectId);
                Datasheet.Parse(stream.NextString(), ref record.monstatId);
                Datasheet.Parse(stream.NextString(), ref record.direction);
                Datasheet.Parse(stream.NextString(), ref record._base);
                Datasheet.Parse(stream.NextString(), ref record.token);
                Datasheet.Parse(stream.NextString(), ref record.mode);
                Datasheet.Parse(stream.NextString(), ref record.weaponClass);
                record.gear = new string[16];
                    Datasheet.Parse(stream.NextString(), ref record.gear[0]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[1]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[2]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[3]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[4]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[5]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[6]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[7]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[8]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[9]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[10]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[11]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[12]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[13]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[14]);
                    Datasheet.Parse(stream.NextString(), ref record.gear[15]);
                Datasheet.Parse(stream.NextString(), ref record.colormap);
                Datasheet.Parse(stream.NextString(), ref record.index);
                Datasheet.Parse(stream.NextString(), ref record.eol);
    }
}
