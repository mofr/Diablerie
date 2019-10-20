using UnityEngine;

public class World : MonoBehaviour
{
    public static string className = "Sorceress";
    private static Act currentAct;

    void Start()
    {
        currentAct = CreateAct(1);
        Vector2i playerPos = currentAct.entry;
        SpawnPlayer(className, Iso.MapTileToWorld(playerPos));
    }

    static Act CreateAct(int actNumber)
    {
        if (actNumber == 1)
        {
            return new Act1();
        }
        if (actNumber == 2)
        {
            return new Act2();
        }
        if (actNumber == 3)
        {
            return new Act3();
        }
        if (actNumber == 4)
        {
            return new Act4();
        }
        if (actNumber == 5)
        {
            return new Act5();
        }

        return new Act1();
    }

    public static void GoToAct(int actNumber)
    {
        Destroy(currentAct.root);
        currentAct = CreateAct(actNumber);
        PlayerController.instance.character.InstantMove(Iso.MapToIso(Iso.MapTileToWorld(currentAct.entry)));
    }

    static void SpawnPlayer(string className, Vector3 pos)
    {
        CharStatsInfo info = CharStatsInfo.Find(className);
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = pos;
        var character = player.AddComponent<Character>();
        character.basePath = @"data\global\chars";
        character.token = info.token;
        character.weaponClass = info.baseWClass;
        character.directionCount = 16;
        character.run = true;
        character.walkSpeed = 7;
        character.runSpeed = 15;
        character.maxHealth = 1000;
        character.health = 1000;
        character.size = 2;

        character.equip = player.AddComponent<Equipment>();
        Inventory.Create(player, 10, 4);
        var body = player.AddComponent<Rigidbody2D>();
        body.isKinematic = true;
        var collider = player.AddComponent<CircleCollider2D>();
        collider.radius = Iso.tileSizeY;
        var listenerObject = new GameObject("Audio Listener");
        listenerObject.AddComponent<AudioListener>();
        listenerObject.transform.SetParent(player.transform, true);
        listenerObject.transform.localPosition = new Vector3(0, 0, -1);
        character.charStat = player.AddComponent<CharStat>();
        character.charStat.character = character;
        character.charStat.info = info;

        PlayerController.instance.SetCharacter(character);

        foreach (var startingItem in info.startingItems)
        {
            if (startingItem.code == null)
                continue;

            var itemInfo = ItemInfo.Find(startingItem.code);
            for (int i = 0; i < startingItem.count; ++i)
            {
                var item = new Item(itemInfo);
                if (startingItem.loc != null)
                {
                    int loc = BodyLoc.GetIndex(startingItem.loc);
                    character.equip.Equip(item, loc);
                }
                else
                {
                    PlayerController.instance.Take(item);
                }
            }
        }
    }

    public static Character SpawnMonster(string id, Vector3 pos, Transform parent = null)
    {
        MonStat monStat = MonStat.Find(id);
        if (monStat == null)
        {
            Debug.LogWarning("Monster id not found: " + id);
            return null;
        }
        return SpawnMonster(monStat, pos, parent);
    }

    public static Character SpawnMonster(MonStat monStat, Vector3 pos, Transform parent = null)
    {
        pos = Iso.MapToIso(pos);
        if (!CollisionMap.Fit(pos, out pos, monStat.ext.sizeX))
        {
            return null;
        }
        pos = Iso.MapToWorld(pos);

        var monster = new GameObject(monStat.nameStr);
        monster.transform.SetParent(parent);
        monster.transform.position = pos;

        var character = monster.AddComponent<Character>();
        character.monStat = monStat;
        character.title = monStat.name;
        character.basePath = @"data\global\monsters";
        character.token = monStat.code;
        character.weaponClass = monStat.ext.baseWeaponClass;
        character.run = false;
        character.walkSpeed = monStat.speed;
        character.runSpeed = monStat.runSpeed;
        character.size = monStat.ext.sizeX;

        var monLvl = MonLvl.Find(monStat.level[0]);
        if (monLvl != null && !monStat.noRatio)
            character.health = Random.Range(monLvl.hp[0] * monStat.stats[0].minHP, monLvl.hp[0] * monStat.stats[0].maxHP + 1) / 100;
        else
            character.health = Random.Range(monStat.stats[0].minHP, monStat.stats[0].maxHP + 1);
        character.maxHealth = character.health;

        var animator = character.GetComponent<COFAnimator>();
        animator.equip = new string[monStat.ext.gearVariants.Length];
        for (int i = 0; i < animator.equip.Length; ++i)
        {
            var variants = monStat.ext.gearVariants[i];
            if (variants == null)
                continue;
            animator.equip[i] = variants[Random.Range(0, variants.Length)];
        }

        if (monStat.ai == "Npc")
            monster.AddComponent<NpcController>();
        else if (monStat.ai != "Idle" && monStat.ai != "NpcStationary")
            monster.AddComponent<MonsterController>();

        var body = monster.AddComponent<Rigidbody2D>();
        body.isKinematic = true;
        var collider = monster.AddComponent<CircleCollider2D>();
        collider.radius = monStat.ext.sizeX * Iso.tileSizeY;

        return character;
    }

    public static StaticObject SpawnObject(ObjectInfo objectInfo, Vector3 pos, bool fit = false, Transform parent = null)
    {
        if (fit)
        {
            pos = Iso.MapToIso(pos);
            if (!CollisionMap.Fit(pos, out pos, objectInfo.sizeX))
            {
                return null;
            }
            pos = Iso.MapToWorld(pos);
        }

        var gameObject = new GameObject(objectInfo.description);
        gameObject.transform.position = pos;

        var staticObject = gameObject.AddComponent<StaticObject>();
        staticObject.objectInfo = objectInfo;
        staticObject.title = objectInfo.name;

        gameObject.transform.SetParent(parent, true);

        return staticObject;
    }

    public static StaticObject SpawnObject(string token, Vector3 worldPos, bool fit = false)
    {
        ObjectInfo objectInfo = ObjectInfo.Find(token);
        if (objectInfo == null)
        {
            Debug.LogWarning("ObjectInfo with token'" + token + "' not found");
            return null;
        }
        return SpawnObject(objectInfo, worldPos, fit: fit);
    }
}
