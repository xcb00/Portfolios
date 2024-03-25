using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FishPanel : Panel
{
    [SerializeField] RectTransform circle = null;
    [SerializeField] RectTransform baseLine = null;

    int grade = -1;
    float dist = 3f;
    bool activeFish = false;

    // circle과 baseLine의 scale값을 변화시킬 때마다 Vector3 인스턴스를 만들지 않기 위해 Vector3.one을 사용
    Vector3 scale = Vector3.one;
    Coroutine fishRoutine = null;

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        EventHandler.OpenFishPanelEvent += OpenFishPanel;
        EventHandler.BeforeSceneUnloadEvent += IsPanelActive;
    }
    private void OnDisable()
    {
        EventHandler.OpenFishPanelEvent -= OpenFishPanel;
        EventHandler.BeforeSceneUnloadEvent -= IsPanelActive;
    }

    void OpenFishPanel(bool open, FishType type)
    {
        if (open)
        {
            StartCoroutine(Utility.DelayCall(0.5f, () => ActivePanel()));
            if (fishRoutine != null)  StopCoroutine(fishRoutine);
            fishRoutine = StartCoroutine(StartFishing());
        }
        else
        {
            if (activeFish)
            {
                if (grade>-1 && circle.localScale.x <= GameDatas.fishGrades[grade].baseDist)
                {
                    int ran = Random.Range(0, GameDatas.fishLists[(int)type, grade].Count);
                    Debug.Log(InventoryManager.Instance.GetNameWithCode(GameDatas.fishLists[(int)type, grade][ran]));
                    InventoryManager.Instance.AddItem(Inventories.bag, GameDatas.fishLists[(int)type, grade][ran], 1);
                    grade = -1;
                    EventHandler.CallIncrementAchievementEvent(GPGSIds.achievement_angler_master);
                    EventHandler.CallUnlockAchievementEvent(GPGSIds.achievement_beginner_angler);
                }
                else Debug.Log("낚시 실패");
            }
            else Debug.Log("낚시 실패");

            ClosePanel();
        }
    }

    void IsPanelActive()
    {
        if (canvas.blocksRaycasts)
            ClosePanel();
    }

    void ClosePanel()
    {
        if (fishRoutine != null)
            fishRoutine = null;

        baseLine.localScale = Vector3.zero;
        circle.localScale = Vector3.zero;

        StopAllCoroutines();
        InactivePanel();
    }
/*
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(circle.localScale.x);
        if (fishRoutine != null) { StopCoroutine(fishRoutine); fishRoutine = null; }
        else { fishRoutine = StartCoroutine(StartFishing()); }
    }*/

    IEnumerator StartFishing()
    {
        float t;
        while (true)
        {
            t = Random.Range(1.5f, 3.5f);
            //t = Random.Range(0.5f, 0.6f);
            while (t > 0.0f)
            {
                t -= Time.deltaTime;
                yield return null;
            }

            grade = Utility.GetRandomGrade();

            yield return StartCoroutine(MovingCircle(GameDatas.fishGrades[grade]));
            activeFish = false;
        }
    }

    IEnumerator MovingCircle(FishGradeInfo info)
    {
#if UNITY_ANDROID
        Vibration.Vibrate(150l);
#endif
        activeFish = true;
        int cnt = info.count;
        float time = info.time * 0.5f;
        float t = 0.0f;
        baseLine.localScale = scale * info.baseDist;
        for(int i = 0; i < cnt * 2; i++)
        {
            t = 0.0f;

            while (t < time)
            {
                t += Time.deltaTime;
                if (t > time) t = time;
                circle.localScale = i%2==0? scale * Mathf.Lerp(0.0f, dist, t / time): scale * Mathf.Lerp(dist, 0.0f, t / time);
                yield return null;
            }
            
            yield return null;
        }

        baseLine.localScale = Vector3.zero;
    }

    /*void GetRandomFish()
    {
        int ran = Random.Range(1, 101);
        int _grade = (int)Grade.legendary;

        while (ran > 0)
        {
            if (ran <= GameDatas.gradePercentage[_grade])
                break;

            ran -= GameDatas.gradePercentage[_grade--];
        }
        //Debug.Log((Grade)_grade);
        grade = _grade;
    }*/
}
