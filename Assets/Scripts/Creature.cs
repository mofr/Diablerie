using System.Collections.Generic;
using UnityEngine;

class Creature : Entity
{
    public Obj obj;

    COFAnimator animator;
    MonStat monStat;

    void Awake()
    {
        animator = GetComponent<COFAnimator>();
    }

    override protected void Start()
    {
        base.Start();
        var cof = COF.Load(obj._base, obj.token, obj._class, obj.gear, obj.mode);
        animator.cof = cof;
        animator.direction = obj.direction;
        monStat = MonStat.Find(obj.act, obj.id);
    }

    public override string name
    {
        get
        {
            if (monStat != null)
                return monStat.nameStr;
            else
                return "monStat null";
        }
    }

    void OnRenderObject()
    {
        MouseSelection.Submit(this);
    }
}
