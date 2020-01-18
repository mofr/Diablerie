using System.Collections;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Diablerie.Game.UI
{
    public class ClassSelectButton : 
        MonoBehaviour, 
        IPointerClickHandler, 
        IPointerEnterHandler, 
        IPointerExitHandler
    {
        public delegate void EnterHandler(CharStatsInfo classInfo);
        public delegate void ExitHandler(CharStatsInfo classInfo);
        public delegate void ClickHandler(CharStatsInfo classInfo);

        public event EnterHandler OnEnter;
        public event ExitHandler OnExit;
        public event ClickHandler OnClick;

        RectTransform rectTransform;
        GameObject dummy;
        COFAnimator dummyAnimator;
        string className;

        private void Awake()
        {
            Canvas.ForceUpdateCanvases();
            rectTransform = GetComponent<RectTransform>();
            className = name;
            CharStatsInfo classInfo = CharStatsInfo.Find(className);
            dummy = CreateDummy(classInfo);
            dummy.transform.position = UiHelper.ScreenToWorldPoint(rectTransform.position);
            dummyAnimator = dummy.GetComponent<COFAnimator>();
            dummyAnimator.cof = GetCof(classInfo, "TN");
        }

        void LateUpdate()
        {
            dummy.transform.position = UiHelper.ScreenToWorldPoint(rectTransform.position);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CharStatsInfo classInfo = CharStatsInfo.Find(className);
            OnClick(classInfo);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CharStatsInfo classInfo = CharStatsInfo.Find(className);
            dummyAnimator.selected = true;
            dummyAnimator.cof = GetCof(classInfo, "SC");
            OnEnter(classInfo);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CharStatsInfo classInfo = CharStatsInfo.Find(className);
            dummyAnimator.selected = false;
            dummyAnimator.cof = GetCof(classInfo, "TN");
            OnExit(classInfo);
        }

        static GameObject CreateDummy(CharStatsInfo info)
        {
            var gameObject = new GameObject(info.className);
            var animator = gameObject.AddComponent<COFAnimator>();
            animator.equip = new string[] { "LIT", "LIT", "LIT", "LIT", "LIT", "", "", "", "LIT", "LIT", "", "", "", "", "", "" };
            return gameObject;
        }

        static COF GetCof(CharStatsInfo info, string mode)
        {
            var basePath = @"data\global\chars";
            return COF.Load(basePath, info.token, info.baseWClass, mode);
        }
    }
}
