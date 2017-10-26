using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    static public PlayerController instance;

    public Character character;
    public Equipment equip;
    public Inventory inventory;
    public CameraController cameraController;

    bool flush = false;
    Iso iso;
    Item _mouseItem;
    bool usingSkills = false;
    bool run = true;

    List<SkillInfo> hotSkills;
    List<KeyCode> hotSkillsBindings = new List<KeyCode> {
        KeyCode.F1,
        KeyCode.F2,
        KeyCode.F3,
        KeyCode.F4,
    };

    void Awake()
    {
        instance = this;

        if (character == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
                SetCharacter(player.GetComponent<Character>());
        }

        hotSkills = new List<SkillInfo> {
            SkillInfo.Find("Fire Bolt"),
            SkillInfo.Find("Charged Bolt"),
            SkillInfo.Find("Glacial Spike"),
            SkillInfo.Find("Teleport"),
        };
    }

    void Start()
    {
        for (int i = 0; i < hotSkills.Count; ++i)
        {
            SkillPanel.instance.SetHotSkill(i, hotSkills[i]);
            SkillPanel.instance.SetHotKey(i, hotSkillsBindings[i].ToString());
        }
    }

    public void FlushInput()
    {
        flush = true;
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
        iso = character.GetComponent<Iso>();
        equip = character.GetComponent<Equipment>();
        inventory = character.GetComponent<Inventory>();
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
                var teleport = World.SpawnObject("TP", pos, fit: true);
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
                Cursor.SetCursor(texture, hotSpot, CursorMode.ForceSoftware);
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
    }

    void DrawDebugPath()
    {
        Vector3 targetPosition;
        if (MouseSelection.current != null)
        {
            targetPosition = Iso.MapToIso(MouseSelection.current.transform.position);
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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            InventoryPanel.instance.visible = false;
            CharstatPanel.instance.visible = false;
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
            if (MouseSelection.current != null)
            {
                var targetCharacter = MouseSelection.current.GetComponent<Character>();
                if (targetCharacter != null)
                    character.UseSkill(skill, targetCharacter);
                else
                    character.UseSkill(skill, Iso.MapToIso(MouseSelection.current.transform.position));
            }
            else
            {
                character.UseSkill(skill, IsoInput.mousePosition);
            }
        }

        // move to PlayerController members once Datasheets loading done not in static section
        SkillInfo leftSkill = SkillInfo.Attack;
        SkillInfo rightSkill = SkillInfo.Attack;

        if (!usingSkills)
        {
            if (Input.GetMouseButton(1) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)))
            {
                character.UseSkill(rightSkill, IsoInput.mousePosition);
            }
            else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (MouseSelection.current != null)
                {
                    var targetCharacter = MouseSelection.current.GetComponent<Character>();
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
                        character.Use(MouseSelection.current);
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
        return Input.GetMouseButton(0) || _mouseItem != null || EventSystem.current.IsPointerOverGameObject() || usingSkills;
    }

    void Update()
    {
        HandleKeyboard();
        UpdateCamera();

        if (flush && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
            return;

        flush = false;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        ControlCharacter();

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

        if (Input.GetKeyDown(KeyCode.F8))
        {
            var pos = Iso.MapToWorld(IsoInput.mousePosition);
            var teleport = World.SpawnObject("TP", pos);
            teleport.modeName = "OP";
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
        {
            UnityEditor.EditorWindow.focusedWindow.maximized ^= true;
        }
#endif
    }
}
