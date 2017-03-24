using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Iso))]
[RequireComponent(typeof(COFAnimator))]
[ExecuteInEditMode]
[System.Diagnostics.DebuggerDisplay("{name}")]
public class StaticObject : Entity
{
    public string modeName = "NU";
    public ObjectInfo objectInfo;

    readonly static string[] gear = { "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT" };

    int mode;
    COFAnimator animator;
    Iso iso;

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
        get { return -objectInfo.nameOffset; }
    }

    void Awake()
    {
        iso = GetComponent<Iso>();
        animator = GetComponent<COFAnimator>();
        animator.gear = gear;
    }

    override protected void Start()
    {
        base.Start();
        SetMode(modeName);
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
            var cof = COF.Load(@"data\global\objects", objectInfo.token, "HTH", modeName);
            animator.shadow = objectInfo.blocksLight[mode];
            animator.cof = cof;
            animator.loop = objectInfo.cycleAnim[mode];
            animator.SetFrameRange(objectInfo.start[mode], objectInfo.frameCount[mode]);
            animator.frameDuration = objectInfo.frameDuration[mode];

            if (objectInfo.hasCollision[mode])
                Tilemap.SetPassable(Iso.Snap(iso.pos), objectInfo.sizeX, objectInfo.sizeY, false);
        }
    }

    void OnRenderObject()
    {
        if (objectInfo.draw && objectInfo.selectable[mode])
            MouseSelection.Submit(this);
    }
}

