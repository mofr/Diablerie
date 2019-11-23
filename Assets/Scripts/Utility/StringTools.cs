public class StringTools
{
    public static int ParseInt(string str)
    {
        if (str.Length == 0)
            return 0;
        int result;
        int sign;
        if (str[0] == '-')
        {
            sign = -1;
            result = 0;
        }
        else
        {
            sign = 1;
            result = str[0] - '0';
        }

        for (int i = 1; i < str.Length; i++)
        {
            result = result * 10 + (str[i] - '0');
        }

        return result * sign;
    }

    public static uint ParseUInt(string str)
    {
        uint result = 0;
        for (int i = 0; i < str.Length; i++)
        {
            result = result * 10 + (uint)(str[i] - '0');
        }
        return result;
    }
}
