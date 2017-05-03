using UnityEngine;

public class Pickup : Entity
{
    new SpriteRenderer renderer;
    static MaterialPropertyBlock materialProperties;
    bool _selected = false;
    Item item;

    public static Pickup Create(Vector3 position, string flippyFile, string name, string title = null)
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
        pickup.title = title;
        return pickup;
    }

    public static Pickup Create(Vector3 position, Item item)
    {
        var title = item.info.name + ", " + COF.layerNames[item.info.component % COF.layerNames.Length] + " " + item.info.alternateGfx;
        var pickup = Create(position, item.info.flippyFile, item.info.name, title);
        pickup.item = item;
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
        var equip = character.GetComponent<Equipment>();
        if (item != null && item.info.type.body)
        {
            equip.Equip(item);
            var dc6 = DC6.Load(@"data\global\items\" + item.info.invFile + ".dc6", loadAllDirections: true);
            var texture = dc6.textures[0];
            var frame = dc6.directions[0].frames[0];
            var hotSpot = new Vector2(frame.width / 2, frame.height / 2);
            Cursor.SetCursor(texture, hotSpot, CursorMode.ForceSoftware);
        }
        Destroy(gameObject);
    }

    private void OnRenderObject()
    {
        MouseSelection.Submit(this);
    }
}
