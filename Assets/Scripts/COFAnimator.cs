using System.Collections.Generic;
using UnityEngine;

class COFAnimator : MonoBehaviour
{
    COF _cof;
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
            for(int i = 0; i < layers.Count; ++i)
            {
                var layer = layers[i];
                if (i == 0)
                    bounds = layer.spriteRenderer.bounds;
                else
                    bounds.Encapsulate(layer.spriteRenderer.bounds);
            }
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

    public COF cof
    {
        get { return _cof; }
        set
        {
            _cof = value;
            UpdateConfiguration();
        }
    }

    public void SetFrameRange(int start, int count)
    {
        frameStart = start;
        frameCount = count != 0 ? count : _cof.framesPerDirection;
        UpdateConfiguration();
    }

    void UpdateConfiguration()
    {
        if (_cof == null)
            return;

        time = 0;
        frameCounter = 0;
        if (frameCount == 0)
            frameCount = _cof.framesPerDirection;

        for (int i = layers.Count; i < _cof.layerCount; ++i)
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
        if (_cof == null)
            return;
        for(int i = 0; i < _cof.layerCount; ++i)
        {
            Layer layer = layers[i];

            int frameIndex = Mathf.Min(frameCounter, frameCount - 1);
            int layerIndex = _cof.priority[(direction * _cof.framesPerDirection * _cof.layerCount) + (frameIndex * _cof.layerCount) + i];
            var cofLayer = _cof.layers[layerIndex];
            var dcc = DCC.Load(cofLayer.dccFilename);

            int spriteIndex = direction * dcc.framesPerDirection + frameStart + frameIndex;
            layer.spriteRenderer.sprite = dcc.sprites[spriteIndex];
        }
    }
}
