using System.Collections.Generic;
using System.IO;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.Utility;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    [ExecuteInEditMode]
    class COFRenderer : MonoBehaviour
    {
        public int direction = 0;
        public float speed = 1.0f;
        public float frameDuration = 1.0f / 12.0f;
        public bool shadow = true;
    
        private COF _cof;
        private string[] _equip;
        private float time = 0;
        private int frameCounter = 0;
        private int _overrideFrame = -1; // TODO New field to replace frame counter
        private int frameCount = 0;
        private int frameStart = 0;
        private List<Layer> layers = new List<Layer>();
        private bool _selected = false;
        private Material shadowMaterial;
        private bool configChanged = false;
        private bool modeChanged = false;

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

        public void SetFrame(int frame)
        {
            _overrideFrame = frame;
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
                layer.shadow.sortingLayerID = SortingLayers.Shadow;
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
                catch (FileNotFoundException e)
                {
                    Debug.LogWarning("Spreadsheet file not found \"" + spritesheetFilename + "\"");
                    layer.gameObject.SetActive(false);
                }
            }
        }

        void Awake()
        {
            shadowMaterial = new Material(Materials.Shadow);
        }

        void Update()
        {
            if (_cof == null || _equip == null)
                return;

            UpdateConfiguration();

            if (frameCounter >= frameCount)
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
            int frameIndex = _overrideFrame == -1 ? frameCounter : _overrideFrame;
            frameIndex = Mathf.Min(frameIndex, frameCount - 1);
            int spriteIndex = frameStart + frameIndex;
            int cofDirection = direction * _cof.directionCount / Unit.DirectionCount;
            int priority = (cofDirection * _cof.framesPerDirection * _cof.layerCount) + (frameIndex * _cof.layerCount);
            for (int i = 0; i < _cof.layerCount; ++i)
            {
                int layerIndex = _cof.priority[priority + i];
                var cofLayer = _cof.compositLayers[layerIndex];
                Layer layer = layers[cofLayer.index];
                if (!layer.gameObject.activeSelf)
                    continue;
                int sheetDirection = direction * layer.spritesheet.directionCount / Unit.DirectionCount;
                layer.renderer.sprite = layer.spritesheet.GetSprites(sheetDirection)[spriteIndex];
                layer.renderer.sortingOrder = sortingOrder;
                layer.shadow.sprite = layer.renderer.sprite;
                var pos = layer.transform.position;
                pos.z = -i * 0.01f;
                layer.transform.position = pos;
            }
        }
    }
}
