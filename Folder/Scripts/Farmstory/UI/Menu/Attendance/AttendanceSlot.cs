using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttendanceSlot : MonoBehaviour
{
    [SerializeField] Image rewardImg = null;
    [SerializeField] TextMeshProUGUI amount = null;

    // Start is called before the first frame update
    void Start()
    {
        rewardImg = transform.GetChild(0).GetComponent<Image>();
        amount = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void SetSlot(int itemCode, int itemAmount, bool receive = false)
    {
        rewardImg.sprite = InventoryManager.Instance.GetItemSpriteWithItemCode(itemCode);
        amount.SetText(itemAmount.ToString());
        transform.GetChild(2).gameObject.SetActive(receive);
    }
}
