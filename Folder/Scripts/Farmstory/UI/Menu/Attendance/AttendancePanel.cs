using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttendancePanel : MenuPanel, IPointerClickHandler
{
    [SerializeField] AttendanceSlot[] slots = null;
    public bool alreadyReceive = true;
    AttendanceData data;
    int max = 0;

    void Start()
    {
        max = Settings.Instance.maxQuantity;
        Init();
        slots = GetComponentsInChildren<AttendanceSlot>();
    }

    public override void ActivePanel()
    {
        LoadAttendance();
        base.ActivePanel();
    }

    // LoadAttendance : 출석체크 창을 열 때 함수 실행
    void LoadAttendance()
    {
        data = DataManager.Instance.LoadAttendance();

        if (data.lastVisit != GameDatas.nowDay)
        {
            if (data.visitCount >= 7)
            {
                data.visitCount = 0;
                data.receiveCount = 0;
            }
            else
                data.visitCount += 1;
        }
        else
        {
            Debug.Log("이미 받음");
        }

        SetAttendanceSlot();
    }
    void SetAttendanceSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSlot(GameDatas.attendanceReward[i].x, GameDatas.attendanceReward[i].y, i < data.receiveCount);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (data.receiveCount == 0 && data.visitCount == 0)
        {
            data.visitCount += 1;
            ReceiveReward();
        }
        else if (data.receiveCount != data.visitCount)
            ReceiveReward();
        else
            Debug.Log("이미 수령함");
    }

    void ReceiveReward()
    {
        Debug.Log("아이템 수령");

        int idx = 0;
        if (data.receiveCount >= 7) {
            data.receiveCount = 0;
        }
        else
        {
            idx = data.receiveCount;
            data.receiveCount += 1;
        }
        data.visitCount = data.receiveCount;
        data.lastVisit = GameDatas.nowDay;
        DataManager.Instance.SaveAttendance(data);

        // 인벤토리에 아이템 추가
        if (GameDatas.attendanceReward[idx].x >= 99999)
            Utility.GoldChange(GameDatas.attendanceReward[idx].y);
        else 
        {
            int code = GameDatas.attendanceReward[idx].x;
            int quantity = GameDatas.attendanceReward[idx].y;
            while (quantity > 0)
            {
                int cnt = InventoryManager.Instance.GetItemStackableWithItemCode(code) ? (quantity>max?max:quantity) : 1;
                InventoryManager.Instance.AddItem(Inventories.bag, GameDatas.attendanceReward[idx].x, cnt);
                quantity -= cnt;
            }
        }

        // 인벤토리 저장
        DataManager.Instance.SaveInventoryData((int)Inventories.bag);
        DataManager.Instance.SaveTimeData();

        SetAttendanceSlot();
    }
}
