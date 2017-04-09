using System.Collections;
using UnityEngine;

public class World : MonoBehaviour
{
    void Start ()
    {
        var town = new LevelBuilder("Act 1 - Town");
        var bloodMoor = CreateBloodMoor();

        var townOffset = new Vector2i(bloodMoor.width - town.width, bloodMoor.height);
        town.Instantiate(townOffset);
        bloodMoor.Instantiate(new Vector2i(0, 0));

        var entry = town.FindEntry();
        SpawnPlayer(Iso.MapTileToWorld(entry + townOffset));
    }

    LevelBuilder CreateBloodMoor()
    {
        var bloodMoor = new LevelBuilder("Act 1 - Wilderness 1");
        var riverN = DS1.Load(@"data\global\tiles\act1\outdoors\UriverN.ds1");
        var uRiver = DS1.Load(@"data\global\tiles\act1\outdoors\Uriver.ds1");
        var lRiver = DS1.Load(@"data\global\tiles\act1\outdoors\Lriver.ds1");
        var bord1 = LevelPreset.Find("Act 1 - Wild Border 1");
        var bord2 = LevelPreset.Find("Act 1 - Wild Border 2");
        var bord3 = LevelPreset.Find("Act 1 - Wild Border 3");
        var bord5 = LevelPreset.Find("Act 1 - Wild Border 5");
        var bord6 = LevelPreset.Find("Act 1 - Wild Border 6");
        var bord9 = LevelPreset.Find("Act 1 - Wild Border 9");
        var cottage = LevelPreset.Find("Act 1 - Cottages 1");
        var denEntrance = LevelPreset.Find("Act 1 - DOE Entrance");

        for (int i = 0; i < bloodMoor.height / (uRiver.height - 1); ++i)
            bloodMoor.Place(lRiver, new Vector2i(bloodMoor.width - (lRiver.width - 1), i * (lRiver.height - 1)));
        for (int i = 1; i < bloodMoor.height / (lRiver.height - 1); ++i)
            bloodMoor.Place(uRiver, new Vector2i(bloodMoor.width - (lRiver.width - 1 + uRiver.width - 1), i * (uRiver.height - 1)));
        bloodMoor.Place(riverN, new Vector2i(bloodMoor.width - 16, 0));

        for (int i = 1; i < bloodMoor.height / bord2.sizeY - 1; ++i)
            bloodMoor.Place(bord2, new Vector2i(0, i * bord2.sizeY), 0, 3);
        bloodMoor.Place(bord5, new Vector2i(0, bloodMoor.height - bord5.sizeY));

        for(int i = 1; i < 3; ++i)
            bloodMoor.Place(bord1, new Vector2i(i * bord1.sizeX, bloodMoor.height - bord1.sizeY), 0, 3);
        bloodMoor.Place(bord9, new Vector2i(3 * bord9.sizeX, bloodMoor.height - bord9.sizeY));

        for (int i = 1; i < (bloodMoor.width - (lRiver.width - 1) * 2) / bord3.sizeX; ++i)
            bloodMoor.Place(bord3, new Vector2i(i * bord3.sizeX, 0), 0, 3);

        bloodMoor.Place(bord6, new Vector2i(0, 0));
        for (int i = 0; i < 5; ++i)
            bloodMoor.Place(cottage, new Vector2i(8 + i * 8, 32 + 8 * Random.Range(-1, 1)));
        bloodMoor.Place(denEntrance, new Vector2i(40, 56));

        return bloodMoor;
    }
    
    static void SpawnPlayer(Vector3 pos)
    {
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = pos;
        var character = player.AddComponent<Character>();
        character.basePath = @"data\global\chars";
        character.token = "BA";
        character.weaponClass = "1SS";
        character.gear = new string[] { "LIT", "LIT", "LIT", "LIT", "LIT", "AXE", "AXE", "", "LIT", "LIT", "", "", "", "", "", "" };
        //character.token = "PA";
        //character.weaponClass = "1HS";
        //character.gear = new string[] { "CRN", "HVY", "HVY", "HVY", "HVY", "SCM", "", "KIT", "", "", "", "", "", "", "", "" };
        character.directionCount = 16;
        character.run = true;
        character.walkSpeed = 7;
        character.runSpeed = 15;
        character.maxHealth = 10000;
        character.health = 10000;
        PlayerController.instance.SetCharacter(character);

        var body = player.AddComponent<Rigidbody2D>();
        body.isKinematic = true;
        var collider = player.AddComponent<CircleCollider2D>();
        collider.radius = Iso.tileSizeY;
    }

    public static Character SpawnMonster(string id, Vector3 pos)
    {
        MonStat monStat = MonStat.Find(id);
        return SpawnMonster(monStat, pos);
    }

    public static Character SpawnMonster(MonStat monStat, Vector3 pos)
    {
        var monster = new GameObject(monStat.nameStr);
        pos = Iso.MapToIso(pos);
        pos = CollisionMap.Fit(pos, monStat.ext.sizeX);
        pos = Iso.MapToWorld(pos);
        monster.transform.position = pos;

        var character = monster.AddComponent<Character>();
        character.monStat = monStat;
        character.basePath = @"data\global\monsters";
        character.token = monStat.code;
        character.weaponClass = monStat.ext.baseWeaponClass;
        character.run = false;
        character.walkSpeed = monStat.speed;
        character.runSpeed = monStat.runSpeed;
        character.health = monStat.minHP;
        character.maxHealth = monStat.minHP;

        character.gear = new string[monStat.ext.gearVariants.Length];
        for (int i = 0; i < character.gear.Length; ++i)
        {
            var variants = monStat.ext.gearVariants[i];
            if (variants == null)
                continue;
            character.gear[i] = variants[Random.Range(0, variants.Length)];
        }

        if (monStat.ai == "Npc")
            monster.AddComponent<NpcController>();
        else if (monStat.ai != "Idle")
            monster.AddComponent<MonsterController>();

        var body = monster.AddComponent<Rigidbody2D>();
        body.isKinematic = true;
        var collider = monster.AddComponent<CircleCollider2D>();
        collider.radius = monStat.ext.sizeX * Iso.tileSizeY;

        return character;
    }

    public static StaticObject SpawnObject(ObjectInfo objectInfo, Vector3 pos, bool fit = false)
    {
        var gameObject = new GameObject();
        gameObject.name = objectInfo.description;

        if (fit)
        {
            pos = Iso.MapToIso(pos);
            pos = CollisionMap.Fit(pos, objectInfo.sizeX);
            pos = Iso.MapToWorld(pos);
        }
        gameObject.transform.position = pos;

        var staticObject = gameObject.AddComponent<StaticObject>();
        staticObject.objectInfo = objectInfo;

        return staticObject;
    }

    public static StaticObject SpawnObject(string token, Vector3 pos)
    {
        ObjectInfo objectInfo = ObjectInfo.Find(token);
        if (objectInfo == null)
        {
            Debug.LogWarning("ObjectInfo with token'" + token + "' not found");
            return null;
        }
        return SpawnObject(objectInfo, pos);
    }
}
