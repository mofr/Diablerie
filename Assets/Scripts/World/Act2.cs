public class Act2 : Act
{
    public Act2()
    {
        var town = new LevelBuilder("Act 2 - Town");
        root = town.Instantiate(new Vector2i(0, 0));
        entry = town.FindEntry();
    }
}