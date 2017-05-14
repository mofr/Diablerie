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
    Dictionary<KeyCode, SkillInfo> skillMap;
    bool usingSkills = false;

    void Awake()
    {
        instance = this;

        if (character == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
                SetCharacter(player.GetComponent<Character>());
        }

        skillMap = new Dictionary<KeyCode, SkillInfo> {
            { KeyCode.F1, SkillInfo.Find("Fire Bolt") },
            { KeyCode.F2, SkillInfo.Find("Charged Bolt") },
            { KeyCode.F3, SkillInfo.Find("Ice Bolt") },
            { KeyCode.F4, SkillInfo.Find("Teleport") },
            { KeyCode.F5, SkillInfo.Find("Bone Spear") }
        };
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

    public bool Take(Item item)
    {
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
        var path = Pathing.BuildPath(iso.pos, targetPosition, self: iso.gameObject);
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

    void ControlUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            InventoryPanel.instance.visible = false;
            CharstatPanel.instance.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryPanel.instance.visible ^= true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CharstatPanel.instance.visible ^= true;
        }

        UpdateCamera();
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
        foreach(var skill in skillMap)
        {
            if (Input.GetKey(skill.Key))
            {
                usingSkills = true;
                Vector2 target = IsoInput.mousePosition;
                if (MouseSelection.current != null)
                {
                    var character = MouseSelection.current.GetComponent<Character>();
                    if (character != null)
                        target = character.iso.pos;
                    else
                        target = Iso.MapToIso(MouseSelection.current.transform.position);
                }
                character.UseSkill(skill.Value, target);
            }
        }

        if (Input.GetMouseButton(1) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)))
        {
            character.Attack(IsoInput.mousePosition);
        }
        else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (MouseSelection.current != null)
            {
                character.target = MouseSelection.current;
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            character.run ^= true;
        }
    }

    public bool FixedSelection()
    {
        return Input.GetMouseButton(0) || _mouseItem != null || EventSystem.current.IsPointerOverGameObject() || usingSkills;
    }

    void Update()
    {
        ControlUI();

        if (flush && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
            return;

        flush = false;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        ControlCharacter();

        if (Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < 1; ++i)
            {
                var tc = TreasureClass.sheet[Random.Range(0, TreasureClass.sheet.Count)];
                if (tc.name == null)
                    continue;
                ItemDrop.Drop(tc.name, Iso.MapToWorld(IsoInput.mouseTile));
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
