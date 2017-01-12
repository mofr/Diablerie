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
	public bool active = true;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void OnMouseEnter() {
		if (!active)
			return;
		hot = this;
		spriteRenderer.material.SetFloat("_SelfIllum", 2.0f);
	}

	void OnMouseExit() {
		if (!active)
			return;
        hot = null;
        spriteRenderer.material.SetFloat("_SelfIllum", 1.0f);
    }

	public void Use() {
		SendMessage("OnUse");
        hot = null;
    }
}
