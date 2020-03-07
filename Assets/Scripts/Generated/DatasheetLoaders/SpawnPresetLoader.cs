
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class SpawnPresetLoader : Datasheet.Loader<SpawnPreset>
{

    public void LoadRecord(ref SpawnPreset record, DatasheetStream stream)
    {
                stream.Read(ref record.act);
                stream.Read(ref record.type);
                stream.Read(ref record.id);
                stream.Read(ref record.description);
                stream.Read(ref record.objectId);
                stream.Read(ref record.monstatId);
                stream.Read(ref record.direction);
                stream.Read(ref record._base);
                stream.Read(ref record.token);
                stream.Read(ref record.modeToken);
                stream.Read(ref record.weaponClass);
                record.gear = new string[16];
                    stream.Read(ref record.gear[0]);
                    stream.Read(ref record.gear[1]);
                    stream.Read(ref record.gear[2]);
                    stream.Read(ref record.gear[3]);
                    stream.Read(ref record.gear[4]);
                    stream.Read(ref record.gear[5]);
                    stream.Read(ref record.gear[6]);
                    stream.Read(ref record.gear[7]);
                    stream.Read(ref record.gear[8]);
                    stream.Read(ref record.gear[9]);
                    stream.Read(ref record.gear[10]);
                    stream.Read(ref record.gear[11]);
                    stream.Read(ref record.gear[12]);
                    stream.Read(ref record.gear[13]);
                    stream.Read(ref record.gear[14]);
                    stream.Read(ref record.gear[15]);
                stream.Read(ref record.colormap);
                stream.Read(ref record.index);
                stream.Read(ref record.eol);
    }
}
