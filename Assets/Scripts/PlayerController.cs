using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    static public PlayerController instance;

    public Character character;

    bool flush = false;
    Iso iso;
    Item _mouseItem;

    void Awake()
    {
        instance = this;

        if (character == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
                SetCharacter(player.GetComponent<Character>());
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
                var dc6 = DC6.Load(_mouseItem.info.invFile, loadAllDirections: true);
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

    void Update()
    {
        if (character == null)
            return;

        if (flush && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
            return;

        flush = false;

        character.LookAt(IsoInput.mousePosition);

        DrawDebugPath();

        if (EventSystem.current.currentSelectedGameObject != null)
            return;

        if (_mouseItem != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup.Create(character.transform.position, _mouseItem);
                FlushInput();
                mouseItem = null;
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (_mouseItem.info.type.body)
                {
                    if (_mouseItem.info.component < character.gear.Length)
                        character.gear[_mouseItem.info.component] = _mouseItem.info.alternateGfx;
                    if (_mouseItem.info.weapon != null)
                        character.weaponClass = _mouseItem.info.weapon.wClass;

                    var equip = character.GetComponent<Equipment>();
                    mouseItem = equip.Equip(_mouseItem);
                }
                else
                    mouseItem = null;
                FlushInput();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            character.Teleport(IsoInput.mouseTile);
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            character.run ^= true;
        }

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
