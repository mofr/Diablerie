using System.Collections.Generic;
using UnityEngine;

class COFAnimator : MonoBehaviour
{
    COF cof;
    public ObjectInfo objectInfo;
    public int direction = 0;

    float time = 0;
    float speed = 1.0f;
    float frameDuration = 1.0f / 12.0f;
    int frameCounter = 0;
    int frameCount = 0;
    int frameStart = 0;
    bool loop = true;
    List<Layer> layers = new List<Layer>();

    struct Layer
    {
        public SpriteRenderer spriteRenderer;
    }

    void Start()
    {
        UpdateConfiguration();
    }

    public void SetCof(COF cof)
    {
        this.cof = cof;
        UpdateConfiguration();
    }

    void UpdateConfiguration()
    {
        if (cof == null)
            return;

        time = 0;
        frameCounter = 0;
        frameCount = cof.framesPerDirection;

        if (objectInfo != null)
        {
            frameStart = objectInfo.start[cof.mode];
            int modeFrameCount = objectInfo.frameCount[cof.mode];
            if (modeFrameCount != 0)
                frameCount = modeFrameCount;
            loop = objectInfo.cycleAnim[cof.mode];
        }

        for (int i = layers.Count; i < cof.layerCount; ++i)
        {
            Layer layer = new Layer();
            GameObject layerObject = new GameObject();
            layerObject.transform.position = new Vector3(0, 0, -i * 0.1f);
            layerObject.transform.SetParent(gameObject.transform, false);
            layer.spriteRenderer = layerObject.AddComponent<SpriteRenderer>();
            layer.spriteRenderer.sortingOrder = Iso.SortingOrder(gameObject.transform.position);
            layers.Add(layer);
        }
    }

    void Update()
    {
        if (!loop && frameCounter >= frameCount)
            return;
        time += Time.deltaTime * speed;
        while (time >= frameDuration)
        {
            time -= frameDuration;
            if (frameCounter < frameCount)
                frameCounter += 1;
            if (frameCounter == frameCount / 2)
                SendMessage("OnAnimationMiddle", SendMessageOptions.DontRequireReceiver);
            if (frameCounter == frameCount)
            {
                SendMessage("OnAnimationFinish", SendMessageOptions.DontRequireReceiver);
                if (loop)
                    frameCounter = 0;
            }
        }
    }

    void LateUpdate()
    {
        for(int i = 0; i < cof.layerCount; ++i)
        {
            Layer layer = layers[i];

            int frameIndex = Mathf.Min(frameCounter, frameCount - 1);
            int layerIndex = cof.priority[(direction * cof.framesPerDirection * cof.layerCount) + (frameIndex * cof.layerCount) + i];
            var cofLayer = cof.layers[layerIndex];
            var dcc = DCC.Load(cofLayer.dccFilename);

            int spriteIndex = direction * dcc.framesPerDirection + frameStart + frameIndex;
            layer.spriteRenderer.sprite = dcc.sprites[spriteIndex];
        }
    }
}
