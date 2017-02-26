using System.Collections.Generic;
using UnityEngine;

class COFAnimator : MonoBehaviour
{
    public COF cof;
    public ObjectInfo objectInfo;
    public int modeIndex = 0;
    public int direction = 0;

    float time = 0;
    float speed = 1.0f;
    float frameDuration = 1.0f / 12.0f;
    int frameIndex = 0;
    int frameCount = 0;
    int frameStart = 0;
    bool loop = true;
    List<Layer> layers = new List<Layer>();

    struct Layer
    {
        public DCC dcc;
        public SpriteRenderer spriteRenderer;
        public Transform transform;
    }

    void Start()
    {
        frameCount = cof.framesPerDirection;

        if (objectInfo != null)
        {
            frameStart = objectInfo.start[modeIndex];
            int modeFrameCount = objectInfo.frameCount[modeIndex];
            if (modeFrameCount != 0)
                frameCount = modeFrameCount;
            loop = objectInfo.cycleAnim[modeIndex];
        }

        for (int i = 0; i < cof.layerCount; ++i)
        {
            Layer layer = new Layer();
            int layerIndex = cof.priority[(direction * cof.framesPerDirection * cof.layerCount) + (frameIndex * cof.layerCount) + i];
            var cofLayer = cof.layers[layerIndex];
            layer.dcc = DCC.Load(cofLayer.dccFilename);

            GameObject layerObject = new GameObject();
            layer.spriteRenderer = layerObject.AddComponent<SpriteRenderer>();
            layerObject.name = cofLayer.name;
            layerObject.transform.position = new Vector3(0, 0, -i * 0.1f);
            layerObject.transform.SetParent(gameObject.transform, false);
            layer.spriteRenderer.sortingOrder = Iso.SortingOrder(gameObject.transform.position);
            layer.transform = layerObject.transform;

            layers.Add(layer);
        }
    }

    void Update()
    {
        if (!loop && frameIndex >= frameCount)
            return;
        time += Time.deltaTime * speed;
        while (time >= frameDuration)
        {
            time -= frameDuration;
            if (frameIndex < frameCount)
                frameIndex += 1;
            if (frameIndex == frameCount / 2)
                SendMessage("OnAnimationMiddle", SendMessageOptions.DontRequireReceiver);
            if (frameIndex == frameCount)
            {
                SendMessage("OnAnimationFinish", SendMessageOptions.DontRequireReceiver);
                if (loop)
                    frameIndex = 0;
            }
        }
    }

    void LateUpdate()
    {
        for(int i = 0; i < cof.layerCount; ++i)
        {
            Layer layer = layers[i];
            int spriteIndex = frameStart + direction * frameCount + Mathf.Min(frameIndex, frameCount - 1);
            layer.spriteRenderer.sprite = layer.dcc.sprites[spriteIndex];
        }
    }
}
