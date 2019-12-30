using System.Collections.Generic;
using UnityEngine;

namespace Diablerie.Engine.UI
{
    public class LabelPool
    {
        private readonly Transform parent;
        private readonly List<Label> pool = new List<Label>();

        public LabelPool(Transform parent)
        {
            this.parent = parent;
        }
        
        public Label Get()
        {
            if (pool.Count == 0)
            {
                return new Label(parent);
            }

            Label label = pool[pool.Count - 1];
            label.Highlighed = false;
            pool.RemoveAt(pool.Count - 1);
            return label;
        }

        public void Return(Label label)
        {
            label.Hide();
            pool.Add(label);
        }
    }
}