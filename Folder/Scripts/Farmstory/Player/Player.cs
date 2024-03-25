using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayerAction
{
    int hp = 5;
    public SceneName scene;
    const string hpPrefab = "PlayerHP";
    [SerializeField] FollowTarget followTarget = null;
    //Coroutine hittingRoutine = null;
    float timeToMove = 0.0f;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent += ActivePlayer;
        EventHandler.BeforeSceneUnloadEvent += InActivePlayer;
        EventHandler.AttackPlayerEvent += GetDamage;
        EventHandler.SeedChangeEvent += SeedChange;
        EventHandler.ChangeSettingEvent += SetCursor;
        EventHandler.SavePlayerPosition += SaveGPGS;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadBeforeFadeInEvent -= ActivePlayer;
        EventHandler.BeforeSceneUnloadEvent -= InActivePlayer;
        EventHandler.AttackPlayerEvent -= GetDamage;
        EventHandler.SeedChangeEvent -= SeedChange;
        EventHandler.ChangeSettingEvent -= SetCursor;
        EventHandler.SavePlayerPosition -= SaveGPGS;
    }

#region Event Function
    void InActivePlayer() 
    {
        myCollider.enabled = false;
        myRenderer.enabled = false;
        cursor.enabled = false;
        transform.GetChild(3).gameObject.SetActive(false);
    }

    void ActivePlayer()
    {
        timeToMove = Settings.Instance.playerTimeToMove;
#if UNITY_EDITOR
        timeToMove *= 0.5f;
#endif

        StopAllCoroutines();
        isCharacterMoving = false;
        CharacterMove(false);
        myRenderer.enabled = true;
        myRenderer.color = Color.white;
        SetCursor();
        myCollider.enabled = true;
        myAnimator.SetTrigger(EnumCaching.ToString(AnimatorParameters.Reset));
        myAnimator.SetBool("playing", false);
        transform.GetChild(3).gameObject.SetActive(true);
        hp = 5;

        if (PlayerPrefs.HasKey(hpPrefab))
            GetDamage();
        GameDatas.playerDie = false;
        transform.position = GameDatas.playerData[0].position;
        //myJoystick.stickDirection = GameDatas.playerData[0].direction;
        myInput.SetInputDirection(GameDatas.playerData[0].direction);
        DirectionChange(GameDatas.playerData[0].direction);
        //followTarget.Init();
        CheckTile();
    }

    void SetCursor()
    {
        if (GameDatas.currentScene == SceneName.Lobby) return;
        cursor.enabled = Settings.Instance.activeCursor;
    }

    void GetDamage(int dmg)
    {
        // 방어력에 따른 미스 처리
        hp -= dmg;

        PlayerPrefs.SetInt(hpPrefab, hp > 0 ? hp : 5); 
        EventHandler.CallUpdateHPBar(hp, 0.2f);

        if (hp > 0)
        {
            myAnimator.SetTrigger(EnumCaching.ToString(AnimatorParameters.Hit));
        }
        else
        {
            StopAllCoroutines();
            GameDatas.pause = true;
            GameDatas.playerDie = true;
            myCollider.enabled = false;
            //if (hittingRoutine != null) StopCoroutine(hittingRoutine);
            myAnimator.SetTrigger(EnumCaching.ToString(AnimatorParameters.Dead));
            //EventHandler.CallSleepPanaltyEvent();
        }
    }

    // HP가 줄어든 상태로 저장했을 경우 게임을 로드할 때 줄어든 상태로 게임을 시작하기 위해 사용
    void GetDamage()
    {
        hp = PlayerPrefs.GetInt(hpPrefab);
        if (hp < 1) hp = 5;
        EventHandler.CallUpdateHPBar(hp, 0.2f);
    }

    void SeedChange(int code) 
    {
        seedCode = code;
        CheckTile();
    }

#endregion

    private void Update()
    {
        if (!GameDatas.pause && !isCharacterMoving && !isFishing)
        {
            CheckDirection();
            // 플레이어가 움직이고 있을 경우
            //if (myJoystick.isMove)
            if(myInput.isMove)
            {
                // 움직이려는 위치가 Collider인지 확인
                if (!SystemTileManager.Instance.TileExist(TilemapType.staticCollider, PlayerCursorCoordinate())
                    && !SystemTileManager.Instance.TileExist(TilemapType.dynamicCollider, PlayerCursorCoordinate()))
                {
                    MoveCharacter(transform.position, myDirection, timeToMove, () => CheckDirection());
                }

                CharacterMove(true);
            }
            // 플레이어가 움직이고 있지 않을 경우
            else
                // 조이스틱을 누르고 있지만 stick이 minDistance를 넘지 않아 idle 상태일 때 isWalking을 false로 만듦
                CharacterMove(false, () => CheckTile());


        }
    }
        

    /// <summary>
    /// 플레이어가 이동한 후 플레이어의 방향 설정
    /// </summary>
    void CheckDirection()
    {
        //if (myDirection != myJoystick.stickDirection)
        //    DirectionChange(myJoystick.stickDirection);
        if (myDirection != myInput.inputDirection)
            DirectionChange(myInput.inputDirection);

        CheckTile();
    }

    void DirectionChange(CharacterDirection direction)
    {
        myDirection = direction;
        PoolManager.Instance.PlayerDir = direction;
        CharacterTurn(myDirection);
        StartCoroutine(TurningCursor(myDirection, Settings.Instance.playerCursorTurnTime));
    }

    void SaveGPGS()
    {
        PlayerData data = new PlayerData();
        data.scene = GameDatas.currentScene;
        data.direction = myDirection;
        data.position = transform.position;
        DataManager.Instance.ChangePlayerData(data, true);
    }

    public CharacterDirection GetPlayerDirection() => myDirection;
}


/* 나의 뻘짓
 * 1. 애니메이션 클립에서 SpriteRenderer의 Color값을 변경 > 플레이어의 방향에 상관 없이 아래방향을 보게 됨
 * 2. 코루틴을 이용해 SpriteRenderer의 Color값 변경 > 색이 바뀌지 않음 // SpriteRenderer의 Color는 현재 Sprite의 Color값을 변경 >> 애니메이션에서 Sprite를 변경하기 때문에 적용 안 됨
 * 3. 코루틴을 이용해 SpriteRenderer.material.color값 변경 > 색이 바뀌지 않음 // 왜 그런지 모르겠음
 * 4. 애니메이션 클립에서 SpriteRenderer.material.color값 변경 : 1번과 동일
 * 5. Idle 애니메이션에서 SpriteRenderer.material.color값 추가로 변경 : 공격 모션 중일 때 플레이어가 피격당할 경우 공격이 무시됨
 * 6. 애니메이터에 Layer를 추가하고 Additive 상태에서 4번 클립 실행 : 성공
 */