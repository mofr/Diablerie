
// It's generated file. DO NOT MODIFY IT!
class SoundEnvironmentLoader : Datasheet.Loader<SoundEnvironment>
{

    public void LoadRecord(ref SoundEnvironment record, Datasheet.Stream stream)
    {
                Datasheet.Parse(stream.NextString(), ref record.handle);
                Datasheet.Parse(stream.NextString(), ref record.index);
                Datasheet.Parse(stream.NextString(), ref record.songId);
                Datasheet.Parse(stream.NextString(), ref record.dayAmbienceId);
                Datasheet.Parse(stream.NextString(), ref record.nightAmbienceId);
                Datasheet.Parse(stream.NextString(), ref record.dayEventId);
                Datasheet.Parse(stream.NextString(), ref record.nightEventId);
                Datasheet.Parse(stream.NextString(), ref record._eventDelay);
                Datasheet.Parse(stream.NextString(), ref record.indoors);
                Datasheet.Parse(stream.NextString(), ref record.material1);
                Datasheet.Parse(stream.NextString(), ref record.material2);
                Datasheet.Parse(stream.NextString(), ref record.EAXEnviron);
                Datasheet.Parse(stream.NextString(), ref record.EAXEnvSize);
                Datasheet.Parse(stream.NextString(), ref record.EAXEnvDiff);
                Datasheet.Parse(stream.NextString(), ref record.EAXRoomVol);
                Datasheet.Parse(stream.NextString(), ref record.EAXRoomHF);
                Datasheet.Parse(stream.NextString(), ref record.EAXDecayTime);
                Datasheet.Parse(stream.NextString(), ref record.EAXDecayHF);
                Datasheet.Parse(stream.NextString(), ref record.EAXReflect);
                Datasheet.Parse(stream.NextString(), ref record.EAXReflectDelay);
                Datasheet.Parse(stream.NextString(), ref record.EAXReverb);
                Datasheet.Parse(stream.NextString(), ref record.EAXRevDelay);
                Datasheet.Parse(stream.NextString(), ref record.EAXRoomRoll);
                Datasheet.Parse(stream.NextString(), ref record.EAXAirAbsorb);
    }
}
