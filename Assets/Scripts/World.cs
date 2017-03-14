using UnityEngine;

public class World : MonoBehaviour
{
    public string levelName;

    void Start ()
    {
        //LevelInfo levelInfo = LevelInfo.Find(levelName);
        //var ds1Filename = levelInfo.preset.ds1Files[Random.Range(0, levelInfo.preset.ds1Files.Count)];
        //var ds1 = DS1.Load(ds1Filename);
        //SpawnPlayer(ds1.entry);

        var town = DS1.Load(Application.streamingAssetsPath + "/d2/data/global/tiles/act1/town/townN1.ds1");
        var entrance = new Vector3(30, -15);
        for (int i = 0; i < 5; ++i)
        {
            SpawnMonster("fallen1", entrance + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)));
            //SpawnMonster("corruptrogue1", entrance + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)));
            //SpawnMonster("skeleton1", entrance + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)));
            //SpawnMonster("zombie1", entrance + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)));
            //SpawnMonster("wraith1", entrance + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)));
        }

        SpawnPlayer(town.entry);
    }

    static void SpawnPlayer(Vector3 pos)
    {
        var player = new GameObject("Player");
        player.transform.position = pos;
        var character = player.AddComponent<Character>();
        character.basePath = "data/global/chars";
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
        PlayerController.instance.SetCharacter(character);
    }

    public static GameObject SpawnMonster(string id, Vector3 pos)
    {
        MonStat monStat = MonStat.Find(id);
        return SpawnMonster(monStat, pos);
    }

    public static GameObject SpawnMonster(MonStat monStat, Vector3 pos)
    {
        var monster = new GameObject(monStat.nameStr);
        monster.transform.position = pos;
        var character = monster.AddComponent<Character>();
        character.monStat = monStat;
        character.basePath = "data/global/monsters";
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
        monster.AddComponent<DummyController>();
        return monster;
    }
}
