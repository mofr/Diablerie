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
        public int direction;
        public bool shadow = true;
        public int frame;
    
        private COF _cof;
        private string[] _equip;
        private bool _selected = false;
        private Material _shadowMaterial;
        private bool _configChanged = false;
        private List<Layer> _layers = new List<Layer>();

        private struct Layer
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
                for(int i = 0; i < _layers.Count; ++i)
                {
                    var layer = _layers[i];
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
                    foreach (var layer in _layers)
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
                    _cof = value;
                    _configChanged = true;
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
                    _configChanged = true;
                }
            }
        }

        private void Awake()
        {
            _shadowMaterial = new Material(Materials.Shadow);
        }

        private void LateUpdate()
        {
            if (_cof == null || _equip == null)
                return;
            UpdateConfiguration();
            int sortingOrder = Iso.SortingOrder(transform.position);
            int showFrame = Mathf.Min(frame, _cof.framesPerDirection - 1);
            int cofDirection = direction * _cof.directionCount / Unit.DirectionCount;
            int priority = (cofDirection * _cof.framesPerDirection * _cof.layerCount) + (showFrame * _cof.layerCount);
            for (int i = 0; i < _cof.layerCount; ++i)
            {
                int layerIndex = _cof.priority[priority + i];
                var cofLayer = _cof.compositLayers[layerIndex];
                Layer layer = _layers[cofLayer.index];
                if (!layer.gameObject.activeSelf)
                    continue;
                int sheetDirection = direction * layer.spritesheet.directionCount / Unit.DirectionCount;
                layer.renderer.sprite = layer.spritesheet.GetSprites(sheetDirection)[showFrame];
                layer.renderer.sortingOrder = sortingOrder;
                layer.shadow.sprite = layer.renderer.sprite;
                var pos = layer.transform.position;
                pos.z = -i * 0.01f;
                layer.transform.position = pos;
            }
        }

        private void UpdateConfiguration()
        {
            if (_cof == null || _equip == null || !_configChanged)
                return;

            _configChanged = false;

            for (int i = _layers.Count; i < _cof.layerCount; ++i)
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
                layer.shadow.material = _shadowMaterial;
                layer.shadow.sortingLayerID = SortingLayers.Shadow;
                _layers.Add(layer);
            }
        
            for (int i = 0; i < _layers.Count; ++i)
            {
                var layer = _layers[i];
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
                    _layers[i] = layer;
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
    }
}
