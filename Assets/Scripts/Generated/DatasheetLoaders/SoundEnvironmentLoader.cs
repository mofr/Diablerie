
// It's generated file. DO NOT MODIFY IT!

using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;

class SoundEnvironmentLoader : Datasheet.Loader<SoundEnvironment>
{

    public void LoadRecord(ref SoundEnvironment record, DatasheetStream stream)
    {
                stream.Read(ref record.handle);
                stream.Read(ref record.index);
                stream.Read(ref record.songId);
                stream.Read(ref record.dayAmbienceId);
                stream.Read(ref record.nightAmbienceId);
                stream.Read(ref record.dayEventId);
                stream.Read(ref record.nightEventId);
                stream.Read(ref record._eventDelay);
                stream.Read(ref record.indoors);
                stream.Read(ref record.material1);
                stream.Read(ref record.material2);
                stream.Read(ref record.EAXEnviron);
                stream.Read(ref record.EAXEnvSize);
                stream.Read(ref record.EAXEnvDiff);
                stream.Read(ref record.EAXRoomVol);
                stream.Read(ref record.EAXRoomHF);
                stream.Read(ref record.EAXDecayTime);
                stream.Read(ref record.EAXDecayHF);
                stream.Read(ref record.EAXReflect);
                stream.Read(ref record.EAXReflectDelay);
                stream.Read(ref record.EAXReverb);
                stream.Read(ref record.EAXRevDelay);
                stream.Read(ref record.EAXRoomRoll);
                stream.Read(ref record.EAXAirAbsorb);
    }
}
