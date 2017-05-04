public class Item
{
    public readonly ItemInfo info;

    static public Item Create(string code)
    {
        var info = ItemInfo.Find(code);
        if (info == null)
            return null;

        return new Item(info);
    }

    public Item(ItemInfo info)
    {
        this.info = info;
    }
}
