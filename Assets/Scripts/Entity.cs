using UnityEngine;

public class Entity : MonoBehaviour
{
    COFAnimator animator;

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

    public virtual string name
    {
        get { return base.name; }
    }

    public virtual Vector2 nameOffset
    {
        get { return new Vector2(0, 0); }
    }

    public virtual float operateRange
    {
        get { return 2; }
    }

    public virtual void Operate()
    {
        throw new System.NotImplementedException("Entity.Operate shouldn't be called directly");
    }
}
