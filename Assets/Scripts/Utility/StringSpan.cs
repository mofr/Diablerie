public struct StringSpan
{
    public readonly string str;
    public readonly int index;
    public readonly int length;

    public StringSpan(string str, int index, int length)
    {
        this.str = str;
        this.index = index;
        this.length = length;
    }

    public override string ToString()
    {
        return str.Substring(index, length);
    }
}
