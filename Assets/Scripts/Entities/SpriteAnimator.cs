using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public bool loop = true;
    public float fps = 25;

    new SpriteRenderer renderer;
    Sprite[] _sprites;
    float time = 0;
    bool _finished = false;

    public bool finished
    {
        get
        {
            return _finished;
        }
    }

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
        _finished = false;
    }

    private void Awake()
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
            {
                _finished = true;
                return;
            }

            frameCounter = frameCounter % sprites.Length;
        }

        renderer.sprite = _sprites[frameCounter];
        time += Time.deltaTime;
    }
}
