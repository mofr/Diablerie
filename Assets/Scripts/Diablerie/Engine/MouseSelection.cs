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
            bool updateHotEntity = !PlayerController.instance.FixedSelection();
            pickups.Clear();
            if (highlightPickups)
            {
                FindVisiblePickups();
            }
            pickupHighlighter.Show(pickups, updateHotEntity);
            
            if (updateHotEntity)
            {
                if (Ui.Hover)
                    HotEntity = null;
                else if (pickupHighlighter.Hot != null)
                    HotEntity = pickupHighlighter.Hot;
                else
                    HotEntity = CalcHotEntity();
            }

            if (HotEntity != null && !highlightPickups)
            {
                var unit = HotEntity.GetComponent<Unit>();
                if (unit && unit.monStat != null)
                {
                    if (unit.monStat.interact)
                    {
                        ShowLabel(HotEntity);
                    }
                    else if (unit.monStat.killable)
                    {
                        ShowEnemyBar(unit);
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

        private void FindVisiblePickups()
        {
            foreach (var entity in WorldState.instance.Entities)
            {
                if (entity is Pickup pickup)
                {
                    Bounds bounds = entity.bounds;
                    Vector2 minPosition = Camera.main.WorldToViewportPoint(bounds.min);
                    if (minPosition.x >= 1 || minPosition.y >= 1)
                        continue;
                    Vector2 maxPosition = Camera.main.WorldToViewportPoint(bounds.max);
                    if (maxPosition.x <= 0 || maxPosition.y <= 0)
                        continue;
                    pickups.Add(pickup);
                }
            }
        }

        private void ShowLabel(Entity entity)
        {
            EnemyBar.instance.unit = null;
            var labelPosition = entity.transform.position + (Vector3) entity.titleOffset / Iso.pixelsPerUnit;
            label.Show(labelPosition, entity.title);
        }

        private void ShowEnemyBar(Unit unit)
        {
            EnemyBar.instance.unit = unit;
            label.Hide();
        }

        private void ShowNothing()
        {
            EnemyBar.instance.unit = null;
            label.Hide();
        }

        private Entity CalcHotEntity()
        {
            Vector3 currentPosition = new Vector3();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Entity bestMatch = null;
            foreach (var entity in WorldState.instance.Entities)
            {
                if (!entity.selectable)
                    continue;
                
                if (entity == WorldState.instance.Player.unit)
                    continue;

                Bounds bounds = entity.bounds;
                bounds.Expand(Expand);
                if (!bounds.Contains(mousePos))
                    continue;

                bool betterMatch = bestMatch == null || Tools.ManhattanDistance(mousePos, bounds.center) < Tools.ManhattanDistance(mousePos, currentPosition);
                if (betterMatch)
                {
                    bestMatch = entity;
                    currentPosition = bounds.center;
                }
            }

            return bestMatch;
        }
    }
}
