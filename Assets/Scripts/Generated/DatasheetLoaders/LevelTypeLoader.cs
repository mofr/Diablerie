
// It's generated file. DO NOT MODIFY IT!
class LevelTypeLoader : Datasheet.Loader<LevelType>
{

    public void LoadRecord(ref LevelType record, Datasheet.Stream stream)
    {
                stream.Read(ref record.name);
                stream.Read(ref record.id);
                record.files = new string[32];
                    stream.Read(ref record.files[0]);
                    stream.Read(ref record.files[1]);
                    stream.Read(ref record.files[2]);
                    stream.Read(ref record.files[3]);
                    stream.Read(ref record.files[4]);
                    stream.Read(ref record.files[5]);
                    stream.Read(ref record.files[6]);
                    stream.Read(ref record.files[7]);
                    stream.Read(ref record.files[8]);
                    stream.Read(ref record.files[9]);
                    stream.Read(ref record.files[10]);
                    stream.Read(ref record.files[11]);
                    stream.Read(ref record.files[12]);
                    stream.Read(ref record.files[13]);
                    stream.Read(ref record.files[14]);
                    stream.Read(ref record.files[15]);
                    stream.Read(ref record.files[16]);
                    stream.Read(ref record.files[17]);
                    stream.Read(ref record.files[18]);
                    stream.Read(ref record.files[19]);
                    stream.Read(ref record.files[20]);
                    stream.Read(ref record.files[21]);
                    stream.Read(ref record.files[22]);
                    stream.Read(ref record.files[23]);
                    stream.Read(ref record.files[24]);
                    stream.Read(ref record.files[25]);
                    stream.Read(ref record.files[26]);
                    stream.Read(ref record.files[27]);
                    stream.Read(ref record.files[28]);
                    stream.Read(ref record.files[29]);
                    stream.Read(ref record.files[30]);
                    stream.Read(ref record.files[31]);
                stream.Read(ref record.beta);
                stream.Read(ref record.act);
    }
}
