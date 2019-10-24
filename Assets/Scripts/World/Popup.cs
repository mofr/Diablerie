using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public int revealMainIndex;
    public IntRect triggerArea;
    public IntRect revealArea;
    public List<Renderer> roofs = new List<Renderer>();

    static MaterialPropertyBlock properties = new MaterialPropertyBlock();
    float t = 1;
    readonly float speed = 4;
    bool revealing = false;
    bool hiding = false;

    public static Popup Create(IntRect triggerArea, IntRect revealArea, int revealMainIndex)
    {
        var gameObject = new GameObject("Popup " + revealMainIndex);
        gameObject.transform.position = Iso.MapTileToWorld(triggerArea.xMin, triggerArea.yMax);
        var popup = gameObject.AddComponent<Popup>();
        popup.triggerArea = triggerArea;
        popup.revealArea = revealArea;
        popup.revealMainIndex = revealMainIndex;
        var collider = gameObject.AddComponent<PolygonCollider2D>();
        collider.points = Iso.CreateTileRectPoints(triggerArea.width, triggerArea.height);
        collider.isTrigger = true;
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
