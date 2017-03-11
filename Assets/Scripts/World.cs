using UnityEngine;

public class World : MonoBehaviour
{
    public string levelName;

    void Start ()
    {
        LevelInfo levelInfo = LevelInfo.Find(levelName);
        var ds1Filename = levelInfo.preset.ds1Files[Random.Range(0, levelInfo.preset.ds1Files.Count)];
        var ds1 = DS1.Load(ds1Filename);
        var playerPos = ds1.entry;

        var player = new GameObject("Player");
        player.transform.position = playerPos;
        var character = player.AddComponent<Character>();
        character.basePath = "data/global/chars";
        character.token = "BA";
        character.weaponClass = "1SS";
        character.gear = new string[] { "LIT", "LIT", "LIT", "LIT", "LIT", "AXE", "AXE", "", "LIT", "LIT", "", "", "", "", "", "" };
        character.directionCount = 16;
        character.run = true;
        character.speed = 14;
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
        character.weaponClass = monStat.ext.baseWeaponClass;
        character.basePath = "data/global/monsters";
        character.token = monStat.code;
        character.weaponClass = monStat.ext.baseWeaponClass;
        character.run = false;
        character.speed = monStat.speed;
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
