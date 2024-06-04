using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterStatus 
{
    public MonsterCharacter type;
    public int hp;
    public int damage;
    public float speed;
    public float attackCooldown;
    public float attackDistance;
    public float detectDistance;
    public MonsterStatus(MonsterStatus origin)
    {
        type = origin.type;
        hp = origin.hp;
        damage = origin.damage;
        speed = origin.speed;
        attackCooldown = origin.attackCooldown;
        attackDistance = origin.attackDistance;
        detectDistance = origin.detectDistance;
    }
}

[System.Serializable]
public class PlayerCharacterStatus
{
    public PlayerCharacter type;
    public int hp;
    public int damage;
    public int exp;
    public float spawnTime;
    public float speed;
    public float attackCooldown;
    public float attackDistance;
    public float skillCooldown;
    public float skillDistance;

    public PlayerCharacterStatus(PlayerCharacterStatus origin)
    {
        type = origin.type;
        spawnTime = origin.spawnTime;
        hp = origin.hp;
        damage = origin.damage;
        exp = origin.exp;
        speed = origin.speed;
        attackCooldown = origin.attackCooldown;
        attackDistance = origin.attackDistance;
        skillCooldown = origin.skillCooldown;
        skillDistance = origin.skillDistance;
    }
}

[CreateAssetMenu(fileName ="CharacterStatusSO", menuName ="ScriptableSO/CharacterStatusSO")]
public class CharacterSO : ScriptableObject
{
    public MonsterStatus[] monsters;
    public PlayerCharacterStatus[] playerCharacters;
    public PlayerCharacterStatus GetCharacterStatus(PlayerCharacter character)
    {
        foreach (PlayerCharacterStatus status in playerCharacters)
            if (status.type == character) return new PlayerCharacterStatus(status);

        return null;
    }

    public MonsterStatus GetCharacterStatus(MonsterCharacter character)
    {
        foreach (MonsterStatus status in monsters)
            if (status.type == character) return new MonsterStatus(status);
        return null;
    }

#if UNITY_EDITOR
    List<PlayerCharacter> playerType = new List<PlayerCharacter>();
    List<MonsterCharacter> monsterType = new List<MonsterCharacter>();
    private void OnValidate()
    {
        playerType.Clear();
        foreach(PlayerCharacterStatus character in playerCharacters)
        {
            if (character.type == PlayerCharacter.None)
                Debug.LogWarning("CharacterSO's playerCharacters contains None type");
            else if (playerType.Contains(character.type))
                Debug.LogWarning($"CharacterSO's playerCharacters contains duplicate type. {character.type} is a duplicate vaule.");
            else
                playerType.Add(character.type);
        }

        monsterType.Clear();
        foreach (MonsterStatus character in monsters)
        {
            if (character.type == MonsterCharacter.None)
                Debug.LogWarning("CharacterSO's monsters contains None type");
            else if (monsterType.Contains(character.type))
                Debug.LogWarning($"CharacterSO's monsters contains duplicate type. {character.type} is a duplicate vaule.");
            else
                monsterType.Add(character.type);
        }
    }
#endif
}
