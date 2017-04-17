public struct IntRect
{
    public static IntRect zero = new IntRect(0, 0, 0, 0);

    int _x;
    int _y;
    int _width;
    int _height;

    public IntRect(int x, int y, int width, int height)
    {
        _x = x;
        _y = y;
        _width = width;
        _height = height;
    }

    public int xMax
    {
        get
        {
            return _x + _width - 1;
        }

        set
        {
            _width = value - _x + 1;
        }
    }

    public int yMax
    {
        get
        {
            return _y;
        }

        set
        {
            int delta = _y - value;
            _height -= delta;
            _y = value;
        }
    }

    public int xMin
    {
        get
        {
            return _x;
        }

        set
        {
            int delta = value - _x;
            _width -= delta;
            _x = value;
        }
    }

    public int yMin
    {
        get
        {
            return _y - _height + 1;
        }

        set
        {
            _height = _y - value + 1;
        }
    }

    public int width
    {
        get
        {
            return _width;
        }

        set
        {
            _width = value;
        }
    }

    public int height
    {
        get
        {
            return _height;
        }

        set
        {
            _height = value;
        }
    }

    public bool Contains(int x, int y)
    {
        return x >= _x && y >= _y && x < _x + _width && y < _y + _height;
    }

    public string AsString()
    {
        return string.Format("({0}, {1})  --->  ({2}, {3})  =  {4} * {5}", xMin, yMin, xMax, yMax, width, height);
    }
}
