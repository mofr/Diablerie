using UnityEngine;

public class Entity : MonoBehaviour
{
    COFAnimator animator;

    protected virtual void Start()
    {
        animator = GetComponent<COFAnimator>();
    }

    public Bounds bounds
    {
        get { return animator.bounds; }
    }

    public bool selected
    {
        get { return animator.selected; }
        set { animator.selected = value; }
    }

    public virtual string name
    {
        get { return base.name; }
    }

    public virtual int nameOffset
    {
        get { return 0; }
    }
}
