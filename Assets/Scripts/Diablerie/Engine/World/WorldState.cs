using System.Collections.Generic;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine.World
{
    public class WorldState : MonoBehaviour
    {
        public static WorldState instance;
        private readonly CollisionMap _collisionMap = new CollisionMap(2048, 2048);
        private readonly WorldGrid _grid = new WorldGrid(1000, 1000);
        private readonly HashSet<Entity> _entities = new HashSet<Entity>();
        private readonly HashSet<Popup> _popups = new HashSet<Popup>();

        void Awake()
        {
            instance = this;
        }

        public void Add(Entity entity)
        {
            _entities.Add(entity);
        }

        public void Add(Popup popup)
        {
            _popups.Add(popup);
        }

        public void Remove(Entity entity)
        {
            _entities.Remove(entity);
        }

        public void Remove(Popup popup)
        {
            _popups.Remove(popup);
        }

        public CollisionMap CollisionMap => _collisionMap;

        public WorldGrid Grid => _grid;

        public IReadOnlyCollection<Entity> Entities => _entities;

        public IReadOnlyCollection<Popup> Popups => _popups;

        public Player Player { get; set; }
    }
}
