using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public int tileIndex;
    public IntRect triggerArea;
    public IntRect scanArea;
    public List<Renderer> walls = new List<Renderer>();
    public List<Renderer> roofs = new List<Renderer>();

    static MaterialPropertyBlock properties = new MaterialPropertyBlock();
    float t = 1;
    readonly float speed = 4;
    bool revealing = false;
    bool hiding = false;

    public static Popup Create(IntRect triggerArea, IntRect scanArea, int tileIndex)
    {
        var gameObject = new GameObject("Popup");
        gameObject.transform.position = Iso.MapTileToWorld(triggerArea.xMin, triggerArea.yMax);
        var popup = gameObject.AddComponent<Popup>();
        popup.triggerArea = triggerArea;
        popup.scanArea = scanArea;
        popup.tileIndex = tileIndex;
        var collider = gameObject.AddComponent<PolygonCollider2D>();
        var points = new Vector2[] {
            Iso.MapTileToWorld(0, 0),
            Iso.MapTileToWorld(triggerArea.width, 0),
            Iso.MapTileToWorld(triggerArea.width, triggerArea.height),
            Iso.MapTileToWorld(0, triggerArea.height)
        };
        collider.points = points;
        collider.isTrigger = true;
        collider.offset = Iso.MapToWorld(-2, -2);
        return popup;
    }

    void Update()
    {
        if (!revealing && !hiding)
            return;

        if (hiding)
            t += speed * Time.deltaTime;
        else
            t -= speed * Time.deltaTime;

        if (t > 1 || t < 0)
        {
            hiding = false;
            revealing = false;
            t = Mathf.Clamp01(t);
        }

        properties.SetColor("_Color", new Color(1, 1, 1, Mathf.SmoothStep(0, 1, t)));
        foreach (var renderer in roofs)
        {
            renderer.SetPropertyBlock(properties);
        }

        properties.SetColor("_Color", new Color(1, 1, 1, Mathf.SmoothStep(0.5f, 1, t)));
        foreach (var renderer in walls)
        {
            renderer.SetPropertyBlock(properties);
        }
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
        hiding = false;
        revealing = true;
    }

    void Hide()
    {
        hiding = true;
        revealing = false;
    }
}
