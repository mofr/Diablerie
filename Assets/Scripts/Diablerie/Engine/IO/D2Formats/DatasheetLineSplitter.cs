using Diablerie.Engine.Utility;

namespace Diablerie.Engine.IO.D2Formats
{
    public class DatasheetLineSplitter
    {
        private readonly string _str;
        private int _index;

        public DatasheetLineSplitter(string str)
        {
            _str = str;
            _index = 0;
        }

        public bool ReadLine(ref StringSpan result)
        {
            if (_index >= _str.Length)
                return false;
            int startIndex = _index;
            int length = ReadLine();
            result = new StringSpan(_str, startIndex, length);
            return true;
        }

        private int ReadLine()
        {
            int length = 0;
            while (_index < _str.Length && !IsSeparator(_str[_index]))
            {
                length++;
                _index++;
            }
            while (_index < _str.Length && IsSeparator(_str[_index]))
            {
                _index++;
            }
            return length;
        }

        public void Skip(int count)
        {
            for (int i = 0; i < count; ++i)
                ReadLine();
        }

        private bool IsSeparator(char c)
        {
            return c == '\n' || c == '\r';
        }
    }
}
