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
    State state;
    IsoAnimation.State variation;
    int frameIndex = 0;
    float frameDuration;
    int spritesPerDirection;
    public Dictionary<string, State> states = new Dictionary<string, State>();

    public class State
    {
        public string name;
        public List<IsoAnimation.State> variations = new List<IsoAnimation.State>();
    }

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        character = GetComponent<Character>();

        State firstState = null;
        foreach(var state in anim.states)
        {
            if (states.ContainsKey(state.name))
            {
                states[state.name].variations.Add(state);
            }
            else
            {
                var newState = new State();
                newState.name = state.name;
                newState.variations.Add(state);
                states.Add(state.name, newState);
                if (firstState == null)
                    firstState = newState;
            }
        }

        SetState(firstState);
        frameDuration = 1.0f / anim.fps;
    }
	
	void Update () {
        if (frameIndex >= spritesPerDirection - 1)
            return;
        time += Time.deltaTime * speed;
        while (time >= frameDuration)
        {
            time -= frameDuration;
            if (frameIndex < spritesPerDirection - 1)
                frameIndex += 1;
            if (frameIndex == spritesPerDirection / 2)
                SendMessage("OnAnimationMiddle", SendMessageOptions.DontRequireReceiver);
            if (frameIndex == spritesPerDirection - 1)
            {
                SendMessage("OnAnimationFinish", SendMessageOptions.DontRequireReceiver);
                if (variation.loop)
                    SetupState();
            }
        }
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
        spriteRenderer.sprite = variation.sprites[spriteIndex];
    }

    public void SetState(string stateName)
    {
        if (stateName == state.name)
            return;

        SetState(states[stateName]);
    }

    public void SetState(State state)
    {
        if (this.state == state)
            return;

        this.state = state;
        SetupState();
    }

    void SetupState()
    {
        frameIndex = 0;
        variation = state.variations[Random.Range(0, state.variations.Count)];
        spritesPerDirection = variation.sprites.Length / anim.directionCount;
        if (spritesPerDirection == 0)
            spritesPerDirection = 1;
    }
}
