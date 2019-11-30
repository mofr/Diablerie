using UnityEngine;

namespace Diablerie.Engine.Entities
{
    public class Entity : MonoBehaviour
    {
        COFAnimator animator;
        string _title = null;

        protected virtual void Start()
        {
            animator = GetComponent<COFAnimator>();
        }

        public virtual Bounds bounds
        {
            get { return animator.bounds; }
        }

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

        public virtual void Operate(Character character = null)
        {
            throw new System.NotImplementedException("Entity.Operate shouldn't be called directly");
        }
    }
}
