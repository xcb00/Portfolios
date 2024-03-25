using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : AnimalState
{
    [SerializeField] int animalHP;
    [SerializeField] Vector2Int animalIdle = new Vector2Int(6, 10);
    public override void Respawn(Vector3 position)
    {
        hp = animalHP;
        idleTime = animalIdle;
        base.Respawn(position);
    }
}
