using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Game.World;
using UnityEngine;

namespace Diablerie.Engine
{
    public class Player
    {
        public GameObject gameObject;
        public Transform transform;
        public Character character;
        public Equipment equip;
        public Inventory inventory;
        public CharStat charStat;
        
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

        public Player(string className, Vector3 pos)
        {
            var info = CharStatsInfo.Find(className);
            gameObject = new GameObject("Player");
            transform = gameObject.transform;
            transform.position = pos;
            gameObject.tag = "Player";
            character = gameObject.AddComponent<Character>();
            character.basePath = @"data\global\chars";
            character.token = info.token;
            character.weaponClass = info.baseWClass;
            character.run = true;
            character.walkSpeed = 7;
            character.runSpeed = 15;
            character.maxHealth = 1000;
            character.health = 1000;
            character.size = 2;
            character.party = Party.Good;

            equip = gameObject.AddComponent<Equipment>();
            character.equip = equip;
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
            character.charStat = charStat;
            character.charStat.character = character;
            character.charStat.info = info;

            foreach (var startingItem in info.startingItems)
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
                    var pos = Iso.MapToWorld(character.iso.pos);
                    var teleport = WorldBuilder.SpawnObject("TP", pos, fit: true);
                    teleport.modeName = "OP";
                    var sound = SoundInfo.Find("player_townportal_cast");
                    AudioManager.instance.Play(sound, pos);
                    break;
                case MiscInfo.UseFunction.Potion:
                    if (itemInfo.stat1 == "hpregen")
                        character.health += itemInfo.calc1;
                    if (itemInfo.stat1 == "manarecovery")
                        character.mana += itemInfo.calc1;
                    break;
                case MiscInfo.UseFunction.RejuvPotion:
                    if (itemInfo.stat1 == "hitpoints")
                        character.health += (int)(itemInfo.calc1 / 100.0f * character.maxHealth);
                    if (itemInfo.stat1 == "mana")
                        character.mana += (int)(itemInfo.calc1 / 100.0f * character.maxMana);
                    if (itemInfo.stat2 == "hitpoints")
                        character.health += (int)(itemInfo.calc2 / 100.0f * character.maxHealth);
                    if (itemInfo.stat2 == "mana")
                        character.mana += (int)(itemInfo.calc2 / 100.0f * character.maxMana);
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