using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerItem", menuName = "Scriptable Object/Item")]
public class ItemSO : ScriptableObject
{
    public List<Item> itemList;
    public int[] enhanceProbability;
}
