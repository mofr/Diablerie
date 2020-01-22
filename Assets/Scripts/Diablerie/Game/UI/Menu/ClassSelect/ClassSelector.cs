using System;
using System.Collections.Generic;
using Diablerie.Engine;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using StormLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Diablerie.Game.UI.Menu.ClassSelect
{
    public class ClassSelector : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Initialization
        
        public delegate void EnterHandler(ClassSelectInfo classInfo);
        public delegate void ExitHandler(ClassSelectInfo classInfo);
        public delegate void ClickHandler(ClassSelectInfo classInfo);
        public delegate void StateChangedHandler(object sender);

        public event EnterHandler OnEnter;
        public event ExitHandler OnExit;
        public event ClickHandler OnClick;
        public event StateChangedHandler OnStateChanged;

        private const string BasePath = @"data\global\ui\FrontEnd";

        private string _className;
        private Dictionary<ClassSelectorState, ClassSelectorStateInfo> _states;
        private RectTransform _rectTransform;
        private GameObject _main;
        private GameObject _overlay;
        private GameObject _sound;
        private SpriteAnimator _mainAnimator;
        private SpriteAnimator _overlayAnimator;
        private AudioSource _audioSource;
        private MpqFileStream _sfxStream;
        private ClassSelectorState _state = ClassSelectorState.None;
        
        #endregion
        
        #region Methods

        public void Toggle()
        {
            _mainAnimator.loop = false;
        }

        #endregion
        
        #region Private Methods

        private void MainAnimatorOnFinish()
        {
            switch (_state)
            {
                case ClassSelectorState.BackIdle:
                    ChangeState(ClassSelectorState.FrontTransition);
                    break;
                case ClassSelectorState.FrontIdle:
                    ChangeState(ClassSelectorState.BackTransition);
                    break;
                case ClassSelectorState.BackTransition:
                    ChangeState(ClassSelectorState.BackIdle);
                    OnStateChanged?.Invoke(this);
                    break;
                case ClassSelectorState.FrontTransition:
                    ChangeState(ClassSelectorState.FrontIdle);
                    OnStateChanged?.Invoke(this);
                    break;
                case ClassSelectorState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ChangeState(ClassSelectorState state)
        {
            if (_state != state)
            {
                _state = state;
                var stateInfo = _states[state];
                
                SetAnimator(_mainAnimator, stateInfo.Sprites, stateInfo.Fps, stateInfo.Loop, stateInfo.SortingOrderShift);
                SetAnimator(_overlayAnimator, stateInfo.OverlaySprites, stateInfo.Fps, stateInfo.Loop, stateInfo.SortingOrderShift, stateInfo.HideOnFinish, stateInfo.Material);
                
                if (!string.IsNullOrEmpty(_states[state].SfxPath))
                {
                    if (_audioSource.isPlaying)
                    {
                        _audioSource.Stop();
                        _sfxStream.Close();
                    }
                    _sfxStream = Mpq.fs.OpenFile(_states[state].SfxPath);
                    _audioSource.clip = Wav.Load("sfx", true, _sfxStream);
                    _audioSource.Play();
                }
            }
        }

        private void SetAnimator(SpriteAnimator spriteAnimator, Sprite[] sprites, int fps, bool loop, int sortingOrderShift, bool hideOnFinish = false, Material material = null)
        {
            spriteAnimator.SetSprites(sprites);
            spriteAnimator.fps = fps;
            spriteAnimator.loop = loop;
            spriteAnimator.hideOnFinish = hideOnFinish;
            spriteAnimator.Renderer.sortingOrder += sortingOrderShift;
            if (material != null)
            {
                spriteAnimator.Renderer.material = material;
            }
            spriteAnimator.Restart();
        }

        private void ToggleHover(bool hover)
        {
            if (_states[_state].HoverSprites != null)
            {
                _mainAnimator.SetSprites(hover ? _states[_state].HoverSprites : _states[_state].Sprites);
            }
        }

        #endregion
        
        #region EventHandlers

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null)
            {
                var classInfo = ClassSelectInfo.Find(_className);
                OnClick(classInfo);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ToggleHover(true);
            if (OnEnter != null)
            {
                var classInfo = ClassSelectInfo.Find(_className);
                OnEnter(classInfo);
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            ToggleHover(false);
            if (OnExit != null)
            {
                var classInfo = ClassSelectInfo.Find(_className);
                OnExit(classInfo);
            }
        }

        #endregion
        
        #region Unity Lifecycle

        private void Awake()
        {
            _className = name;
            _rectTransform = GetComponent<RectTransform>();
            
            var classAnimationInfo = ClassSelectInfo.Find(_className);
            var classPath = $@"{BasePath}\{classAnimationInfo.ClassName}\{classAnimationInfo.Token}";
            var backTransitionSprites = Spritesheet.Load($"{classPath}BW", PaletteType.Fechar);
            var backIdleSprites = Spritesheet.Load($"{classPath}NU1", PaletteType.Fechar);
            var backIdleHoverSprites = Spritesheet.Load($"{classPath}NU2", PaletteType.Fechar);
            var backTransitionOverlaySprites = classAnimationInfo.HasBackTransitionOverlay ? Spritesheet.Load($@"{classPath}BWs", PaletteType.Fechar) : null;
            var frontTransitionSprites = Spritesheet.Load($"{classPath}FW", PaletteType.Fechar);
            var frontIdleSprites = Spritesheet.Load($"{classPath}NU3", PaletteType.Fechar);
            var frontTransitionOverlaySprites = classAnimationInfo.HasFrontTransitionOverlay ? Spritesheet.Load($@"{classPath}FWs", PaletteType.Fechar) : null;
            var frontIdleOverlaySprites = classAnimationInfo.HasFrontIdleOverlay ? Spritesheet.Load($@"{classPath}NU3s", PaletteType.Fechar) : null;
            var frontTransitionSfxPath = $@"data\global\sfx\cursor\intro\{classAnimationInfo.ClassName} select.wav";
            var backTransitionSfxPath =$@"data\global\sfx\cursor\intro\{classAnimationInfo.ClassName} deselect.wav";
            
            _states = new Dictionary<ClassSelectorState, ClassSelectorStateInfo>
            {
                {
                    ClassSelectorState.BackIdle,
                    new ClassSelectorStateInfo
                    {
                        Sprites = backIdleSprites.GetSprites(0),
                        HoverSprites = backIdleHoverSprites.GetSprites(0),
                        OverlaySprites = null,
                        Loop = true,
                        HideOnFinish = false,
                        SortingOrderShift = -10,
                        Fps = classAnimationInfo.BackIdleFps
                    }
                },
                {
                    ClassSelectorState.FrontIdle,
                    new ClassSelectorStateInfo
                    {
                        Sprites = frontIdleSprites.GetSprites(0),
                        OverlaySprites = frontIdleOverlaySprites?.GetSprites(0),
                        Loop = true,
                        HideOnFinish = false,
                        SortingOrderShift = 0,
                        Material = classAnimationInfo.OverlayMaterial,
                        Fps = classAnimationInfo.FrontIdleFps
                    }
                },
                {
                    ClassSelectorState.BackTransition,
                    new ClassSelectorStateInfo
                    {
                        Sprites = backTransitionSprites.GetSprites(0),
                        OverlaySprites = backTransitionOverlaySprites?.GetSprites(0),
                        Loop = false,
                        HideOnFinish = true,
                        SortingOrderShift = 0,
                        Material = classAnimationInfo.OverlayMaterial,
                        SfxPath = backTransitionSfxPath,
                        Fps = 25
                    }
                },
                {
                    ClassSelectorState.FrontTransition,
                    new ClassSelectorStateInfo
                    {
                        Sprites = frontTransitionSprites.GetSprites(0),
                        OverlaySprites = frontTransitionOverlaySprites?.GetSprites(0),
                        Loop = false,
                        HideOnFinish = true,
                        SortingOrderShift = 10,
                        Material = classAnimationInfo.OverlayMaterial,
                        SfxPath = frontTransitionSfxPath,
                        Fps = 25
                    }
                }
            };

            var position = _rectTransform.position;
            
            _main = UiHelper.CreateAnimatedObject("main", loop: false, sortingOrder: classAnimationInfo.BaseSortingOrder);
            _main.transform.position = UiHelper.ScreenToWorldPoint(position);
            _main.transform.parent = _rectTransform;

            _overlay = UiHelper.CreateAnimatedObject("overlay", loop: false, sortingOrder: classAnimationInfo.BaseSortingOrder + classAnimationInfo.OverlaySortingOrder);
            _overlay.transform.position = UiHelper.ScreenToWorldPoint(position);
            _overlay.transform.parent = _rectTransform;

            _mainAnimator = _main.GetComponent<SpriteAnimator>();
            _mainAnimator.OnFinish += MainAnimatorOnFinish;
            
            _overlayAnimator = _overlay.GetComponent<SpriteAnimator>();
            
            _sound = new GameObject("sound");
            _sound.transform.parent = _rectTransform;
            
            _audioSource = _sound.AddComponent<AudioSource>();
            _audioSource.loop = false;

            ChangeState(ClassSelectorState.BackIdle);
        }
        
        private void Update()
        {
            if (!_audioSource.isPlaying)
            {
                _sfxStream?.Close();
            }
        }
        
        private void LateUpdate()
        {
            var position = _rectTransform.position;
            _main.transform.position = UiHelper.ScreenToWorldPoint(position);
            _overlay.transform.position = UiHelper.ScreenToWorldPoint(position);
        }

        #endregion
    }
}