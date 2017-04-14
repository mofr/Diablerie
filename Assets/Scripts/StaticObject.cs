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

    public override Vector2 nameOffset
    {
        get { return new Vector2(0, -objectInfo.nameOffset); }
    }

    public override float operateRange
    {
        get { return objectInfo.operateRange; }
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
        if (objectInfo.draw)
        {
            int newMode = System.Array.IndexOf(COF.ModeNames[2], modeName);
            if (newMode == -1 || !objectInfo.mode[newMode])
            {
                Debug.LogWarning("Failed to set mode '" + modeName + "' of object " + name);
                return;
            }

            if (objectInfo.hasCollision[mode])
                CollisionMap.SetPassable(Iso.Snap(iso.pos), objectInfo.sizeX, objectInfo.sizeY, true);

            mode = newMode;

            var cof = COF.Load(@"data\global\objects", objectInfo.token, "HTH", modeName);
            animator.shadow = objectInfo.blocksLight[mode];
            animator.cof = cof;
            animator.loop = objectInfo.cycleAnim[mode];
            animator.SetFrameRange(objectInfo.start[mode], objectInfo.frameCount[mode]);
            animator.frameDuration = objectInfo.frameDuration[mode];

            if (objectInfo.hasCollision[mode])
                CollisionMap.SetPassable(Iso.Snap(iso.pos), objectInfo.sizeX, objectInfo.sizeY, false);
        }
    }

    public override void Operate(Character character)
    {
        Debug.Log(character.name + " use " + name);
        SetMode("OP");
    }

    void OnRenderObject()
    {
        if (objectInfo.draw && objectInfo.selectable[mode])
            MouseSelection.Submit(this);
    }
}

