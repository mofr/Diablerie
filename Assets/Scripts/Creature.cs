using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(COFAnimator))]
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

        if (monStat != null)
        {
            var character = gameObject.AddComponent<Character>();
            character.basePath = obj._base;
            character.token = obj.token;
            character.weaponClass = obj._class;
            character.gear = obj.gear;
            character.directionCount = cof.directionCount;
            character.run = false;
            character.speed = monStat.speed;
            character.attackSpeed = 2.2f;
            gameObject.AddComponent<DummyController>();
        }
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

    public override int nameOffset
    {
        get
        {
            return -(int) (bounds.size.y * Iso.pixelsPerUnit) - 10;
        }
    }

    void OnRenderObject()
    {
        MouseSelection.Submit(this);
    }
}
