using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMask : MonoBehaviour
{
    SpriteMask mask;
    private void OnEnable()
    {
        mask = GetComponent<SpriteMask>();
        EventHandler.NightMaskSwitch += MaskSwitch;
    }
    private void OnDisable()
    {
        EventHandler.NightMaskSwitch -= MaskSwitch;
    }

    void MaskSwitch(bool isNight)
    {
        mask.enabled = isNight;
    }
}
