using Diablerie.Engine.World;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        private COFRenderer _renderer;
        private string _title = null;

        protected virtual void Awake()
        {
            WorldState.instance.Add(this);
        }

        protected virtual void OnDestroy()
        {
            WorldState.instance.Remove(this);
        }
        
        protected virtual void Start()
        {
            _renderer = GetComponent<COFRenderer>();
        }

        public virtual Bounds bounds
        {
            get { return _renderer.bounds; }
        }

        public virtual bool selectable => _renderer != null;

        public virtual bool selected
        {
            get { return _renderer.selected; }
            set { _renderer.selected = value; }
        }

        public string title
        {
            set { _title = value; }
            get { return _title == null ? name : _title; }
        }

        public virtual bool isAttackable => false;

        public virtual Vector2 titleOffset
        {
            get { return new Vector2(0, 0); }
        }

        public virtual float operateRange
        {
            get { return 2; }
        }

        public abstract void Operate(Unit unit);
    }
}
