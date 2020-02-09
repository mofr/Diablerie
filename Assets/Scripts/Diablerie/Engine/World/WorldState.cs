using System.Collections.Generic;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine.World
{
    public class WorldState : MonoBehaviour
    {
        public static WorldState instance;
        private HashSet<Entity> entities = new HashSet<Entity>();
        private HashSet<Popup> popups = new HashSet<Popup>();

        void Awake()
        {
            instance = this;
        }

        public void Add(Entity entity)
        {
            entities.Add(entity);
        }

        public void Add(Popup popup)
        {
            popups.Add(popup);
        }

        public void Remove(Entity entity)
        {
            entities.Remove(entity);
        }

        public void Remove(Popup popup)
        {
            popups.Remove(popup);
        }

        public IReadOnlyCollection<Entity> Entities => entities;

        public IReadOnlyCollection<Popup> Popups => popups;

        public Player Player { get; set; }
    }
}
