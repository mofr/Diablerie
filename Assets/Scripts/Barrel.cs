using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]
public class Barrel : MonoBehaviour {

	Animator animator;
	Usable usable;
	SpriteRenderer spriteRenderer;

	void Awake() {
		animator = GetComponent<Animator>();
		usable = GetComponent<Usable>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void OnUse() {
		animator.Play("Use");
		usable.active = false;
		Tilemap.instance[Iso.MapToIso(transform.position)] = true;
		spriteRenderer.sortingLayerName = "OnFloor";
	}
}
