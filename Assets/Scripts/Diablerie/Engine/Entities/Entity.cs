using Diablerie.Engine.World;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        private COFAnimator animator;
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
            animator = GetComponent<COFAnimator>();
        }

        public virtual Bounds bounds
        {
            get { return animator.bounds; }
        }

        public virtual bool selectable => true;

        public virtual bool selected
        {
            get { return animator.selected; }
            set { animator.selected = value; }
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

        public abstract void Operate(Character character);
    }
}
