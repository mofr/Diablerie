using Diablerie.Engine;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class LoadingScreen
    {
        private const string SpritesheetPath = @"data\global\ui\Loading\loadingscreen";
        
        private static LoadingScreen instance;
        private readonly GameObject _gameObject;
        private readonly Image _image;
        private Sprite[] _sprites;
        
        public static void Show(float completeness = 0)
        {
            GetInstance().ShowInternal(completeness);
        }

        public static void Hide()
        {
            GetInstance().HideInternal();
        }

        private static LoadingScreen GetInstance()
        {
            if (instance == null)
                instance = new LoadingScreen();
            return instance;
        }

        private LoadingScreen()
        {
            _gameObject = new GameObject("Loading Screen");
            Object.DontDestroyOnLoad(_gameObject);
            var canvas = _gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var backgroundGameObject = new GameObject("Background");
            var backgroundImage = backgroundGameObject.AddComponent<RawImage>();
            backgroundImage.color = Color.black;
            var backgroundTransform = backgroundGameObject.GetComponent<RectTransform>();
            backgroundTransform.SetParent(_gameObject.transform);
            backgroundTransform.anchorMin = new Vector2(0, 0);
            backgroundTransform.anchorMax = new Vector2(1, 1);
            backgroundTransform.pivot = new Vector2(0.5f, 0.5f);
            backgroundTransform.anchoredPosition = new Vector2(0, 0);

            var spritesheet = Spritesheet.Load(SpritesheetPath, PaletteType.Loading);
            _sprites = spritesheet.GetSprites(0);
            
            var imageGameObject = new GameObject("Image");
            _image = imageGameObject.AddComponent<Image>();
            var imageRectTransform = imageGameObject.GetComponent<RectTransform>();
            imageRectTransform.SetParent(_gameObject.transform, true);
            imageRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            imageRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            imageRectTransform.pivot = new Vector2(0.5f, 0.5f);
            imageRectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
        }

        private void ShowInternal(float completeness)
        {
            int spriteIndex = (int)((_sprites.Length - 1) * completeness);
            _image.sprite = _sprites[spriteIndex];
            _image.SetNativeSize();
            _gameObject.SetActive(true);
        }
        
        private void HideInternal()
        {
            _gameObject.SetActive(false);
        }
    }
}
