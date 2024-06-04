using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollContent : MonoBehaviour
{
    [SerializeField] Scrollbar bar = null;

    private void OnEnable() =>bar.value = 1.0f;
}
