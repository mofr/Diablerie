using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Usable))]
public class Barrel : MonoBehaviour {

	IsoAnimator animator;
	Usable usable;
	SpriteRenderer spriteRenderer;

	void Awake() {
		animator = GetComponent<IsoAnimator>();
		usable = GetComponent<Usable>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void OnUse() {
		animator.SetState("Use");
		usable.active = false;
		Tilemap.instance[Iso.MapToIso(transform.position)] = true;
		spriteRenderer.sortingLayerName = "OnFloor";
	}
}
