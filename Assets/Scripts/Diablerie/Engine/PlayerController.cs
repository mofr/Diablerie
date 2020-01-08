using System.Collections.Generic;
using System.Linq;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.UI;
using Diablerie.Game.UI;
using Diablerie.Game.UI.Inventory;
using Diablerie.Game.UI.SkillPanel;
using Diablerie.Game.World;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Diablerie.Engine
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;

        public Character character;
        public Equipment equip;
        public Inventory inventory;
        public CharStat charStat;
        public CameraController cameraController;

        private bool flush = false;
        private Iso iso;
        private Item _mouseItem;
        private bool usingSkills = false;
        private bool run = true;
    
        private List<SkillInfo> availableSkills;
        private List<KeyCode> hotSkillsBindings = new List<KeyCode> {
            KeyCode.F1,
            KeyCode.F2,
            KeyCode.F3,
            KeyCode.F4,
            KeyCode.F5,
            KeyCode.F6,
        };
        private List<SkillInfo> hotSkills;
        private SkillInfo leftSkill;
        private SkillInfo rightSkill;

        private int selectingSkillIndex = 0;

        void Awake()
        {
            instance = this;

            if (character == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null)
                    SetCharacter(player.GetComponent<Character>());
            }

            hotSkills = new List<SkillInfo>();
            hotSkills.AddRange(Enumerable.Repeat((SkillInfo)null, 6));
            availableSkills = new List<SkillInfo> {
                // Sorceress
                SkillInfo.Find("Fire Bolt"),
                SkillInfo.Find("Charged Bolt"),
                SkillInfo.Find("Frost Nova"),
                SkillInfo.Find("Teleport"),
                SkillInfo.Find("Nova"),
                SkillInfo.Find("Frozen Orb"),
                SkillInfo.Find("Poison Nova"),
                SkillInfo.Find("Glacial Spike"),
                SkillInfo.Find("Ice Bolt"),
                SkillInfo.Find("Ice Blast"),
                SkillInfo.Find("Fire Ball"),
                // Barbarian
                SkillInfo.Find("Shout"),
                SkillInfo.Find("Battle Cry"),
                SkillInfo.Find("Battle Orders"),
                SkillInfo.Find("Battle Command"),
                SkillInfo.Find("War Cry"),
                // Necromancer
                SkillInfo.Find("Raise Skeleton"),
                SkillInfo.Find("Raise Skeletal Mage"),
                SkillInfo.Find("Clay Golem"),
                SkillInfo.Find("IronGolem"),
                SkillInfo.Find("FireGolem"),
                SkillInfo.Find("BloodGolem"),
                // Druid
                SkillInfo.Find("Raven"),
                SkillInfo.Find("Summon Spirit Wolf"),
                SkillInfo.Find("Summon Fenris"),
                SkillInfo.Find("Summon Grizzly"),
                SkillInfo.Find("Oak Sage"),
                SkillInfo.Find("Heart of Wolverine"),
                SkillInfo.Find("Spirit of Barbs"),
                // Assassin
                SkillInfo.Find("Charged Bolt Sentry"),
                SkillInfo.Find("Wake of Fire Sentry"),
                SkillInfo.Find("Lightning Sentry"),
                SkillInfo.Find("Inferno Sentry"),
                SkillInfo.Find("Death Sentry"),
            };
            
            leftSkill = SkillInfo.Attack;
            rightSkill = SkillInfo.Attack;
        }

        void Start()
        {
            SkillPanel.instance.OnClick += index =>
            {
                selectingSkillIndex = index;
                AvailableSkillsPanel.instance.Toggle();
            };
            AvailableSkillsPanel.instance.OnSkillSelected += skillInfo =>
            {
                SetHotSkill(selectingSkillIndex, skillInfo);
            };
            for (int i = 0; i < hotSkills.Count; ++i)
            {
                SkillPanel.instance.SetHotKey(i, hotSkillsBindings[i].ToString());
            }

            foreach (var skillInfo in availableSkills)
            {
                AvailableSkillsPanel.instance.AddSkill(skillInfo);
            }
            AvailableSkillsPanel.instance.Hide();  // TODO This line affects newly added skills with AddSkill, fix it

            SetHotSkill(0, SkillInfo.Find("Fire Bolt"));
            SetHotSkill(1, SkillInfo.Find("Raise Skeleton"));
            SetHotSkill(2, SkillInfo.Find("Frost Nova"));
            SetHotSkill(3, SkillInfo.Find("Teleport"));
            SetHotSkill(4, SkillInfo.Find("Nova"));
            SetHotSkill(5, SkillInfo.Find("Frozen Orb"));
        }

        public void FlushInput()
        {
            flush = true;
        }

        public void SetHotSkill(int index, SkillInfo skillInfo)
        {
            hotSkills[index] = skillInfo;
            SkillPanel.instance.SetHotSkill(index, skillInfo);
        }

        public void SetCharacter(Character character)
        {
            this.character = character;
            iso = character.GetComponent<Iso>();
            equip = character.GetComponent<Equipment>();
            inventory = character.GetComponent<Inventory>();
            charStat = character.GetComponent<CharStat>();
            InventoryPanel.instance.equip = equip;
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
                    var pos = Iso.MapToWorld(iso.pos);
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

        public bool Take(Item item)
        {
            if (item.info.code == "gld")
            {
                inventory.gold += item.quantity;
                return true;
            }

            if (InventoryPanel.instance.visible)
            {
                mouseItem = item;
                return true;
            }

            if (equip.Equip(item))
                return true;

            return inventory.Put(item);
        }

        public Item mouseItem
        {
            get { return _mouseItem; }
            set
            {
                if (_mouseItem == value)
                    return;

                _mouseItem = value;

                if (_mouseItem != null)
                {
                    var dc6 = DC6.Load(_mouseItem.invFile, loadAllDirections: true);
                    var texture = dc6.textures[0];
                    var frame = dc6.directions[0].frames[0];
                    var hotSpot = new Vector2(frame.width / 2, frame.height / 2);
                
                    SoftwareCursor.SetCursor(texture, hotSpot);
                    Cursor.visible = false;
                }
                else
                {
                    SoftwareCursor.SetCursor(null);
                    Cursor.visible = true;
                }
            }
        }

        void DrawDebugPath()
        {
            Vector3 targetPosition;
            if (MouseSelection.instance.HotEntity != null)
            {
                targetPosition = Iso.MapToIso(MouseSelection.instance.HotEntity.transform.position);
            }
            else
            {
                targetPosition = IsoInput.mousePosition;
            }
            var path = Pathing.BuildPath(iso.pos, targetPosition, size: character.size, self: iso.gameObject);
            Pathing.DebugDrawPath(iso.pos, path);
        }

        void UpdateCamera()
        {
            float cameraShift = 0;
            if (InventoryPanel.instance.visible)
                cameraShift += 0.5f;
            if (CharstatPanel.instance.visible)
                cameraShift -= 0.5f;
            cameraController.horizontalShift = cameraShift;
        }

        void HandleKeyboard()
        {
            bool highlightPickups = Input.GetKey(KeyCode.LeftAlt) | Input.GetKey(KeyCode.RightAlt);
            MouseSelection.instance.SetHighlightPickups(highlightPickups);
            if (InventoryPanel.instance.visible || CharstatPanel.instance.visible)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
                {
                    InventoryPanel.instance.visible = false;
                    CharstatPanel.instance.visible = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameMenu.Show();
            }

            if (!CommandPrompt.instance.visible)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    CommandPrompt.instance.visible = true;
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    InventoryPanel.instance.visible ^= true;
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    CharstatPanel.instance.visible ^= true;
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    run ^= true;
                }
            
                // following section serves debugging purposes only
                if (Input.GetKeyDown(KeyCode.T))
                {
                    for (int i = 0; i < 1; ++i)
                    {
                        var tc = TreasureClass.sheet[Random.Range(0, TreasureClass.sheet.Count)];
                        if (tc.name == null)
                            continue;
                        int itemLevel = Random.Range(50, 100);
                        ItemDrop.Drop(tc.name, Iso.MapToWorld(IsoInput.mouseTile), itemLevel);
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    CommandPrompt.instance.visible = false;
                    CommandPrompt.instance.Execute();
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CommandPrompt.instance.visible = false;
                }
            }
        }

        void ControlCharacter()
        {
            if (character == null)
                return;

            DrawDebugPath();

            if (_mouseItem != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Pickup.Create(character.transform.position, _mouseItem);
                    FlushInput();
                    mouseItem = null;
                }
                return;
            }

            usingSkills = false;
            for (int i = 0; i < hotSkills.Count; ++i)
            {
                SkillInfo skill = hotSkills[i];
                KeyCode key = hotSkillsBindings[i];
                if (skill == null || !Input.GetKey(key))
                    continue;
            
                usingSkills = true;
                if (MouseSelection.instance.HotEntity != null)
                {
                    var targetCharacter = MouseSelection.instance.HotEntity.GetComponent<Character>();
                    if (targetCharacter != null)
                        character.UseSkill(skill, targetCharacter);
                    else
                        character.UseSkill(skill, Iso.MapToIso(MouseSelection.instance.HotEntity.transform.position));
                }
                else
                {
                    character.UseSkill(skill, IsoInput.mousePosition);
                }
            }

            if (!usingSkills)
            {
                if (Input.GetMouseButton(1) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)))
                {
                    character.UseSkill(rightSkill, IsoInput.mousePosition);
                }
                else if (Input.GetMouseButton(0))
                {
                    if (MouseSelection.instance.HotEntity != null)
                    {
                        var targetCharacter = MouseSelection.instance.HotEntity.GetComponent<Character>();
                        if (targetCharacter != null)
                        {
                            if (targetCharacter.monStat != null && targetCharacter.monStat.npc)
                            {
                                // todo interact with npc
                            }
                            else
                            {
                                character.UseSkill(leftSkill, targetCharacter);
                            }
                        }
                        else
                        {
                            character.Use(MouseSelection.instance.HotEntity);
                        }
                    }
                    else
                    {
                        character.GoTo(IsoInput.mousePosition);
                    }
                }
                else
                {
                    character.LookAt(IsoInput.mousePosition);
                }
            }

            character.run = run;
        }

        public bool FixedSelection()
        {
            return Input.GetMouseButton(0) || _mouseItem != null || usingSkills;
        }

        void Update()
        {
            if (GameMenu.IsVisible())
                return;
            
            HandleKeyboard();
            UpdateCamera();
            

            if (flush && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
                return;

            flush = false;
            
            if (Ui.Hover && MouseSelection.instance.HotEntity == null)
                return;

            ControlCharacter();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
            {
                UnityEditor.EditorWindow.focusedWindow.maximized ^= true;
            }
#endif
        }
    }
}
