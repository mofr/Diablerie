using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{

    public string path;
    public Character playerPrefab;

	void Start ()
    {
        DS1.Import("Assets/d2/data/global/tiles/" + path);
        var player = GameObject.Instantiate(playerPrefab);
        PlayerController.instance.SetCharacter(player);
	}
}
