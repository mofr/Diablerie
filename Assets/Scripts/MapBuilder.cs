using UnityEngine;

public class MapBuilder : MonoBehaviour
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
        player.AddComponent<COFAnimator>();
        player.AddComponent<Iso>();
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
}
