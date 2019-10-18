using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandPrompt : MonoBehaviour
{
    static public CommandPrompt instance;
    public InputField inputField;

    List<string> history = new List<string>();
    int historyIndex = -1;

    public bool visible
    {
        get
        {
            return inputField.gameObject.activeSelf;
        }
        set
        {
            inputField.gameObject.SetActive(value);
            if (value)
            {
                historyIndex = -1;
                inputField.text = "";
                inputField.Select();
                inputField.ActivateInputField();
            }
        }
    }

    public void Execute()
    {
        Execute(inputField.text);
    }

    void Awake()
    {
        instance = this;
        inputField.gameObject.SetActive(false);
    }

    void Update()
    {
        bool keyUp = Input.GetKeyDown(KeyCode.UpArrow);
        bool keyDown = Input.GetKeyDown(KeyCode.DownArrow);
        if (visible && history.Count > 0 && (keyUp || keyDown))
        {
            if (keyUp)
            {
                if (historyIndex == -1)
                    historyIndex = history.Count - 1;
                else
                    --historyIndex;
            }
            else if (keyDown)
            {
                ++historyIndex;
            }

            if (historyIndex >= history.Count)
            {
                inputField.text = "";
            }
            else
            {
                historyIndex = Mathf.Clamp(historyIndex, 0, history.Count - 1);
                inputField.text = history[historyIndex];
                inputField.caretPosition = inputField.text.Length;
            }
        }
    }

    void Execute(string input)
    {
        if (input == "")
            return;

        if (history.Count == 0 || history[history.Count - 1] != input)
            history.Add(input);

        string[] parts = input.Split(' ');
        if (parts.Length >= 2 && parts[0] == "/spawn")
        {
            var pos = Iso.MapToWorld(IsoInput.mouseTile);
            if (parts[1] == "pickup")
            {
                string flippyFile = @"data\global\items\flp" + parts[2] + ".dc6";
                Pickup.Create(pos, flippyFile, flippyFile);
                return;
            }

            if (parts[1] == "item")
            {
                string code = parts[2];
                ItemDrop.Drop(code, pos, 100);
                return;
            }

            if (parts[1] == "itemset")
            {
                string subname = parts[2];
                var set = ItemSet.sheet.Find(s => s.id.ToLower().Contains(subname));
                if (set != null)
                {
                    foreach(var setItem in set.items)
                    {
                        var item = Item.Create(setItem.itemCode);
                        item.quality = Item.Quality.Set;
                        item.level = setItem.level;
                        item.identified = false;
                        ItemDrop.GenerateSetItem(item);
                        Pickup.Create(pos, item);
                    }
                }
                return;
            }

            var id = parts[1];

            var objectInfo = ObjectInfo.Find(id);
            if (objectInfo != null)
            {
                var obj = World.SpawnObject(objectInfo, pos, fit: true);
                if (obj != null && parts.Length > 2)
                    obj.modeName = parts[2];
                return;
            }
            
            World.SpawnMonster(id, pos);
        }
        if (parts.Length == 2 && parts[0] == "/act")
        {
            if (int.TryParse(parts[1], out int actNumber))
            {
                World.GoToAct(actNumber);
            }
        }
        else
        {
            Debug.LogWarning(input);
        }
    }
}
