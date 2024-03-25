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
        // ���¿� ���� �̽� ó��
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

    // HP�� �پ�� ���·� �������� ��� ������ �ε��� �� �پ�� ���·� ������ �����ϱ� ���� ���
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
            // �÷��̾ �����̰� ���� ���
            //if (myJoystick.isMove)
            if(myInput.isMove)
            {
                // �����̷��� ��ġ�� Collider���� Ȯ��
                if (!SystemTileManager.Instance.TileExist(TilemapType.staticCollider, PlayerCursorCoordinate())
                    && !SystemTileManager.Instance.TileExist(TilemapType.dynamicCollider, PlayerCursorCoordinate()))
                {
                    MoveCharacter(transform.position, myDirection, timeToMove, () => CheckDirection());
                }

                CharacterMove(true);
            }
            // �÷��̾ �����̰� ���� ���� ���
            else
                // ���̽�ƽ�� ������ ������ stick�� minDistance�� ���� �ʾ� idle ������ �� isWalking�� false�� ����
                CharacterMove(false, () => CheckTile());


        }
    }
        

    /// <summary>
    /// �÷��̾ �̵��� �� �÷��̾��� ���� ����
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


/* ���� ����
 * 1. �ִϸ��̼� Ŭ������ SpriteRenderer�� Color���� ���� > �÷��̾��� ���⿡ ��� ���� �Ʒ������� ���� ��
 * 2. �ڷ�ƾ�� �̿��� SpriteRenderer�� Color�� ���� > ���� �ٲ��� ���� // SpriteRenderer�� Color�� ���� Sprite�� Color���� ���� >> �ִϸ��̼ǿ��� Sprite�� �����ϱ� ������ ���� �� ��
 * 3. �ڷ�ƾ�� �̿��� SpriteRenderer.material.color�� ���� > ���� �ٲ��� ���� // �� �׷��� �𸣰���
 * 4. �ִϸ��̼� Ŭ������ SpriteRenderer.material.color�� ���� : 1���� ����
 * 5. Idle �ִϸ��̼ǿ��� SpriteRenderer.material.color�� �߰��� ���� : ���� ��� ���� �� �÷��̾ �ǰݴ��� ��� ������ ���õ�
 * 6. �ִϸ����Ϳ� Layer�� �߰��ϰ� Additive ���¿��� 4�� Ŭ�� ���� : ����
 */