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

    public override bool isAttackable => objectInfo.isAttackable;

    public override Vector2 titleOffset
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
        animator.equip = gear;
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
                CollisionMap.SetPassable(Iso.Snap(iso.pos), objectInfo.sizeX, objectInfo.sizeY, true, gameObject);

            mode = newMode;

            var cof = COF.Load(@"data\global\objects", objectInfo.token, "HTH", modeName);
            animator.shadow = objectInfo.blocksLight[mode];
            animator.cof = cof;
            animator.loop = objectInfo.cycleAnim[mode];
            animator.SetFrameRange(objectInfo.start[mode], objectInfo.frameCount[mode]);
            animator.frameDuration = objectInfo.frameDuration[mode];

            if (objectInfo.hasCollision[mode])
                CollisionMap.SetPassable(Iso.Snap(iso.pos), objectInfo.sizeX, objectInfo.sizeY, false, gameObject);
        }
    }

    static string[] treasureClassLetters = new string[] { "A", "B", "C" };

    public override void Operate(Character character)
    {
        Debug.Log(character.name + " use " + name + " (operateFn " + objectInfo.operateFn + ")");

        if (objectInfo.operateFn == 1 // bed, caskets
            || objectInfo.operateFn == 3 // urns
            || objectInfo.operateFn == 4 // chests
            || objectInfo.operateFn == 5 // barrels
            || objectInfo.operateFn == 14 // crates
            || objectInfo.operateFn == 51 // jungle objects
            )
        {
            AudioManager.instance.Play("object_chest_large");

            var levelInfo = LevelInfo.sheet[85]; // todo determine current level
            string tc = "Act " + (levelInfo.act + 1);
            var actLevels = LevelInfo.byAct[levelInfo.act];
            int lowest = actLevels[0].id;
            int highest = actLevels[actLevels.Count - 1].id;
            int letterIndex = (levelInfo.id - lowest) / ((highest - lowest + 1) / 3);
            string letter = treasureClassLetters[letterIndex];
            tc += " Chest " + letter;
            Debug.Log(tc);
            ItemDrop.Drop(tc, transform.position, levelInfo.id);
            SetMode("OP");
        }
        else if (objectInfo.operateFn == 23)
        {
            // waypoint
            if (COF.ModeNames[2][mode] != "OP")
            {
                AudioManager.instance.Play("object_waypoint_open");
                SetMode("OP");
            }
        }
        else
        {
            SetMode("OP");
        }
    }

    void OnRenderObject()
    {
        if (objectInfo.draw && objectInfo.selectable[mode])
            MouseSelection.Submit(this);
    }
}
