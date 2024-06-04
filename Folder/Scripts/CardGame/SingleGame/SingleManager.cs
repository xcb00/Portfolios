using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SingleManager : MonoBehaviour
{
    [SerializeField] UnityEvent SetComCard;
    [SerializeField] UnityEvent PutComCard;
    [SerializeField] Transform playerDeck;
    [SerializeField] SingleResult resultPanel;
    GameObject cardObj;
    bool[] playerCards;
    int turn;

    int[] playerResult;
    int[] comResult;

    List<int> deck;
    float adjust;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        float hR = 1920f / Screen.height;
        float wR = 1080f / Screen.width;
        adjust = hR > wR ? hR : wR;
        cardObj = Resources.Load("Prefab/Card") as GameObject;
        canvasGroup = playerDeck.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;

        InitDeck();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        for(int i=0;i<playerCards.Length;i++)
        {
            if (playerCards[i])
                playerDeck.GetChild(i).GetChild(0).GetComponent<PlayerCard>().DestoryCard();
        }
    }

    private void Start()
    {
        playerCards = new bool[] { false, false, false, false };
        playerResult = new int[] { 0, 0, 0 };
        comResult = new int[] { 0, 0, 0 };
        turn = 0;
        SetComCard?.Invoke();
    }

    void InitDeck() 
    {
        int _num = 0;
        deck = new List<int>();
        for (int i = 1; i < 61; i++)
        {
            _num = i % 30;
            if (_num == 0)
                _num = 30;
            deck.Add(_num);
        }
    }

    public void PlayingButton() => DrawPlayerCard(true);

    public int DrawFromDeck()
    {
        int _idx = Random.Range(0, deck.Count);
        int result = deck[_idx];
        deck.RemoveAt(_idx);
        return result;
    }

    void DrawPlayerCard(bool first)
    {
        for (int i = 0; i < 4; i++)
        {
            if (playerCards[i]) continue;

            if (first)
                Instantiate(cardObj, playerDeck.GetChild(i)).GetComponent<PlayerCard>().Initialise(DrawFromDeck(), i, (idx) => DropCard(idx), adjust);
            else
                playerDeck.GetChild(i).GetChild(0).GetComponent<PlayerCard>().Initialise(DrawFromDeck(), i, (idx) => DropCard(idx), adjust);
            playerCards[i] = true;
        }
    }

    void DropCard(int idx)
    {
        StartCoroutine(DropingCard(idx));
    }

    void ActiveResultPanel()
    {
        StopAllCoroutines();
        int result = 0;
        for (int i = 0; i < 3; i++)
            result += (int)CheckResult(i);

        GameResult _result = GameResult.Win;
        if (result > 0)
            _result = GameResult.Win;
        else if (result == 0)
            _result = GameResult.Draw;
        else
            _result = GameResult.Lose;
        resultPanel.SetResultPanel(_result, Mathf.Abs(result) == 3);
    }

    IEnumerator DrawReepeating()
    {
        float t = 0.3f;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }
        DrawPlayerCard(false);
    }

    IEnumerator DropingCard(int idx)
    {
        canvasGroup.blocksRaycasts = false;
        playerCards[idx] = false;

        float t = 0.5f;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }
        PutComCard?.Invoke();

        if (++turn == 3)
            yield return DrawReepeating();
        if (turn == 6)
            ActiveResultPanel();
        
        canvasGroup.blocksRaycasts = true;
    }

    public void SetPlayerResult(TableType type, int result) => playerResult[(int)type] = result;
    public void SetComResult(TableType type, int result) => comResult[(int)type] = result;

    GameResult CheckResult(int idx)
    {
        if (playerResult[idx] > comResult[idx]) return idx < 1 ? GameResult.Win : GameResult.Lose;
        else if (playerResult[idx] == comResult[idx]) return GameResult.Draw;
        else return idx < 1 ? GameResult.Lose : GameResult.Win;
    }
}
