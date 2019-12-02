using System;
using System.Globalization;
using Diablerie.Engine.Utility;

namespace Diablerie.Engine.IO.D2Formats
{
    public class DatasheetStream
    {
        private static readonly char Separator = '\t';
        
        private string _data;
        private int _index;
        private int _end;

        public void SetSource(StringSpan span)
        {
            _data = span.str;
            _index = span.index;
            _end = span.index + span.length;
        }

        public string NextString()
        {
            int endIndex = _index;
            while (endIndex < _end && _data[endIndex] != Separator)
                endIndex++;
            int length = endIndex - _index;
            string result = null;
            if (length != 0)
                result = _data.Substring(_index, length);
            _index = endIndex + 1;
            return result;
        }

        public void Read(ref int result)
        {
            if (_index >= _end)
            {
                return;
            }
            if (_data[_index] == Separator)
            {
                _index++;
                return;
            }
            int sign;
            char c = _data[_index++];
            if (c == '-')
            {
                sign = -1;
                result = 0;
            }
            else
            {
                sign = 1;
                result = c - '0';
            }

            while (_index < _end && _data[_index] != Separator)
            {
                result = result * 10 + (_data[_index] - '0');
                _index++;
            }

            result *= sign;
            _index++; // skip tab
        }

        public void Read(ref uint result)
        {
            if (_index >= _end)
            {
                return;
            }
            if (_data[_index] == Separator)
            {
                _index++;
                return;
            }

            if (_index < _end && _data[_index] != Separator)
                result = 0;
            while (_index < _end && _data[_index] != Separator)
            {
                result = result * 10 + (uint)(_data[_index] - '0');
                _index++;
            }

            _index++; // skip tab
        }
        
        public void Read(ref string result)
        {
            var value = NextString();
            if (value == null || value == "xxx")
                return;
            result = value;
        }

        public void Read(ref bool result)
        {
            if (_index >= _end)
            {
                return;
            }
            if (_data[_index] == Separator)
            {
                _index++;
                return;
            }
            
            result = _data[_index] != '0';
            _index += 2; // skip tab
        }

        public void Read(ref float result)
        {
            var value = NextString();
            if (value == null || value == "xxx")
                return;
            result = (float) Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
    }
}