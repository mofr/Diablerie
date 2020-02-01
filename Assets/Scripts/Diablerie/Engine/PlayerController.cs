using System.Collections.Generic;
using System.Linq;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.UI;
using Diablerie.Game.UI;
using Diablerie.Game.UI.Inventory;
using Diablerie.Game.UI.SkillPanel;
using UnityEditor;
using UnityEngine;

namespace Diablerie.Engine
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        
        public CameraController cameraController;

        private Player player;
        private bool flush = false;
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
        private Color32[] palette;

        void Awake()
        {
            instance = this;

            // todo: Maybe add customized palette
            palette = Palette.GetPalette(PaletteType.Act1);

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

        private void SetHotSkill(int index, SkillInfo skillInfo)
        {
            hotSkills[index] = skillInfo;
            SkillPanel.instance.SetHotSkill(index, skillInfo);
        }

        public void SetPlayer(Player player)
        {
            this.player = player;
            InventoryPanel.instance.SetPlayer(player);
            OnHandsItemChanged(player.HandsItem);
            player.HandsItemChanged += OnHandsItemChanged;
        }

        public bool Take(Item item)
        {
            return player.Take(item, preferHands: InventoryPanel.instance.visible);
        }

        private void OnHandsItemChanged(Item item)
        {
            if (item != null)
            {
                var dc6 = DC6.Load(item.invFile, palette, loadAllDirections: true);
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
            var iso = player.character.iso;
            var path = Pathing.BuildPath(iso.pos, targetPosition, size: player.character.size, self: player.gameObject);
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
            if (player == null)
                return;

            DrawDebugPath();

            if (player.HandsItem != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Pickup.Create(player.transform.position, player.HandsItem);
                    FlushInput();
                    player.HandsItem = null;
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
                        player.character.UseSkill(skill, targetCharacter);
                    else
                        player.character.UseSkill(skill, Iso.MapToIso(MouseSelection.instance.HotEntity.transform.position));
                }
                else
                {
                    player.character.UseSkill(skill, IsoInput.mousePosition);
                }
            }

            if (!usingSkills)
            {
                if (Input.GetMouseButton(1) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)))
                {
                    player.character.UseSkill(rightSkill, IsoInput.mousePosition);
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
                                player.character.UseSkill(leftSkill, targetCharacter);
                            }
                        }
                        else
                        {
                            player.character.Use(MouseSelection.instance.HotEntity);
                        }
                    }
                    else
                    {
                        player.character.GoTo(IsoInput.mousePosition);
                    }
                }
            }

            player.character.run = run;
        }

        public bool FixedSelection()
        {
            return Input.GetMouseButton(0) || player.HandsItem != null || usingSkills;
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
                EditorWindow.focusedWindow.maximized ^= true;
            }
#endif
        }
    }
}
