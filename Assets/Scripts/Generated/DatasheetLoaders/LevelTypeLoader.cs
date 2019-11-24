
// It's generated file. DO NOT MODIFY IT!
class LevelTypeLoader : Datasheet.Loader<LevelType>
{
    public void LoadRecord(ref LevelType record, string[] values)
    {
        int index = 0;
                Datasheet.Parse(values[index++], ref record.name);
                Datasheet.Parse(values[index++], ref record.id);
                record.files = new string[32];
                    Datasheet.Parse(values[index++], ref record.files[0]);
                    Datasheet.Parse(values[index++], ref record.files[1]);
                    Datasheet.Parse(values[index++], ref record.files[2]);
                    Datasheet.Parse(values[index++], ref record.files[3]);
                    Datasheet.Parse(values[index++], ref record.files[4]);
                    Datasheet.Parse(values[index++], ref record.files[5]);
                    Datasheet.Parse(values[index++], ref record.files[6]);
                    Datasheet.Parse(values[index++], ref record.files[7]);
                    Datasheet.Parse(values[index++], ref record.files[8]);
                    Datasheet.Parse(values[index++], ref record.files[9]);
                    Datasheet.Parse(values[index++], ref record.files[10]);
                    Datasheet.Parse(values[index++], ref record.files[11]);
                    Datasheet.Parse(values[index++], ref record.files[12]);
                    Datasheet.Parse(values[index++], ref record.files[13]);
                    Datasheet.Parse(values[index++], ref record.files[14]);
                    Datasheet.Parse(values[index++], ref record.files[15]);
                    Datasheet.Parse(values[index++], ref record.files[16]);
                    Datasheet.Parse(values[index++], ref record.files[17]);
                    Datasheet.Parse(values[index++], ref record.files[18]);
                    Datasheet.Parse(values[index++], ref record.files[19]);
                    Datasheet.Parse(values[index++], ref record.files[20]);
                    Datasheet.Parse(values[index++], ref record.files[21]);
                    Datasheet.Parse(values[index++], ref record.files[22]);
                    Datasheet.Parse(values[index++], ref record.files[23]);
                    Datasheet.Parse(values[index++], ref record.files[24]);
                    Datasheet.Parse(values[index++], ref record.files[25]);
                    Datasheet.Parse(values[index++], ref record.files[26]);
                    Datasheet.Parse(values[index++], ref record.files[27]);
                    Datasheet.Parse(values[index++], ref record.files[28]);
                    Datasheet.Parse(values[index++], ref record.files[29]);
                    Datasheet.Parse(values[index++], ref record.files[30]);
                    Datasheet.Parse(values[index++], ref record.files[31]);
                Datasheet.Parse(values[index++], ref record.beta);
                Datasheet.Parse(values[index++], ref record.act);
    }
}
