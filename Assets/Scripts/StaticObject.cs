using System.Collections.Generic;
using UnityEngine;

[System.Diagnostics.DebuggerDisplay("{name}")]
class StaticObject : Entity
{
    public int direction = 0;
    public Obj obj;
    public ObjectInfo objectInfo;

    int mode;
    COFAnimator animator;

    public ObjectInfo info
    {
        get { return objectInfo; }
    }

    public override string name
    {
        get { return objectInfo.name; }
    }

    public override int nameOffset
    {
        get { return objectInfo.nameOffset; }
    }

    void Awake()
    {
        animator = GetComponent<COFAnimator>();
    }

    override protected void Start()
    {
        base.Start();
        SetMode(obj.mode);
    }

    void OnAnimationFinish()
    {
        if (mode == 1)
        {
            SetMode("ON");
        }
    }

    void SetMode(string modeName)
    {
        mode = System.Array.IndexOf(COF.ModeNames[2], modeName);
        if (objectInfo.draw)
        {
            var cof = COF.Load(obj, modeName);
            animator.cof = cof;
            animator.direction = direction;
            animator.loop = objectInfo.cycleAnim[mode];
            animator.SetFrameRange(objectInfo.start[mode], objectInfo.frameCount[mode]);
        }
    }

    void OnRenderObject()
    {
        if (objectInfo.draw && objectInfo.selectable[mode])
            MouseSelection.Submit(this);
    }
}

