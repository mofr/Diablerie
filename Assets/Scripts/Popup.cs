using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public IntRect rect;
    public List<Renderer> renderers = new List<Renderer>();
    static MaterialPropertyBlock properties = new MaterialPropertyBlock();

    public static Popup Create(IntRect rect)
    {
        var gameObject = new GameObject("Popup");
        gameObject.transform.position = Iso.MapTileToWorld(rect.xMin, rect.yMax);
        var popup = gameObject.AddComponent<Popup>();
        popup.rect = rect;
        var collider = gameObject.AddComponent<PolygonCollider2D>();
        var points = new Vector2[] {
            Iso.MapTileToWorld(0, 0),
            Iso.MapTileToWorld(rect.width - 1, 0),
            Iso.MapTileToWorld(rect.width - 1, rect.height - 1),
            Iso.MapTileToWorld(0, rect.height - 1)
        };
        collider.points = points;
        collider.isTrigger = true;
        collider.offset = Iso.MapToWorld(-2, -2);
        return popup;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            Reveal();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            Hide();
    }

    void Reveal()
    {
        properties.SetColor("_Color", new Color32(255, 255, 255, 64));
        foreach (var renderer in renderers)
        {
            renderer.SetPropertyBlock(properties);
        }
    }

    void Hide()
    {
        properties.SetColor("_Color", new Color32(255, 255, 255, 255));
        foreach (var renderer in renderers)
        {
            renderer.SetPropertyBlock(properties);
        }
    }
}
