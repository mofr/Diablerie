using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Character character;

    [HideInInspector]
    static public GameObject hover;

    Iso iso;
    Collider2D[] hoverColliders = new Collider2D[4];


    void Awake() {
		if (character == null)
			character = GameObject.FindWithTag("Player").GetComponent<Character>();
		SetCharacter(character);
	}

	void Start () {
	}

	void SetCharacter (Character character) {
		this.character = character;
		iso = character.GetComponent<Iso>();
	}

    void UpdateHover()
    {
        if (Input.GetMouseButton(0))
        {
            return;
        }

        GameObject newHover = null;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int overlapCount = Physics2D.OverlapPointNonAlloc(mousePos, hoverColliders);
        if (overlapCount > 0)
        {
            newHover = hoverColliders[0].gameObject;
        }

        if (newHover != hover)
        {
            if (hover != null)
            {
                var spriteRenderer = hover.GetComponent<SpriteRenderer>();
                spriteRenderer.material.SetFloat("_SelfIllum", 1.0f);
            }
            hover = newHover;
            if (hover != null)
            {
                var spriteRenderer = hover.GetComponent<SpriteRenderer>();
                spriteRenderer.material.SetFloat("_SelfIllum", 1.75f);

                EnemyBar.instance.character = hover.GetComponent<Character>();
            }
            else
            {
                EnemyBar.instance.character = null;
            }
        }
    }

	void Update () {
        UpdateHover();

        Vector3 targetTile;
		if (hover != null) {
			targetTile = Iso.MapToIso(hover.transform.position);
		} else {
			targetTile = IsoInput.mouseTile;
		}
		Iso.DebugDrawTile(targetTile, Tilemap.instance[targetTile] ? Color.green : Color.red, 0.1f);
		Pathing.BuildPath(iso.tilePos, targetTile, character.directionCount, character.useRange);

		if (Input.GetKeyDown(KeyCode.F4)) {
			character.Teleport(IsoInput.mouseTile);
		}

		if (Input.GetMouseButton(1) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))) {
			character.Attack();
		}
		else if (Input.GetMouseButton(0)) {
			if (hover != null) {
                character.target = hover;
			} else {
				character.GoTo(targetTile);
			}
		}

		character.LookAt(IsoInput.mousePosition);

		if (Input.GetKeyDown(KeyCode.Tab)) {
			foreach (Character character in GameObject.FindObjectsOfType<Character>()) {
				if (this.character != character) {
					SetCharacter(character);
					return;
				}
			}
		}
	}
}
