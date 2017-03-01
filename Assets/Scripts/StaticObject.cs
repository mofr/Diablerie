using System.Collections.Generic;
using UnityEngine;

[System.Diagnostics.DebuggerDisplay("{name}")]
class StaticObject : MonoBehaviour
{
    public int direction = 0;
    public Obj obj;
    public ObjectInfo objectInfo;

    int mode;
    COFAnimator animator;

    void Awake()
    {
        animator = GetComponent<COFAnimator>();
    }

    void Start()
    {
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
            animator.SetCof(cof);
            animator.direction = direction;
            animator.loop = objectInfo.cycleAnim[mode];
            animator.SetFrameRange(objectInfo.start[mode], objectInfo.frameCount[mode]);
        }
    }

    void OnRenderObject()
    {
        if (objectInfo.draw)
            MouseSelection.Submit(this, animator.bounds);
    }

    public bool selected
    {
        get { return animator.selected; }
        set { animator.selected = value; }
    }
}

