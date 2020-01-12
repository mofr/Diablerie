using System.Collections.Generic;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine.World
{
    public class WorldState : MonoBehaviour
    {
        public static WorldState instance;
        private HashSet<Entity> entities = new HashSet<Entity>();

        void Awake()
        {
            instance = this;
        }

        public void Add(Entity entity)
        {
            entities.Add(entity);
        }

        public void Remove(Entity entity)
        {
            entities.Remove(entity);
        }

        public IReadOnlyCollection<Entity> Entities => entities;
    }
}