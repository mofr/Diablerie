public class Act4 : Act
{
    public Act4()
    {
        var town = new LevelBuilder("Act 4 - Town");
        root = town.Instantiate(new Vector2i(0, 0));
        entry = town.FindEntry();
    }
}