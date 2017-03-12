using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
class COFAnimator : MonoBehaviour
{
    COF _cof;
    public int direction = 0;
    public bool loop = true;
    public float speed = 1.0f;
    public float frameDuration = 1.0f / 12.0f;
    string[] _gear;

    float time = 0;
    int frameCounter = 0;
    int frameCount = 0;
    int frameStart = 0;
    List<Layer> layers = new List<Layer>();
    bool _selected = false;
    Material shadowMaterial = new Material(Materials.shadow);

    struct Layer
    {
        public GameObject gameObject;
        public SpriteRenderer renderer;
        public Transform transform;
        public DCC dcc;
        public SpriteRenderer shadow;
    }

    public Bounds bounds
    {
        get
        {
            Bounds bounds = new Bounds();
            for(int i = 0; i < layers.Count; ++i)
            {
                var layer = layers[i];
                if (!layer.gameObject.activeSelf)
                    continue;
                if (i == 0)
                    bounds = layer.renderer.bounds;
                else
                    bounds.Encapsulate(layer.renderer.bounds);
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
                foreach(var layer in layers)
                    layer.renderer.material.SetFloat("_SelfIllum", _selected ? 2.0f : 1.0f);
            }
        }
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
            if (_cof != value)
            {
                _cof = value;
                frameCount = 0;
                UpdateConfiguration();
            }
        }
    }

    public string[] gear
    {
        get { return _gear; }
        set
        {
            _gear = value;
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
        if (_cof == null || _gear == null)
            return;

        time = 0;
        frameCounter = 0;
        frameDuration = _cof.frameDuration;
        if (frameCount == 0)
            frameCount = _cof.framesPerDirection;

        for (int i = layers.Count; i < _cof.layerCount; ++i)
        {
            var cofLayer = _cof.layers[i];
            Layer layer = new Layer();
            layer.gameObject = new GameObject(cofLayer.name);
            layer.transform = layer.gameObject.transform;
            layer.transform.SetParent(transform, false);
            layer.renderer = layer.gameObject.AddComponent<SpriteRenderer>();
            var shadowObject = new GameObject("shadow");
            shadowObject.transform.SetParent(layer.transform, false);
            shadowObject.transform.localScale = new Vector3(1, 0.5f);
            layer.shadow = shadowObject.AddComponent<SpriteRenderer>();
            layer.shadow.material = shadowMaterial;
            layers.Add(layer);
        }
        
        for (int i = 0; i < layers.Count; ++i)
        {
            var layer = layers[i];
            if (i >= _cof.layerCount)
            {
                layer.gameObject.SetActive(false);
                continue;
            }

            var cofLayer = _cof.layers[i];
            string equip = gear[cofLayer.compositIndex];
            if (equip == null || equip == "")
            {
                layer.gameObject.SetActive(false);
                continue;
            }

            var dccFilename = _cof.DccFilename(cofLayer, equip);
            try
            {
                layer.dcc = DCC.Load(dccFilename);
                layer.renderer.material = new Material(cofLayer.material);
                layers[i] = layer;
                layer.gameObject.SetActive(true);
                layer.shadow.gameObject.SetActive(cofLayer.shadow);
            }
            catch (System.IO.FileNotFoundException)
            {
                Debug.LogWarning("File not found " + dccFilename);
                layer.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (_cof == null)
            return;
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
        int sortingOrder = Iso.SortingOrder(transform.position);
        int frameIndex = Mathf.Min(frameCounter, frameCount - 1);
        int spriteIndex = frameStart + frameIndex;
        int priority = (direction * _cof.framesPerDirection * _cof.layerCount) + (frameIndex * _cof.layerCount);
        for (int i = 0; i < _cof.layerCount; ++i)
        {
            int layerIndex = _cof.priority[priority + i];
            var cofLayer = _cof.compositLayers[layerIndex];
            Layer layer = layers[cofLayer.index];
            if (!layer.gameObject.activeSelf)
                continue;
            layer.renderer.sprite = layer.dcc.GetSprites(direction)[spriteIndex];
            layer.renderer.sortingOrder = sortingOrder;
            layer.shadow.sprite = layer.renderer.sprite;
            var pos = layer.transform.position;
            pos.z = -i * 0.1f;
            layer.transform.position = pos;
        }
    }
}
