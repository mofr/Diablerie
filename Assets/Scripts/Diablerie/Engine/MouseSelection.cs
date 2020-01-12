using System.Collections.Generic;
using Diablerie.Engine.Entities;
using Diablerie.Engine.UI;
using Diablerie.Engine.Utility;
using Diablerie.Engine.World;
using Diablerie.Game.UI;
using UnityEngine;

namespace Diablerie.Engine
{
    class MouseSelection : MonoBehaviour
    {
        private static readonly Vector3 Expand = new Vector3(25, 20) / Iso.pixelsPerUnit;

        public static MouseSelection instance;
        
        private Entity hotEntity;
        private Entity newHotEntity;
        private Vector3 mousePos;
        private Vector3 currentPosition;
        private bool highlightPickups;
        private PickupHighlighter pickupHighlighter;
        private readonly HashSet<Pickup> pickups = new HashSet<Pickup>();
        private LabelPool pickupLabelPool;
        private Label label;

        void Awake()
        {
            instance = this;
            pickupLabelPool = new LabelPool(transform);
            pickupHighlighter = new PickupHighlighter(pickupLabelPool);
            label = new Label(transform);
            label.Hide();
        }

        void Update()
        {
            foreach (var entity in WorldState.instance.Entities)
            {
                if (entity.selectable)
                    Submit(entity);
            }
            bool updateHotEntity = !PlayerController.instance.FixedSelection();
            if (!highlightPickups)
                pickups.Clear();
            pickupHighlighter.Show(pickups, updateHotEntity);
            pickups.Clear();
            if (updateHotEntity)
            {
                if (Ui.Hover)
                    HotEntity = null;
                else if (pickupHighlighter.Hot != null)
                    HotEntity = pickupHighlighter.Hot;
                else
                    HotEntity = newHotEntity;
            }
            newHotEntity = null;

            if (HotEntity != null && !highlightPickups)
            {
                var character = HotEntity.GetComponent<Character>();
                if (character && character.monStat != null)
                {
                    if (character.monStat.interact)
                    {
                        ShowLabel(HotEntity);
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
                    ShowLabel(HotEntity);
                }
            }
            else
            {
                ShowNothing();
            }

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
        }

        public Entity HotEntity
        {
            get { return hotEntity; }
            private set
            {
                if (hotEntity == value)
                    return;
                if (hotEntity != null)
                    hotEntity.selected = false;
                hotEntity = value;
                if (hotEntity != null)
                    hotEntity.selected = true;
            }
        }

        public void SetHighlightPickups(bool highlightPickups)
        {
            this.highlightPickups = highlightPickups;
        }

        private void ShowLabel(Entity entity)
        {
            EnemyBar.instance.character = null;
            var labelPosition = entity.transform.position + (Vector3) entity.titleOffset / Iso.pixelsPerUnit;
            label.Show(labelPosition, entity.title);
        }

        private void ShowEnemyBar(Character character)
        {
            EnemyBar.instance.character = character;
            label.Hide();
        }

        private void ShowNothing()
        {
            EnemyBar.instance.character = null;
            label.Hide();
        }

        private void Submit(Entity entity)
        {
            if (entity == PlayerController.instance.character)
                return;

            if (entity is Pickup pickup)
                pickups.Add(pickup);

            Bounds bounds = entity.bounds;

            if (PlayerController.instance.FixedSelection())
            {
                if (entity == newHotEntity)
                {
                    currentPosition = bounds.center;
                }
                return;
            }

            bounds.Expand(Expand);
            if (!bounds.Contains(mousePos))
                return;

            bool betterMatch = newHotEntity == null || Tools.ManhattanDistance(mousePos, bounds.center) < Tools.ManhattanDistance(mousePos, currentPosition);
            if (betterMatch)
            {
                newHotEntity = entity;
                currentPosition = bounds.center;
            }
        }
    }
}
