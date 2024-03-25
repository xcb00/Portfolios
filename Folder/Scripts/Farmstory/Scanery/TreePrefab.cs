using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePrefab : MonoBehaviour
{
    SpriteRenderer[] renderers;
    Animator[] animator;
    int fruitCode;
    int dayCount;
    int hp;
    TreeState state;
    Vector2Int coordinate;
    bool fading = false;

    void SpawnTree()
    {

        transform.position = Utility.CoordinateToPosition(coordinate);
        gameObject.SetActive(true);

        if ((int)state > 1)
            animator[0].SetTrigger("Reset");
        if ((int)state > 2)
            animator[1].SetTrigger("Reset");

        EventHandler.CallSetTileEvent(coordinate, TilemapType.dynamicCollider);
        EventHandler.CallSetTileToListEvent(TilemapType.dynamicCollider);
    }

    public void SetTree(int fruit, Vector2Int coordinate)
    {
        if (renderers == null)
            renderers = GetComponentsInChildren<SpriteRenderer>();
        if (animator == null)
            animator = GetComponentsInChildren<Animator>();

        fruitCode = fruit;
        dayCount = 0;
        hp = 0;
        state = TreeState.sapling;
        this.coordinate = coordinate;


        StateChange();
        SpawnTree();        
    }

    public void SetTree(TreePrefabData data)
    {
        if(renderers==null)
            renderers = GetComponentsInChildren<SpriteRenderer>();
        if(animator==null)
            animator = GetComponentsInChildren<Animator>();

        if (fading)
        {
            Debug.Log("Fading is true");
            for (int i = 0; i < renderers.Length; i++) renderers[i].color = Vector4.one;
        }

        renderers[1].sortingLayerName = "Front";

        fruitCode = data.fruitCode;
        coordinate = data.coordinate;
        int cnt = Utility.DayGap(data.lastVisit);

        if (cnt == 0)
        {
            hp = data.stateHP;
            dayCount = data.dayCount;
            state = (TreeState)data.state;
        }
        else
        {
            dayCount = data.dayCount + cnt;
            int _state = data.state;
            while (_state < (int)TreeState.fruit)
            {
                // 功格/关嫡老 锭
                if (_state < (int)TreeState.tree)
                {
                    if (dayCount >= GameDatas.treeDataDic[fruitCode].firstGrowth)
                    {
                        dayCount -= GameDatas.treeDataDic[fruitCode].firstGrowth;
                        _state = (int)TreeState.tree;
                    }
                    else
                        break;
                }
                // 唱公老 锭
                else if (_state == (int)TreeState.tree)
                {
                    if (GameDatas.treeDataDic[fruitCode].fruitSeason == GameDatas.YearSeasonDay.y && dayCount >= GameDatas.treeDataDic[fruitCode].secondGrowth) // && GameDatas.treeDataDic[fruitCode].season == GameDatas.YearSeasonDay.y
                    {
                        dayCount = 0;
                        _state = (int)TreeState.fruit;
                    }
                    else
                        break;
                }
            }

            state = (TreeState)_state;
            SetHP();
        }

        SpawnTree();
    }

    public TreePrefabData GetPrefabData() => new TreePrefabData(fruitCode, dayCount, (int)state, hp, coordinate, (GameDatas.YearSeasonDay + (GameDatas.HourMinuteGold.x<7?-Vector3Int.forward:Vector3Int.zero)));

    public void StateChange(TreeDropItem drop = TreeDropItem.none)
    {
        switch (state)
        {
            case TreeState.sapling:
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = i == 0;
                renderers[0].sprite = Utility.GetSprite("trees_2");
                break;
            case TreeState.stump:
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = i == 0;
                renderers[0].sprite = Utility.GetSprite("trees_0");
                break;
            case TreeState.tree:
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = i < 2;
                renderers[0].sprite = Utility.GetSprite("trees_0");
                break;
            case TreeState.fruit:
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = true;
                renderers[0].sprite = Utility.GetSprite("trees_0");

                Sprite sp = Utility.GetSprite(GameDatas.treeDataDic[fruitCode].spriteName);
                for (int i = 2; i < renderers.Length; i++)
                    renderers[i].sprite = sp;
                break;
            default:
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].enabled = false;
                PoolManager.Instance.EnqueueObject(PoolPrefabName.tree, gameObject);
                EventHandler.CallRemoveTreeAtTreeData(coordinate);
                break;
        }

        //if (drop == TreeDropItem.wood) PoolManager.Instance.SpawnItem(20001, transform.position);
        //else if (drop == TreeDropItem.fruit) PoolManager.Instance.SpawnItem(fruitCode, transform.position);
    }

    public bool OnHit(int dmg = 1)
    {
        hp -= dmg;

        bool destroy = false;
        bool playingAnimation = false;
        if (hp <= 0)
        {
            switch (state)
            {
                case TreeState.sapling:
                    state = TreeState.die;
                    destroy = true;
                    break;
                case TreeState.stump:
                    PoolManager.Instance.SpawnItem(20001, transform.position);
                    state = TreeState.die;
                    destroy = true;
                    break;
                case TreeState.tree:
                    animator[0].SetTrigger("FellDown");
                    StartCoroutine(Utility.DelayCall(0.5f, () => PoolManager.Instance.SpawnItem(20001, transform.position)));
                    state = TreeState.stump;
                    destroy = false;
                    playingAnimation = true;
                    break;
                case TreeState.fruit:
                    animator[1].SetTrigger("FellFruit");
                    StartCoroutine(Utility.DelayCall(0.18f, () => PoolManager.Instance.SpawnItem(fruitCode, transform.position)));
                    EventHandler.CallIncrementAchievementEvent(GPGSIds.achievement_farmer_master);
                    EventHandler.CallUnlockAchievementEvent(GPGSIds.achievement_beginner_farmer);
                    state = TreeState.tree;
                    destroy = false;
                    playingAnimation = true;
                    break;
            }
            SetHP(false);
        }
        else if (state == TreeState.tree)
        {
            playingAnimation = true;
            animator[0].SetTrigger("Hit");
        }
        if(!playingAnimation)
            StateChange();
        return destroy;
    }
    void SetHP(bool stateChange = true)
    {
        switch (state)
        {
            case TreeState.stump:
                hp = GameDatas.treeDataDic[fruitCode].stumpHP;
                break;
            case TreeState.tree:
                hp = GameDatas.treeDataDic[fruitCode].treeHP;
                break;
            default:
                hp = 0;
                break;
        }
        if(stateChange)
            StateChange();
    }

    public void FadingTree(bool playerEnter) 
    {
        StopAllCoroutines();
        if (!gameObject.activeSelf) return;
        for (int i = 1; i < renderers.Length; i++)
        {
            if (renderers[i].enabled)
            {
                fading = true;
                renderers[i].sortingLayerName = "Player";
                StartCoroutine(Utility.Fading(renderers[i], (playerEnter ? new Color(1f, 1f, 1f, Settings.Instance.scaneryAlpha) : Color.white), Settings.Instance.scaneryFadeTime));
            }
        }
    }

    public void ChangeSortingLayer(bool playerEnter)
    {
        renderers[1].sortingLayerName = playerEnter ? "Instantiate" : "Front";
    }

    
}
