public class Act5 : Act
{
    public Act5()
    {
        var town = new LevelBuilder("Act 5 - Town");
        root = town.Instantiate(new Vector2i(0, 0));
        entry = town.FindEntry();
    }
}