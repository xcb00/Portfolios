using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterProperty, IDamage
{
    #region Player Value
    [Header("Player Value")]
    [SerializeField] float walkSpd = 1.0f;
    [SerializeField] float runSpd = 2.0f;
    [SerializeField] float attackRange = 3.0f;
    [SerializeField] float damage = 0.0f;
    [SerializeField] float s1Damage = 2.0f;
    [SerializeField] float s2Damage = 1.5f;
    [SerializeField] float xMinPos = -40f;
    [SerializeField] Cooldown attack;
    public Item item { get; set; }
    [SerializeField] Transform weapon;
    #endregion

    float inputVar = 0.0f;
    float knockbackTime = -1f;
    bool isRun = false;
    Coroutine movingPlayer = null;
    Coroutine hittingPlayer = null;
    Coroutine hittingEffect = null;
    Coroutine attackDelay = null;
    Collider[] targets;
    private void OnEnable()
    {
        EventHandler.PlayerMoveEvent += MovePlayer;
        EventHandler.PlayerAttackEvent += AttackPlayer;
        EventHandler.PlayerEquipmentEvent += Equipment;
        EventHandler.StageClearEvent += StageClear;

    }

    private void OnDisable()
    {
        EventHandler.PlayerMoveEvent -= MovePlayer;
        EventHandler.PlayerAttackEvent -= AttackPlayer;
        EventHandler.PlayerEquipmentEvent -= Equipment;
        EventHandler.StageClearEvent -= StageClear;
    }
    void StageClear()
    {
        ShowIcon(Color.blue);
        transform.position = new Vector3(-34f, 0f, 1.5f);
    }

    private void Start()
    {
        ShowIcon(Color.blue);
        Equipment(0);
    }

    void Equipment(int index)
    {
        item = GameDatas.itemList[index]; damage = item.damage; 
        for (int i = 0; i < weapon.childCount; i++)
            weapon.GetChild(i).gameObject.SetActive(i == index);
    }

    #region PlayerMove
    void MovePlayer(float inputValue, bool isRun)
    {
        inputVar = inputValue;
        this.isRun = isRun;

        if (myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
            return;

        if (movingPlayer != null) StopMoving();

        transform.GetChild(0).transform.rotation = Quaternion.identity;
        if (!Mathf.Approximately(inputValue, 0f))
        {
            if(inputValue < 0f && Mathf.Abs(transform.GetChild(0).transform.rotation.y)<1f) 
                transform.GetChild(0).transform.Rotate(Vector3.up * 180f);
            myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.walking), true);
            myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.running), isRun);
            movingPlayer = StartCoroutine(MovingPlayer(inputValue, isRun));
        }
    }

    void StopMoving()
    {
        myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.walking), false);
        myAnimator.SetBool(EnumCaching.ToString(AnimationParameters.running), false);
        StopCoroutine(movingPlayer);
    }

    IEnumerator MovingPlayer(float inputValue, bool isRun)
    {
        while (true)
        {
            if (transform.position.x < xMinPos && inputValue < 0f) break;
            if (transform.position.x > 45f && inputValue > 0f) break;

            transform.Translate((inputValue > 0f ? Vector3.right : Vector3.left) * (isRun ? runSpd : walkSpd) * Time.deltaTime);
            yield return null;
        }
    }
    #endregion
    #region PlayerAttack
    void AttackPlayer(int type)
    {
        if (movingPlayer != null) StopMoving();//StopCoroutine(movingPlayer);
        StartCoroutine(AttackingPlayer(type));
        if (type == 0)
        {
            attack.UseSkill();
            attack.CoolDown(item.attackSpeed); // Attack Icon Cool Down
        }
    }

    IEnumerator Delaying()
    {
        float t = item.attackSpeed;
        while (t > 0.0f) { t -= Time.deltaTime; yield return null; }
        EventHandler.CallCanAttackEvent(true);
    }

    IEnumerator AttackingPlayer(int type)
    {
        // 공격 애니메이션
        myAnimator.SetTrigger(EnumCaching.ToString((AnimationParameters)type));

        // 딜레이 코루틴 실행
        attackDelay= StartCoroutine(Delaying());
        yield return null;


        while (!myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
            yield return null;

        float time = myAnimator.GetFloat("length");
        //Debug.Log(time);
        while (time > 0.0f) { time -= Time.deltaTime; yield return null; }
        yield return null;

        if (type > 0)
        {
            EventHandler.CallCanUseSkillEvent(true);
            if (type == 1) EventHandler.CallCoolDownEvent(true);
            else if (type == 2) EventHandler.CallCoolDownEvent(false);
        }
        if (Mathf.Abs(inputVar)>0.0f) MovePlayer(inputVar, isRun);
    }

    void FindTargets() { targets = Physics.OverlapSphere(transform.GetChild(1).transform.position + (inputVar >= 0f ? Vector3.right : Vector3.left) * attackRange * 0.5f, attackRange * 0.5f, 1 << (int)LayerName.ComUnit); }

    public void AttackTarget()
    {
        FindTargets();
        if(targets.Length>0)
            targets[0].GetComponentInParent<IDamage>().GetDamage(item.damage);
    }



    public void FirstSkill()
    {
        FindTargets();
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;
            targets[i].GetComponentInParent<IDamage>().GetDamage(item.damage * s1Damage);
        }
    }

    public void SecondSkill()
    {
        FindTargets();
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;
            targets[i].GetComponentInParent<IDamage>().GetDamage(item.damage * s2Damage, 20);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.GetChild(1).transform.position + (inputVar >= 0f ? Vector3.right : Vector3.left) * attackRange * 0.5f, attackRange * 0.5f);
    }
#endif

#endregion
    #region PlayerHit

    public void GetDamage(float damage, int possibility = 0)
    {
        if (hittingEffect != null) StopCoroutine(hittingEffect);
        hittingEffect = StartCoroutine(CharacterHitting());

        if (hittingPlayer == null) //StopCoroutine(hittingPlayer);
            hittingPlayer = StartCoroutine(KnockBacking());
        //myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.knockback));
    }

    protected IEnumerator KnockBacking()
    {
        myAnimator.SetTrigger(EnumCaching.ToString(AnimationParameters.knockback));
        yield return null;

        if (knockbackTime < 0f)
        {
            while (!myAnimator.GetBool(EnumCaching.ToString(AnimationParameters.playing)))
                yield return null;

            knockbackTime = myAnimator.GetFloat("length");
        }
        float backDist = 0.8f;
        float timing = 0.0f;
        Vector3 from = transform.position;
        Vector3 to = transform.position - Vector3.right * backDist;
        StartCoroutine(CharacterHitting(knockbackTime * 0.5f));
        while (timing < knockbackTime)
        {
            if (transform.position.x < xMinPos) break;
            timing = timing + Time.deltaTime > knockbackTime ? knockbackTime : timing + Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, timing / knockbackTime);
            yield return null;
        }
        inputVar = 0;
        hittingPlayer = null;
    }
    #endregion

}

