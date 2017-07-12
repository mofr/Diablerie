using UnityEngine;

public class Pickup : Entity
{
    new SpriteRenderer renderer;
    SpriteAnimator animator;
    static MaterialPropertyBlock materialProperties;
    bool _selected = false;
    Item item;

    public static Pickup Create(Vector3 position, string flippyFile, string name, string title = null, int dir = 0)
    {
        position = Iso.MapToIso(position);
        if (!CollisionMap.Fit(position, out position))
        {
            Debug.LogError("Can't fit pickup");
            return null;
        }
        position = Iso.MapToWorld(position);
        var gameObject = new GameObject(name);
        gameObject.transform.position = position;
        var spritesheet = DC6.Load(flippyFile);
        var animator = gameObject.AddComponent<SpriteAnimator>();
        animator.sprites = spritesheet.GetSprites(dir);
        animator.loop = false;
        var pickup = gameObject.AddComponent<Pickup>();
        pickup.title = title;
        return pickup;
    }

    public static Pickup Create(Vector3 position, Item item)
    {
        var title = item.GetTitle();
        int dir = 0;
        if (item.info.code == "gld")
        {
            if (item.quantity >= 5000)
                dir = 3;
            else if (item.quantity >= 500)
                dir = 2;
            else if (item.quantity >= 100)
                dir = 1;
        }
        var pickup = Create(position, item.flippyFile, item.info.name, title, dir);
        pickup.item = item;
        pickup.animator.SetTrigger(item.dropSoundFrame, () => {
            AudioManager.instance.Play(item.dropSound, pickup.transform.position);
        });
        pickup.Flip();
        return pickup;
    }

    private void Awake()
    {
        if (materialProperties == null)
            materialProperties = new MaterialPropertyBlock();
        CollisionMap.SetPassable(Iso.MapToIso(transform.position), false);
        animator = GetComponent<SpriteAnimator>();
    }

    private void OnDisable()
    {
        CollisionMap.SetPassable(Iso.MapToIso(transform.position), true);
    }

    protected override void Start()
    {
        base.Start();
        renderer = GetComponent<SpriteRenderer>();
        renderer.sortingOrder = Iso.SortingOrder(transform.position);
    }

    public override bool selected
    {
        get { return _selected; }
        set
        {
            if (_selected != value)
            {
                _selected = value;
                Materials.SetRendererHighlighted(renderer, _selected);
            }
        }
    }

    public override Vector2 titleOffset
    {
        get { return new Vector2(0, 24); }
    }

    public override Bounds bounds
    {
        get { return renderer.bounds; }
    }

    void Flip()
    {
        AudioManager.instance.Play(SoundInfo.itemFlippy, transform.position);
        animator.Restart();
    }

    public override void Operate(Character character = null)
    {
        if (item == null)
            Destroy(gameObject);

        if (PlayerController.instance.Take(item))
        {
            AudioManager.instance.Play(SoundInfo.itemPickup);
            Destroy(gameObject);
        }
        else
        {
            // I can't!
            Flip();
        }
    }

    private void OnRenderObject()
    {
        MouseSelection.Submit(this);
    }
}
