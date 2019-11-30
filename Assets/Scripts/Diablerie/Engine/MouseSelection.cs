using Diablerie.Engine.Entities;
using Diablerie.Engine.UI;
using Diablerie.Engine.Utility;
using Diablerie.Game.UI;
using UnityEngine;

namespace Diablerie.Engine
{
    class MouseSelection : MonoBehaviour
    {
        static readonly Vector3 Expand = new Vector3(25, 20) / Iso.pixelsPerUnit;

        [HideInInspector]
        public static Entity current;
        static Entity previous;
        static Vector3 mousePos;
        static Vector3 currentPosition;

        void Update()
        {
            if (current != null)
            {
                var character = current.GetComponent<Character>();
                if (character && character.monStat != null)
                {
                    if (character.monStat.interact)
                    {
                        ShowLabel();
                    }
                    else if (character.monStat.killable)
                    {
                        ShowEnemyBar(character);
                    }
                    else
                    {
                        ShowNothing();
                    }
                }
                else
                {
                    ShowLabel();
                }
            }
            else
            {
                ShowNothing();
            }

            if (PlayerController.instance.FixedSelection())
            {
                return;
            }

            if (previous != null)
            {
                previous.selected = false;
            }
            if (current != null)
            {
                current.selected = true;
            }
            previous = current;
            current = null;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
        }

        static private void ShowLabel()
        {
            EnemyBar.instance.character = null;
            var labelPosition = current.transform.position + (Vector3)current.titleOffset / Iso.pixelsPerUnit;
            Ui.ShowLabel(labelPosition, current.title);
        }

        static private void ShowEnemyBar(Character character)
        {
            EnemyBar.instance.character = character;
            Ui.HideLabel();
        }

        static private void ShowNothing()
        {
            EnemyBar.instance.character = null;
            Ui.HideLabel();
        }

        static public void Submit(Entity entity)
        {
            if (entity == PlayerController.instance.character)
                return;

            Bounds bounds = entity.bounds;

            if (PlayerController.instance.FixedSelection())
            {
                if (entity == current)
                {
                    currentPosition = bounds.center;
                }
                return;
            }

            bounds.Expand(Expand);
            if (!bounds.Contains(mousePos))
                return;

            bool betterMatch = current == null || Tools.ManhattanDistance(mousePos, bounds.center) < Tools.ManhattanDistance(mousePos, currentPosition);
            if (betterMatch)
            {
                current = entity;
                currentPosition = bounds.center;
            }
        }
    }
}
