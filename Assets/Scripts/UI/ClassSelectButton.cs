using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ClassSelectButton : 
    MonoBehaviour, 
    IPointerClickHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler
{
    public delegate void PointerEnterHandler(CharStatsInfo classInfo);
    public delegate void PointerExitHandler(CharStatsInfo classInfo);

    public event PointerEnterHandler OnEnter;
    public event PointerExitHandler OnExit;

    RectTransform rectTransform;
    GameObject dummy;
    COFAnimator dummyAnimator;
    string className;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        className = name;
    }

    void Start()
    {
        CharStatsInfo classInfo = CharStatsInfo.Find(className);
        dummy = CreateDummy(classInfo, new Vector3(0, 0));
        dummyAnimator = dummy.GetComponent<COFAnimator>();
        dummyAnimator.cof = GetCof(classInfo, "TN");
    }
	
	void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(rectTransform.position);
        dummy.transform.position = pos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        World.className = className;
        SceneManager.LoadScene("Game");
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

    static GameObject CreateDummy(CharStatsInfo info, Vector3 pos)
    {
        var gameObject = new GameObject(info.className);
        gameObject.transform.position = pos;

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
