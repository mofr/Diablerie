using System.Collections;
using UnityEngine;

public class World : MonoBehaviour
{
    void Start ()
    {
        var town = new Level("Act 1 - Town");
        var bloodMoor = CreateBloodMoor();

        var townOffset = new Vector2i(bloodMoor.width - town.width, bloodMoor.height);
        town.Instantiate(townOffset);
        bloodMoor.Instantiate(new Vector2i(0, 0));

        var entry = town.FindEntry();
        SpawnPlayer(Iso.MapTileToWorld(entry + townOffset));
    }

    Level CreateBloodMoor()
    {
        var bloodMoor = new Level("Act 1 - Wilderness 1");
        var river = DS1.Load(@"data\global\tiles\act1\outdoors\river.ds1");
        var bord2 = LevelPreset.Find("Act 1 - Wild Border 2");
        var bord3 = LevelPreset.Find("Act 1 - Wild Border 3");
        var bord6 = LevelPreset.Find("Act 1 - Wild Border 6");
        var cottage = LevelPreset.Find("Act 1 - Cottages 1");
        var denEntrance = LevelPreset.Find("Act 1 - DOE Entrance");

        for (int i = 0; i < bloodMoor.height / (river.height - 1); ++i)
            bloodMoor.Place(river, new Vector2i(bloodMoor.width - (river.width - 1), bloodMoor.height - (i + 1) * (river.height - 1)));

        for (int i = 1; i < bloodMoor.height / bord2.sizeY; ++i)
            bloodMoor.Place(bord2, new Vector2i(0, i * bord2.sizeY));

        for (int i = 1; i < (bloodMoor.width - river.width + 1) / bord3.sizeX; ++i)
            bloodMoor.Place(bord3, new Vector2i(i * bord3.sizeX, 0));

        bloodMoor.Place(bord6, new Vector2i(0, 0));
        for (int i = 0; i < 5; ++i)
            bloodMoor.Place(cottage, new Vector2i(8 + i * 8, 32 + 8 * Random.Range(-1, 1)));
        bloodMoor.Place(denEntrance, new Vector2i(40, 56));

        var spawnPoint = Iso.MapTileToWorld(new Vector2i(44, 60));
        StartCoroutine(SpawnMonsters(spawnPoint));

        return bloodMoor;
    }
    
    static void SpawnPlayer(Vector3 pos)
    {
        var player = new GameObject("Player");
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
        character.runSpeed = 13;
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

    IEnumerator SpawnMonsters(Vector3 spawnPoint)
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            for (int i = 0; i < 2; ++i)
            {
                var pos = spawnPoint + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                var monster = SpawnMonster("fallen1", pos);
                monster.ressurecting = true;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            }
        }
    }
}
