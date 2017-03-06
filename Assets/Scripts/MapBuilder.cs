using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public string path;

    void Start ()
    {
        var result = DS1.Import(Application.streamingAssetsPath + "/d2/data/global/tiles/" + path);
        var playerPos = result.entry;

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
