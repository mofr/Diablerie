using System.Collections.Generic;
using UnityEngine;

class COFAnimator : MonoBehaviour
{
    COF cof;
    public int direction = 0;
    public bool loop = true;

    float time = 0;
    float speed = 1.0f;
    float frameDuration = 1.0f / 12.0f;
    int frameCounter = 0;
    int frameCount = 0;
    int frameStart = 0;
    List<Layer> layers = new List<Layer>();
    bool _selected = false;
    Material material;

    struct Layer
    {
        public SpriteRenderer spriteRenderer;
    }

    public Bounds bounds
    {
        get
        {
            Bounds bounds = new Bounds();
            foreach (var layer in layers)
                bounds.Encapsulate(layer.spriteRenderer.bounds);
            return bounds;
        }
    }

    public bool selected
    {
        get { return _selected; }

        set
        {
            if (_selected != value)
            {
                _selected = value;
                material.SetFloat("_SelfIllum", _selected ? 2.0f : 1.0f);
            }
        }
    }

    void Awake()
    {
        material = new Material(Shader.Find("Sprite"));
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

    public void SetFrameRange(int start, int count)
    {
        frameStart = start;
        frameCount = count != 0 ? count : cof.framesPerDirection;
        UpdateConfiguration();
    }

    void UpdateConfiguration()
    {
        if (cof == null)
            return;

        time = 0;
        frameCounter = 0;
        if (frameCount == 0)
            frameCount = cof.framesPerDirection;

        for (int i = layers.Count; i < cof.layerCount; ++i)
        {
            Layer layer = new Layer();
            GameObject layerObject = new GameObject();
            layerObject.transform.position = new Vector3(0, 0, -i * 0.1f);
            layerObject.transform.SetParent(gameObject.transform, false);
            layer.spriteRenderer = layerObject.AddComponent<SpriteRenderer>();
            layer.spriteRenderer.material = material;
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
        if (cof == null)
            return;
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
