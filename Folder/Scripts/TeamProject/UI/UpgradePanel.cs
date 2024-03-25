using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
