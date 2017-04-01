using UnityEngine;

public struct TexturePacker
{
    int maxWidth;
    int maxHeight;
    int padding;

    int xPos;
    int yPos;
    int rowHeight;

    public struct PackResult
    {
        public int x;
        public int y;
        public bool newTexture;
    }

    public TexturePacker(int maxWidth, int maxHeight, int padding = 0)
    {
        this.maxWidth = maxWidth;
        this.maxHeight = maxHeight;
        this.padding = padding;
        xPos = 0;
        yPos = 0;
        rowHeight = 0;
    }

    public PackResult put(int width, int height)
    {
        width += padding;
        height += padding;
        rowHeight = Mathf.Max(height, rowHeight);
        if (xPos + width > maxWidth)
        {
            xPos = 0;
            yPos += rowHeight;
            rowHeight = height;
        }

        if (yPos + rowHeight > maxHeight)
        {
            xPos = 0;
            yPos = 0;
            rowHeight = height;
        }

        var result = new PackResult();
        result.x = xPos;
        result.y = yPos;
        result.newTexture = xPos == 0 && yPos == 0;

        xPos += width;

        return result;
    }
}
