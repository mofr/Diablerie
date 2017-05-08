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
    public bool shadow = true;
    string[] _equip;

    float time = 0;
    int frameCounter = 0;
    int frameCount = 0;
    int frameStart = 0;
    List<Layer> layers = new List<Layer>();
    bool _selected = false;
    Material shadowMaterial;
    static MaterialPropertyBlock materialProperties;
    bool configChanged = false;
    bool modeChanged = false;

    struct Layer
    {
        public GameObject gameObject;
        public SpriteRenderer renderer;
        public Transform transform;
        public Spritesheet spritesheet;
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
                foreach (var layer in layers)
                {
                    Materials.SetRendererHighlighted(layer.renderer, _selected);
                }
            }
        }
    }

    void Start()
    {
    }

    public COF cof
    {
        get { return _cof; }
        set
        {
            if (_cof != value)
            {
                modeChanged = _cof == null || _cof.mode != value.mode;
                _cof = value;
                frameCount = 0;
                configChanged = true;
            }
        }
    }

    public string[] equip
    {
        get { return _equip; }
        set
        {
            if (_equip == null || value == null || Equals(_equip, value))
            {
                _equip = value;
                configChanged = true;
            }
        }
    }

    public void SetFrameRange(int start, int count)
    {
        frameStart = start;
        frameCount = count != 0 ? count : _cof.framesPerDirection;
        configChanged = true;
    }

    public void Restart()
    {
        time = 0;
        frameCounter = 0;
    }

    void UpdateConfiguration()
    {
        if (_cof == null || _equip == null || !configChanged)
            return;

        configChanged = false;
        frameDuration = _cof.frameDuration;
        if (frameCount == 0)
            frameCount = _cof.framesPerDirection;

        if (modeChanged)
        {
            Restart();
            modeChanged = false;
        }

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
            layer.shadow.sortingLayerName = "Shadow";
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
            string equip = this.equip[cofLayer.compositIndex];
            if (equip == null || equip == "")
            {
                layer.gameObject.SetActive(false);
                continue;
            }

            var spritesheetFilename = _cof.GetSpritesheetFilename(cofLayer, equip);
            try
            {
                layer.spritesheet = Spritesheet.Load(spritesheetFilename);
                layer.renderer.material = cofLayer.material;
                layers[i] = layer;
                layer.gameObject.SetActive(true);
                layer.shadow.gameObject.SetActive(cofLayer.shadow && shadow);
            }
            catch (System.IO.FileNotFoundException e)
            {
                Debug.LogWarning("File not found " + e.FileName);
                layer.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        if (materialProperties == null)
            materialProperties = new MaterialPropertyBlock();
        shadowMaterial = new Material(Materials.shadow);
    }

    void Update()
    {
        if (_cof == null || _equip == null)
            return;

        UpdateConfiguration();

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
        if (_cof == null || _equip == null)
            return;
        UpdateConfiguration();
        int sortingOrder = Iso.SortingOrder(transform.position);
        int frameIndex = Mathf.Min(frameCounter, frameCount - 1);
        int spriteIndex = frameStart + frameIndex;
        direction %= _cof.directionCount;
        int priority = (direction * _cof.framesPerDirection * _cof.layerCount) + (frameIndex * _cof.layerCount);
        for (int i = 0; i < _cof.layerCount; ++i)
        {
            int layerIndex = _cof.priority[priority + i];
            var cofLayer = _cof.compositLayers[layerIndex];
            Layer layer = layers[cofLayer.index];
            if (!layer.gameObject.activeSelf)
                continue;
            layer.renderer.sprite = layer.spritesheet.GetSprites(direction)[spriteIndex];
            layer.renderer.sortingOrder = sortingOrder;
            layer.shadow.sprite = layer.renderer.sprite;
            var pos = layer.transform.position;
            pos.z = -i * 0.1f;
            layer.transform.position = pos;
        }
    }
}
