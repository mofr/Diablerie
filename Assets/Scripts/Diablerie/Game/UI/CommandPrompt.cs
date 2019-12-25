using System.Collections.Generic;
using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Game.World;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class CommandPrompt : MonoBehaviour
    {
        public static CommandPrompt instance;
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
                    string subname = parts[2].ToLower();
                    var uniqueItem = UniqueItem.sheet.Find(s => s.nameStr.ToLower().Contains(subname));
                    if (uniqueItem != null)
                    {
                        ItemDrop.Drop(uniqueItem, pos);
                    }
                    else
                    {
                        string code = parts[2];
                        ItemDrop.Drop(code, pos, 100);
                    }

                    return;
                }

                if (parts[1] == "itemset")
                {
                    string subname = parts[2].ToLower();
                    var set = ItemSet.sheet.Find(s => s.id.ToLower().Contains(subname));
                    if (set != null)
                    {
                        foreach(var setItem in set.items)
                        {
                            ItemDrop.Drop(setItem, pos);
                        }
                    }
                    return;
                }

                var id = parts[1];

                var objectInfo = ObjectInfo.Find(id);
                if (objectInfo != null)
                {
                    var obj = WorldBuilder.SpawnObject(objectInfo, pos, fit: true);
                    if (obj != null && parts.Length > 2)
                        obj.modeName = parts[2];
                }
                else
                {
                    int monsterCount = parts.Length > 2 ? int.Parse(parts[2]) : 1;
                    for (int i = 0; i < monsterCount; ++i)
                    {
                        WorldBuilder.SpawnMonster(id, pos);
                    }
                }
            }
            else if (parts.Length == 2 && parts[0] == "/act")
            {
                if (int.TryParse(parts[1], out int actNumber))
                {
                    WorldBuilder.GoToAct(actNumber);
                }
            }
            else if (parts.Length == 2 && parts[0] == "/addstate")
            {
                string stateCode = parts[1];
                var stateInfo = StateInfo.FindByCode(stateCode);
                if (stateInfo != null)
                {
                    var player = PlayerController.instance.character.gameObject;
                    Overlay.Create(player, stateInfo.castoverlay, loop: false);
                    Overlay.Create(player, stateInfo.overlay1, loop: true);
                    Overlay.Create(player, stateInfo.overlay2, loop: true);
                    Overlay.Create(player, stateInfo.overlay3, loop: true);
                    Overlay.Create(player, stateInfo.overlay4, loop: true);
                }
            }
            else
            {
                Debug.LogWarning(input);
            }
        }
    }
}
