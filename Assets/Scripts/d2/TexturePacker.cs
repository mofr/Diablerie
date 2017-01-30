using System.Collections.Generic;
using UnityEngine;

class TexturePacker
{
    static Color32[] transparentColors = new Color32[2048 * 2048];

    public List<Texture2D> textures = new List<Texture2D>();

    Texture2D texture;
    int maxWidth;
    int maxHeight;

    int xPos = 0;
    int yPos = 0;
    int rowHeight = 0;

    public struct PackResult
    {
        public int x;
        public int y;
        public bool newTexture;
        public Texture2D texture;
    }

    public TexturePacker(int maxWidth, int maxHeight)
    {
        this.maxWidth = maxWidth;
        this.maxHeight = maxHeight;
    }

    private Texture2D CreateTexture()
    {
        var texture = new Texture2D(maxWidth, maxHeight, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels32(transparentColors);
        textures.Add(texture);
        return texture;
    }

    public PackResult put(int width, int height)
    {   
        rowHeight = Mathf.Max(height, rowHeight);
        if (xPos + width > maxWidth)
        {
            xPos = 0;
            yPos += rowHeight;
            rowHeight = height;
        }

        bool newTexture = texture == null || yPos + rowHeight > maxHeight;
        if (newTexture)
        {
            texture = CreateTexture();
            xPos = 0;
            yPos = 0;
            rowHeight = height;
        }

        var result = new PackResult();
        result.x = xPos;
        result.y = yPos;
        result.texture = texture;
        result.newTexture = newTexture;

        xPos += width;

        return result;
    }
}
