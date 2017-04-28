using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandPrompt : MonoBehaviour
{
    public InputField inputField;

    List<string> history = new List<string>();
    int historyIndex = -1;

    void Awake()
    {
        inputField.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            bool active = !inputField.gameObject.activeSelf;
            inputField.gameObject.SetActive(active);
            if (active)
            {
                historyIndex = -1;
                inputField.text = "";
                inputField.Select();
                inputField.ActivateInputField();
            }
            else
            {
                Process(inputField.text);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inputField.gameObject.SetActive(false);
        }

        if (inputField.gameObject.activeSelf && history.Count > 0 &&
            (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (historyIndex == -1)
                    historyIndex = history.Count - 1;
                else
                    --historyIndex;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
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

    void Process(string input)
    {
        if (input == "")
            return;

        if (history.Count == 0 || history[history.Count - 1] != input)
            history.Add(input);

        string[] parts = input.Split(' ');
        if (parts.Length >= 2 && parts[0] == "/spawn")
        {
            var pos = Iso.MapToWorld(IsoInput.mouseTile);
            if (parts[1] == "item")
            {
                string code = parts[2];
                Pickup.Create(pos, code);
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
        else
        {
            Debug.LogWarning(input);
        }
    }
}
