using System.Collections;
using UnityEngine;

public class World : MonoBehaviour
{
    public string levelName;
    Vector3 entrance;

    void Start ()
    {
        Debug.Log("World Start");
        //LevelInfo levelInfo = LevelInfo.Find(levelName);
        //var ds1Filename = levelInfo.preset.ds1Files[Random.Range(0, levelInfo.preset.ds1Files.Count)];
        //var ds1 = DS1.Load(ds1Filename);
        //SpawnPlayer(ds1.entry);

        var town = DS1.Load(@"data\global\tiles\act1\town\townN1.ds1");
        entrance = new Vector3(30, -15);
        for (int i = 0; i < 5; ++i)
        {
            SpawnMonster("fallen1", entrance + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
            //SpawnMonster("corruptrogue1", entrance + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)));
            //SpawnMonster("skeleton1", entrance + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)));
            //SpawnMonster("zombie1", entrance + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)));
            //SpawnMonster("wraith1", entrance + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)));
        }

        SpawnPlayer(town.entry);
        StartCoroutine(SpawnMonsters());
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

    public static StaticObject SpawnObject(ObjectInfo objectInfo, Vector3 pos)
    {
        var gameObject = new GameObject();
        gameObject.transform.position = pos;
        gameObject.name = objectInfo.description;

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

    IEnumerator SpawnMonsters()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            for (int i = 0; i < 5; ++i)
            {
                var monster = SpawnMonster("fallen1", entrance + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
                monster.ressurecting = true;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            }
        }
    }
}
