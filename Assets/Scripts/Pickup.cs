using UnityEngine;

public class Pickup : Entity
{
    new SpriteRenderer renderer;
    static MaterialPropertyBlock materialProperties;
    bool _selected = false;

    public static Pickup Create(Vector3 position, string flippyFile, string name)
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
        var spritesheet = DC6.Load(@"data\global\items\" + flippyFile + ".dc6");
        var animator = gameObject.AddComponent<SpriteAnimator>();
        animator.sprites = spritesheet.GetSprites(0);
        animator.loop = false;
        var pickup = gameObject.AddComponent<Pickup>();
        return pickup;
    }

    private void Awake()
    {
        if (materialProperties == null)
            materialProperties = new MaterialPropertyBlock();
        CollisionMap.SetPassable(Iso.MapToIso(transform.position), false);
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

    public override void Operate(Character character = null)
    {
        Destroy(gameObject);
    }

    private void OnRenderObject()
    {
        MouseSelection.Submit(this);
    }
}
