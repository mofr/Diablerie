using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {

	[Range(0, 2)]
	public int fullness = 2;
	Animator animator;
	Usable usable;

	void Awake() {
		animator = GetComponent<Animator>();
		usable = GetComponent<Usable>();
	}

	void Start() {
		usable.active = fullness != 0;
		animator.Play(fullness.ToString());
	}

	void OnUse() {
        if (fullness == 0)
            return;
		fullness -= 1;
		animator.Play(fullness.ToString());
		usable.active = fullness != 0;
	}
}
