using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] BuildPreview previewSoil;
    [SerializeField] BuildPreview previewBanner;
    [SerializeField] float s1Cooldown = 3f;
    [SerializeField] float s2Cooldown = 5f;
    [SerializeField] Cooldown s1;
    [SerializeField] Cooldown s2;
    [SerializeField] Animator playerAnimator;
    //[SerializeField] RectTransform bannerUI;
    //[SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] ExplanationUI explanation;
    float axisRaw = 0f;
    float inputValue = 0.0f;
    bool isDash = false;
    Vector3 position;

    public bool canAttack = true;
    bool canSkill = true;
    bool canSkill1 = true;
    bool canSkill2 = true;


    private void OnEnable()
    {
        EventHandler.CanUseSkillEvent += CanUseSkill;
        EventHandler.CanAttackEvent += CanAttack;
        EventHandler.CoolDownEvent += CoolDown;
    }
    private void OnDisable()
    {
        EventHandler.CanUseSkillEvent -= CanUseSkill;
        EventHandler.CanAttackEvent -= CanAttack;
        EventHandler.CoolDownEvent -= CoolDown;
    }

    void CanUseSkill(bool b) { canSkill = b; }
    void CanAttack(bool b) { canAttack = b; }

    void Update()
    {
        #region Player Input
        axisRaw = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.LeftControl)) isDash = true;
        else if (Input.GetKeyUp(KeyCode.LeftControl)) isDash = false;

        if (Mathf.Abs(axisRaw) > 0f)
        {
            if (!Mathf.Approximately(inputValue, axisRaw * (isDash ? 10f : 1f)))
            {
                inputValue = axisRaw * (isDash ? 10f : 1f);
                EventHandler.PlayerMoveEvent(inputValue, isDash);
            }
        }
        else
        {
            if(!Mathf.Approximately(inputValue, 0f))
            {
                inputValue = 0f;
                EventHandler.PlayerMoveEvent(inputValue, false);
            }
            
        }



        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Debug.Log($"playing : {playerAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing))} / canAttack : {canAttack} / canSkill : {canSkill} / canSkill1 : {canSkill1} / canSkill2 : {canSkill2}");
            if (!playerAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing))/* && canAttack*/ && canSkill && canSkill1)
            {
                ActiveSkill1();
            }
            else
                EventHandler.CallPrintSystemMassageEvent("스킬을 사용할 수 없습니다");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            //Debug.Log($"playing : {playerAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing))} / canAttack : {canAttack} / canSkill : {canSkill} / canSkill1 : {canSkill1} / canSkill2 : {canSkill2}");
            if (!playerAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing))/* && canAttack*/ && canSkill && canSkill2)
            {
                ActiveSkill2();
            }
            else
                EventHandler.CallPrintSystemMassageEvent("스킬을 사용할 수 없습니다");
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //Debug.Log($"playing : {playerAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing))} / canAttack : {canAttack} / canSkill : {canSkill} / canSkill1 : {canSkill1} / canSkill2 : {canSkill2}");
            if (!playerAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)) && canAttack)
            {
                canAttack = false;
                inputValue = 0f;
                EventHandler.CallPlayerAttackEvent(0);
            }
            else
                EventHandler.CallPrintSystemMassageEvent("공격할 수 없습니다");
        }

        #endregion

        #region Building Input - Old Ver
        /*if (previewSoil.activeSelf)        
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                SoilManager.Instance.FollowCursor(hit);
                if (Input.GetMouseButtonDown(0))
                {
                    if(EventSystem.current.IsPointerOverGameObject())
                        SoilManager.Instance.InativePreview();
                    else
                        SoilManager.Instance.Build(hit);
                }
            }
            else
                SoilManager.Instance.InativePreview();
        }

        if (previewBanner.activeSelf)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                SoilManager.Instance.FollowCursor(hit);
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        SoilManager.Instance.InativePreview();
                    else
                        SoilManager.Instance.Build(hit);
                }
            }
            else
                SoilManager.Instance.InativePreview();
        }*/
        #endregion

        #region Building Input - New Ver
        if (previewSoil.gameObject.activeSelf)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                previewSoil.FollowCursor(hit);
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        previewSoil.InactivePreview();
                    else
                        previewSoil.Build(hit);
                }
            }
            else
                previewSoil.InactivePreview();
        }

        if (previewBanner.gameObject.activeSelf)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                previewBanner.FollowCursor(hit);
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        previewBanner.InactivePreview();
                    else
                        previewBanner.Build(hit);
                }
            }
            else
                previewBanner.InactivePreview();
        }
        
        if (!previewSoil.gameObject.activeSelf && !previewBanner.gameObject.activeSelf) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, (1 << (int)LayerName.Banner)))
                {
                    //Vector3 screenPos = Camera.main.WorldToScreenPoint(hit.point);
                    //bannerUI.position = new Vector3(Mathf.Round(screenPos.x), 600f, 0f);
                    previewBanner.ActivePreview();
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EventHandler.CallBannerchangeEvent(true);
            }
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Escape))
            explanation.ShowExplanation();
    }

    void ActiveSkill1()
    {
        inputValue = 0f;
        //canAttack = false;
        canSkill = false;
        canSkill1 = false;
        EventHandler.CallPlayerAttackEvent(1);
        s1.UseSkill();
    }

    void ActiveSkill2()
    {
        inputValue = 0f;
        //canAttack = false;
        canSkill = false;
        canSkill2 = false;
        EventHandler.CallPlayerAttackEvent(2);
        s2.UseSkill();
    }

    void CoolDown(bool firstSkill)
    {
        if (firstSkill)
        {
            s1.CoolDown(s1Cooldown);
            StartCoroutine(SkillCoolingdown(s1Cooldown, true));
        }
        else
        {
            s2.CoolDown(s2Cooldown);
            StartCoroutine(SkillCoolingdown(s2Cooldown, false));
        }
    }

    IEnumerator SkillCoolingdown(float time, bool firstSkill)
    {
        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        canSkill = true;
        if (firstSkill) canSkill1 = true;
        else canSkill2 = true;
    }

}
