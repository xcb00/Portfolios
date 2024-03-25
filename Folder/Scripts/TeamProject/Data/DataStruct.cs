using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum�� ���ڿ��� ��ȯ�� �� ����ȭ�� ���� ���
/// </summary>
public static class EnumCaching
{
    // Enum�� key��, Enum�� ���ڿ��� ��ȯ�� ���� value�� Dictionary�� ����
    private static Dictionary<Enum, string> enum2string = new Dictionary<Enum, string>();

    /// <summary>
    /// Enum�� string���� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="key">String���� ��ȯ�� Enum��</param>
    /// <returns>value�� ���ڿ��� ��ȯ�� ��</returns>
    public static string ToString(Enum key)
    {
        // TryGetValue : Dictionary�� key�� ������ �ִٸ� true ��ȯ�ϰ�, value���� out���� ����
        // Dictionary�� key�� ������ �ִٸ� value���� ��ȯ
        if (enum2string.TryGetValue(key, out string cachedString))
            return cachedString;

        // Dictionary�� key�� ������ ���� �ʴٸ� key�� string���� ��ȯ�� �� Dictionary�� �߰�
        string stringValue = key.ToString();
        // enum2string[key] = stringValue == enum2string.Add(key, stringValue)
        enum2string[key] = stringValue;
        return stringValue;
    }
}

[Serializable]
public class UnitDetails
{
    public float hp;
    public float speed;
    public float damage;
    public float range;
    public float attackSpeed;
    public int price;
}

[Serializable]
public class BossSkillDetails
{
    public ComUnitType type;
    public float skill1Damage;
    public float skill1Cooldown;
    public float skill2Damage;
    public float skill2Cooldown;

    public BossSkillDetails DeepCopy()
    {
        BossSkillDetails copy = new BossSkillDetails();
        copy.type = type;
        copy.skill1Damage = skill1Damage;
        copy.skill2Damage = skill2Damage;
        copy.skill1Cooldown = skill1Cooldown;
        copy.skill2Cooldown = skill2Cooldown;
        return copy;
    }
}

[Serializable]
public class PlayerUnitDetails : UnitDetails
{
    public PlayerUnitType type;
    public float respownTime;

    public PlayerUnitDetails DeepCopy()
    {
        PlayerUnitDetails copy = new PlayerUnitDetails();
        copy.hp = hp;
        copy.speed = speed;
        copy.damage = damage;
        copy.range = range;
        copy.attackSpeed = attackSpeed;
        copy.type = type;
        copy.price = price;
        copy.respownTime = respownTime;
        return copy;
    }
}

[Serializable]
public class ComUnitDetails : UnitDetails
{
    public ComUnitType type;

    public ComUnitDetails DeepCopy()
    {
        ComUnitDetails copy = new ComUnitDetails();
        copy.hp = hp;
        copy.speed = speed;
        copy.damage = damage;
        copy.range = range;
        copy.attackSpeed = attackSpeed;
        copy.type = type;
        copy.price = price;
        return copy;
    }
}

[Serializable]
public struct PoolObject
{
    public GameObject poolObj;
    public PoolType type;
}

[Serializable]
public class StageMonster
{
    public ComUnitType type;
    public Vector2 respawnRange;
    public float GetRespawnTime => UnityEngine.Random.Range(respawnRange.x, respawnRange.y);
}

[Serializable]
public struct StageLevel
{
    public StageMonster[] stageMonsters;
    public StageMonster stageBoss;
    public int[] waves;
}

[Serializable]
public class Item
{
    public ItemName name;
    public int price;
    public float damage;
    public float attackSpeed;
    public float damageEnhance;
    public float attackSpeedEnhance;

    public Item ItemCopy()
    {
        Item copy = new Item();
        copy.name = name;
        copy.price = price;
        copy.damage = damage;
        copy.attackSpeed = attackSpeed;
        copy.damageEnhance = damageEnhance;
        copy.attackSpeedEnhance = attackSpeedEnhance;
        return copy;
    }
}

