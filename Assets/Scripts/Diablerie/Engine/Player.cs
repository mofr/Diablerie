using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.Utility;
using Diablerie.Game.World;
using UnityEngine;

namespace Diablerie.Engine
{
    public class Player
    {
        public GameObject gameObject;
        public Transform transform;
        public Unit unit;
        public Equipment equip;
        public Inventory inventory;
        public CharStat charStat;
        public CharStatsInfo charStatInfo;
        
        private Item _handsItem;

        public Item HandsItem
        {
            get => _handsItem;
            set
            {
                if (_handsItem == value)
                    return;
                _handsItem = value;
                HandsItemChanged?.Invoke(_handsItem);
            }
        }

        public delegate void OnHandsItemChangedHandler(Item item);
        public event OnHandsItemChangedHandler HandsItemChanged;

        public Player(string className, Vector2i pos)
        {
            charStatInfo = CharStatsInfo.Find(className);
            gameObject = new GameObject("Player");
            transform = gameObject.transform;
            transform.position = Iso.MapTileToWorld(pos);
            gameObject.tag = "Player";
            unit = gameObject.AddComponent<Unit>();
            unit.basePath = @"data\global\chars";
            unit.token = charStatInfo.token;
            unit.weaponClass = charStatInfo.baseWClass;
            unit.run = true;
            unit.walkSpeed = 7;
            unit.runSpeed = 15;
            unit.maxHealth = 1000;
            unit.health = 1000;
            unit.size = 2;
            unit.party = Party.Good;

            equip = gameObject.AddComponent<Equipment>();
            equip.charInfo = charStatInfo;
            unit.equip = equip;
            inventory = Inventory.Create(gameObject, 10, 4);
            var body = gameObject.AddComponent<Rigidbody2D>();
            body.isKinematic = true;
            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = Iso.tileSizeY;
            var listenerObject = new GameObject("Audio Listener");
            listenerObject.AddComponent<AudioListener>();
            listenerObject.transform.SetParent(gameObject.transform, true);
            listenerObject.transform.localPosition = new Vector3(0, 0, -1);
            charStat = gameObject.AddComponent<CharStat>();
            charStat.unit = unit;

            foreach (var startingItem in charStatInfo.startingItems)
            {
                if (startingItem.code == null)
                    continue;

                var itemInfo = ItemInfo.Find(startingItem.code);
                for (int i = 0; i < startingItem.count; ++i)
                {
                    var item = new Item(itemInfo);
                    if (startingItem.loc != null)
                    {
                        int loc = BodyLoc.GetIndex(startingItem.loc);
                        equip.Equip(item, loc);
                    }
                    else
                    {
                        Take(item);
                    }
                }
            }
        }
        
        public bool Take(Item item, bool preferHands = false)
        {
            if (item.info.code == "gld")
            {
                inventory.gold += item.quantity;
                return true;
            }

            if (preferHands)
            {
                HandsItem = item;
                return true;
            }

            if (equip.Equip(item))
                return true;

            return inventory.Put(item);
        }

        public void Use(MiscInfo itemInfo)
        {
            Debug.Log("Use item " + itemInfo.name + ", function: " + itemInfo.useFunction);
            switch (itemInfo.useFunction)
            {
                case MiscInfo.UseFunction.None:
                    break;
                case MiscInfo.UseFunction.IdentifyItem:
                    break;
                case MiscInfo.UseFunction.TownPortal:
                    var pos = Iso.MapToWorld(unit.iso.pos);
                    var teleport = WorldBuilder.SpawnObject("TP", pos, fit: true);
                    teleport.modeName = "OP";
                    var sound = SoundInfo.Find("player_townportal_cast");
                    AudioManager.instance.Play(sound, pos);
                    break;
                case MiscInfo.UseFunction.Potion:
                    if (itemInfo.stat1 == "hpregen")
                        unit.health += itemInfo.calc1;
                    if (itemInfo.stat1 == "manarecovery")
                        unit.mana += itemInfo.calc1;
                    break;
                case MiscInfo.UseFunction.RejuvPotion:
                    if (itemInfo.stat1 == "hitpoints")
                        unit.health += (int)(itemInfo.calc1 / 100.0f * unit.maxHealth);
                    if (itemInfo.stat1 == "mana")
                        unit.mana += (int)(itemInfo.calc1 / 100.0f * unit.maxMana);
                    if (itemInfo.stat2 == "hitpoints")
                        unit.health += (int)(itemInfo.calc2 / 100.0f * unit.maxHealth);
                    if (itemInfo.stat2 == "mana")
                        unit.mana += (int)(itemInfo.calc2 / 100.0f * unit.maxMana);
                    break;
                case MiscInfo.UseFunction.TemporaryPotion:
                    break;
                case MiscInfo.UseFunction.HoradricCube:
                    break;
                case MiscInfo.UseFunction.Elixir:
                    break;
                case MiscInfo.UseFunction.StaminaPotion:
                    break;
            }
        }
    }
}