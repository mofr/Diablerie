using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public bool loop = true;

    new SpriteRenderer renderer;
    Sprite[] _sprites;
    float time = 0;
    float fps = 25;

    public Sprite[] sprites
    {
        get
        {
            return _sprites;
        }

        set
        {
            _sprites = value;
        }
    }

    public void Restart()
    {
        time = 0;
    }

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.material = Materials.normal;
    }

    void Update()
    {
        if (_sprites == null)
            return;

        int frameCounter = (int)(time * fps);
        if (frameCounter >= sprites.Length)
        {
            if (!loop)
                return;

            frameCounter = frameCounter % sprites.Length;
        }

        renderer.sprite = _sprites[frameCounter];
        time += Time.deltaTime;
    }
}
