using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    public class Loot : Entity
    {
        public Item item;
        
        private SpriteRenderer _renderer;
        private SpriteAnimator _animator;
        private bool _selected;

        public static Loot Create(Vector3 position, string flippyFile, string name, string title = null, int dir = 0)
        {
            position = Iso.MapToIso(position);
            if (!CollisionMap.Fit(position, out position, mask: CollisionLayers.Item))
            {
                Debug.LogError("Can't fit loot");
                return null;
            }
            position = Iso.MapToWorld(position);
            var gameObject = new GameObject(name);
            gameObject.transform.position = position;
            var palette = Palette.GetPalette(PaletteType.Act1);
            var spritesheet = DC6.Load(flippyFile, palette);
            var animator = gameObject.AddComponent<SpriteAnimator>();
            animator.SetSprites(spritesheet.GetSprites(dir));
            animator.loop = false;
            var loot = gameObject.AddComponent<Loot>();
            loot.title = title;
            return loot;
        }

        public static Loot Create(Vector3 position, Item item)
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
            var loot = Create(position, item.flippyFile, item.info.name, title, dir);
            loot.item = item;
            loot.Flip();
            return loot;
        }

        protected override void Awake()
        {
            base.Awake();
            CollisionMap.SetBlocked(Iso.MapToIso(transform.position), CollisionLayers.Item);
            _animator = GetComponent<SpriteAnimator>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnDisable()
        {
            CollisionMap.SetBlocked(Iso.MapToIso(transform.position), CollisionLayers.None);
        }

        protected override void Start()
        {
            base.Start();
            _renderer.sortingOrder = Iso.SortingOrder(transform.position);
        }

        public override bool selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    Materials.SetRendererHighlighted(_renderer, _selected);
                }
            }
        }

        public override Vector2 titleOffset => new Vector2(0, 20);

        public override Bounds bounds => _renderer.bounds;

        void Flip()
        {
            _animator.Restart();
            Events.InvokeLootFlipped(this);
        }

        public override void Operate(Unit unit)
        {
            if (item == null)
                Destroy(gameObject);

            if (PlayerController.instance.Take(item))
            {
                Destroy(gameObject);
            }
            else
            {
                Flip();
            }
        }
    }
}
