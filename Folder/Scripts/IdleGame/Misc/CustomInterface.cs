using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    public void OnDamage(int damage, float stun = -1f);
}
