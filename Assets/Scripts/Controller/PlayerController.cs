using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Character character;
    static public PlayerController instance;

    Iso iso;

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

	public void SetCharacter (Character character)
    {
		this.character = character;
		iso = character.GetComponent<Iso>();
	}

    void DrawDebugPath()
    {
        Vector3 targetPosition;
        if (MouseSelection.current != null)
        {
            targetPosition = Iso.MapToIso(MouseSelection.current.transform.position);
        }
        else {
            targetPosition = IsoInput.mousePosition;
        }
        var path = Pathing.BuildPath(iso.pos, targetPosition, self: iso.gameObject);
        Pathing.DebugDrawPath(iso.pos, path);
    }

	void Update () {
        if (character == null)
            return;

        DrawDebugPath();

        character.LookAt(IsoInput.mousePosition);

        if (Input.GetKeyDown(KeyCode.F4)) {
			character.Teleport(IsoInput.mouseTile);
		}

        if (Input.GetMouseButton(1) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)))
        {
            character.Attack(IsoInput.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            if (MouseSelection.current != null)
            {
                character.target = MouseSelection.current.gameObject;
            }
            else {
                character.GoTo(IsoInput.mousePosition);
            }
        }

		if (Input.GetKeyDown(KeyCode.Tab)) {
			foreach (Character character in GameObject.FindObjectsOfType<Character>()) {
				if (this.character != character) {
					SetCharacter(character);
					return;
				}
			}
		}

        if (Input.GetKeyDown(KeyCode.R))
        {
            character.run ^= true;
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
