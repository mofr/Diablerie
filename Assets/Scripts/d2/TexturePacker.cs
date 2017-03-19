using System.Collections.Generic;
using UnityEngine;

public class TexturePacker
{
    int maxWidth;
    int maxHeight;
    int padding = 0;

    int xPos = 0;
    int yPos = 0;
    int rowHeight = 0;

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
