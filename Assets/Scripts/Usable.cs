using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(BoxCollider2D))]
[RequireComponent (typeof(Animator))]
public class Usable : MonoBehaviour {

	static public Usable hot;
	SpriteRenderer spriteRenderer;
	Animator animator;
	bool used = false;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	void OnMouseEnter() {
		if (used)
			return;
		hot = this;
		spriteRenderer.material.SetFloat("_SelfIllum", 1.0f);
	}

	void OnMouseExit() {
		if (used)
			return;
		hot = null;
		spriteRenderer.material.SetFloat("_SelfIllum", 0.75f);
	}

	public void Use() {
		animator.Play("Use");
		used = true;
		hot = null;
		Tilemap.instance[Iso.MapToIso(transform.position)] = true;
		spriteRenderer.sortingLayerName = "OnFloor";
	}
}
