using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI turnTxt;
    static MultiManager inst;
    [SerializeField] RoomPanel roomPanel;
    [SerializeField] MultiResult resultPanel;

    // Protected
    [SerializeField] Transform playerDeck;
    GameObject cardObj;
    bool[] playerCards;
    int[] handCards;
    int turn;

    int[] playerResult;
    [SerializeField] ComTable[] otherTables;

    List<int> deck;
    CanvasGroup canvasGroup;
    float adjust;

    public static MultiManager Inst
    {
        get
        {
            if (inst == null) inst = FindObjectOfType<MultiManager>();
            return inst;
        }
    }

    private void Awake()
    {

        float hR = 1920f / Screen.height;
        float wR = 1080f / Screen.width;
        adjust = hR > wR ? hR : wR;
    }

    private void Start()
    {
        roomPanel.EnterRoom(PhotonNetwork.CurrentRoom.Name);
        cardObj = Resources.Load("MultiCard") as GameObject;
        canvasGroup = playerDeck.GetComponent<CanvasGroup>();

        playerCards = new bool[] { false, false, false, false };
        playerResult = new int[] { 0, 0, 0 };
        turn = 0;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        EventHandler.CheckPlayerEvent += CheckPlayer;
    }

    public override void OnDisable()
    {
        EventHandler.CheckPlayerEvent -= CheckPlayer;
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (playerCards[i])
                playerDeck.GetChild(i).GetChild(0).GetComponent<PlayerCard>().DestoryCard();
        }
        base.OnDisable();
    }

    void CheckPlayer()
    {
        if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
            photonView.RPC("InitGame", RpcTarget.MasterClient);
    }

    #region PunRPC

    [PunRPC]
    void InitGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        InitDeck();
        handCards = DrawHandCard();
        photonView.RPC("StartGame", RpcTarget.Others, DrawHandCard());
        InactiveRoomPanel();
        canvasGroup.blocksRaycasts = true;
        turnTxt.SetText("My Turn");
    }

    [PunRPC]
    void StartGame(int[] handCards)
    {
        this.handCards = handCards;
        InactiveRoomPanel();
        canvasGroup.blocksRaycasts = false;
        turnTxt.SetText(string.Empty);
    }

    [PunRPC]
    void TurnOver(int tableType, int tableIdx, int cardNum)
    {
        otherTables[tableType].PutCard(cardNum, tableIdx);
        if (turn > 5)
            return;
        canvasGroup.blocksRaycasts = true;
        turnTxt.SetText("My Turn");
    }

    [PunRPC]
    void EndMyTurn(int turn)
    {
        if (this.turn > 5 && turn > 5)
            photonView.RPC("ShowResult", RpcTarget.All);
    }

    [PunRPC]
    void ShowResult()
    {
        GameManager.Inst.StartPlaying(100);
        int result = 0;
        int max = otherTables[0].GetResult;
        int min = otherTables[1].GetResult;
        int gap = otherTables[2].GetResult;
        // Max
        result += (playerResult[0] > max ? 1 : (playerResult[0] == max ? 0 : -1));
        // Min
        result += (playerResult[1] < min ? 1 : (playerResult[1] == min ? 0 : -1));
        // Gap
        result += (playerResult[2] < gap ? 1 : (playerResult[2] == gap ? 0 : -1));

        GameResult _result = result > 0 ? GameResult.Win : (result == 0 ? GameResult.Draw : GameResult.Lose);
        resultPanel.SetResultPanel(_result);
    }


    #endregion

    #region Protected
    void InitDeck() // protected
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

    int DrawFromDeck()
    {
        int _idx = Random.Range(0, deck.Count);
        int result = deck[_idx];
        deck.RemoveAt(_idx);
        return result;
    }

    #endregion
    void DrawPlayerCard(bool first)
    {
        int _idx = first ? 0 : 4;

        for (int i = 0; i < 4; i++)
        {
            if (playerCards[i]) continue;
            if (first)
                Instantiate(cardObj, playerDeck.GetChild(i)).GetComponent<PlayerCard>().Initialise(handCards[_idx++], i, (idx) => DropCard(idx), adjust);
            else
                playerDeck.GetChild(i).GetChild(0).GetComponent<PlayerCard>().Initialise(handCards[_idx++], i, (idx) => DropCard(idx), adjust);
            
            playerCards[i] = true;
        }
    }

    void InactiveRoomPanel()
    {
        roomPanel.Inactive();
        DrawPlayerCard(true);
    }

    int[] DrawHandCard()
    {
        int[] result = new int[7];
        for (int i = 0; i < result.Length; i++)
            result[i] = DrawFromDeck();
        return result;
    }

    void DropCard(int idx)
    {
        turnTxt.SetText(string.Empty);
        canvasGroup.blocksRaycasts = false;
        playerCards[idx] = false;

        if (++turn == 3 || turn == 6)
            Invoke("DelayDraw", 0.1f);
    }

    void DelayDraw()
    {
        if (turn == 3)
            DrawPlayerCard(false);
        else if (turn == 6)
            EndGame();

    }

    void EndGame()
    {
        canvasGroup.blocksRaycasts = false;
        turnTxt.SetText("End my turn");
        photonView.RPC("EndMyTurn", RpcTarget.Others, turn);
    }

    public void PutCard(Vector2Int tableIndex, int num)
    {
        if(tableIndex.x < 2)
            playerResult[tableIndex.x] += num;
        else
            playerResult[tableIndex.x] = Mathf.Abs(num - playerResult[tableIndex.x]);

        photonView.RPC("TurnOver", RpcTarget.Others, tableIndex.x, tableIndex.y, num);
    }
}