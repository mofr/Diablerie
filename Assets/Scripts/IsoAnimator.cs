using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoAnimator : MonoBehaviour {

    public IsoAnimation anim;
    [HideInInspector]
    public float speed = 1.0f;

    SpriteRenderer spriteRenderer;
    Character character;
    float time = 0;
    IsoAnimation.State state;
    int frameIndex = 0;
    float frameDuration;
    int spritesPerDirection;

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        character = GetComponent<Character>();

        SetState(anim.states[0]);
        frameDuration = 1.0f / anim.fps;
    }
	
	void Update () {
        time += Time.deltaTime * speed;
        while (time >= frameDuration)
        {
            time -= frameDuration;
            if (frameIndex < spritesPerDirection - 1 || state.loop)
                frameIndex += 1;
            if (frameIndex == spritesPerDirection / 2)
                SendMessage("OnAnimationMiddle", SendMessageOptions.DontRequireReceiver);
            if (frameIndex == spritesPerDirection - 1)
                SendMessage("OnAnimationFinish", SendMessageOptions.DontRequireReceiver);
        }
        frameIndex = frameIndex % spritesPerDirection;
    }

    void LateUpdate()
    {
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        int direction = 0;
        if (character)
            direction = (character.direction + anim.directionOffset) % anim.directionCount;
        int spriteIndex = direction * spritesPerDirection + frameIndex;
        spriteRenderer.sprite = state.sprites[spriteIndex];
    }

    public void SetState(string stateName)
    {
        if (stateName == state.name)
            return;
        
        foreach (var state in anim.states)
        {
            if (state.name == stateName)
            {
                SetState(state);
                break;
            }
        }
    }

    public void SetState(IsoAnimation.State state)
    {
        if (this.state == state)
            return;

        this.state = state;
        SetupState();
    }

    void SetupState()
    {
        frameIndex = 0;
        spritesPerDirection = state.sprites.Length / anim.directionCount;
        if (spritesPerDirection == 0)
            spritesPerDirection = 1;
    }
}
