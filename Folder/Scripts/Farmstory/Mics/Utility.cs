using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Utility
{
    #region Summary
    /// <summary>
    /// CharacterDirection을 Vector3의 값으로 변환하는 함수
    /// </summary>
    #endregion
    public static Vector3 CharacterDirectionToVector3(CharacterDirection direction)
    {
        switch (direction)
        {
            case CharacterDirection.up: return Vector3.up;
            case CharacterDirection.down: return Vector3.down;
            case CharacterDirection.left: return Vector3.left;
            case CharacterDirection.right: return Vector3.right;
            //case CharacterDirection.zero:
            default:
                return Vector3.zero;
        }
    }

    public static Vector2 CharacterDirectionToVector2(CharacterDirection direction)
    {
        switch (direction)
        {
            case CharacterDirection.up: return Vector2.up;
            case CharacterDirection.down: return Vector2.down;
            case CharacterDirection.left: return Vector2.left;
            case CharacterDirection.right: return Vector2.right;
            //case CharacterDirection.zero:
            default:
                return Vector2.zero;
        }
    }

    public static Vector2Int CharacterDirectionToVector2Int(CharacterDirection direction)
    {
        switch (direction)
        {
            case CharacterDirection.up: return Vector2Int.up;
            case CharacterDirection.down: return Vector2Int.down;
            case CharacterDirection.left: return Vector2Int.left;
            case CharacterDirection.right: return Vector2Int.right;
            //case CharacterDirection.zero:
            default:
                return Vector2Int.zero;
        }
    }

    public static CharacterDirection Vector2IntToCharacterDirection(Vector2Int coordinate)
    {
        if (coordinate.x > 0 && coordinate.y == 0) return CharacterDirection.right;
        else if (coordinate.x < 0 && coordinate.y == 0) return CharacterDirection.left;
        else if (coordinate.x == 0 && coordinate.y > 0) return CharacterDirection.up;
        else if (coordinate.x == 0 && coordinate.y < 0) return CharacterDirection.down;
        else return CharacterDirection.zero;
    }

    /*public static Vector3 TargetPositionToVector3(Vector3 position, CharacterDirection direction)
    {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z))+ CharacterDirectionToVector3(direction);
    }*/

    // Tilemap을 x, y 방향으로 0.5씩 이동했기 때문에 Wolrd Position과 Tilemap의 coordinate 사이에 (1, 1)의 차이값 발생
    public static Vector3 CoordinateToPosition(Vector2Int vec) => new Vector3(vec.x + 1f, vec.y + 1f, 0f);
    public static Vector2Int PositionToCoordinate(Vector3 vec) => new Vector2Int((int)vec.x, (int)vec.y);
    public static Vector2Int PositionToCoordinate(Vector3 vec, CharacterDirection dir)
    {
        Vector2Int _dir = CharacterDirectionToVector2Int(dir);
        return new Vector2Int((int)vec.x - 1, (int)vec.y - 1) + _dir; 
    }

    //public static Vector3 Vector2ToVector3(Vector2 vec)=>new Vector3(vec.x, )

    /// <summary>
    /// ysd가 현재 날짜와 같을 경우 false 반환
    /// </summary>
    /// <param name="ysd"></param>
    /// <returns></returns>
    public static bool CompareDay(Vector3Int ysd)
    {
        if (ysd.z != GameDatas.YearSeasonDay.z) return true;
        else
        {
            if (ysd.y != GameDatas.YearSeasonDay.y) return true;
            else
            {
                if (ysd.x != GameDatas.YearSeasonDay.x) return true;
                else return false;
            }
        }
    }

    public static Vector3Int GetDayData() => new Vector3Int(GameDatas.YearSeasonDay.x, GameDatas.YearSeasonDay.y, GameDatas.YearSeasonDay.z);

    public static int YSDtoDay(Vector3Int ysd) => ((ysd.x - 1) * 4 + ysd.y)  * Settings.Instance.monthDays + ysd.z;
    public static int DayGap(Vector3Int date) => YSDtoDay(GameDatas.YearSeasonDay) - YSDtoDay(date);

    //public static bool GoldChange(int gold) => GoldChange(ref gold);
    public static Sprite GetSprite(string name_idx)
    {
        string[] str = name_idx.Split('_');
        switch (str[0])
        {
            case "crops": return GameDatas.crops[int.Parse(str[1])];
            case "cropIcon": return GameDatas.cropIcon[int.Parse(str[1])];
            case "seeds": return GameDatas.seeds[int.Parse(str[1])];
            case "mining"://mining
                return GameDatas.mining[int.Parse(str[1])];
            case "trees": return GameDatas.trees[int.Parse(str[1])];
            case "items": return GameDatas.items[int.Parse(str[1])];
            case "fish": return GameDatas.fish[int.Parse(str[1])];
            case "coin": return GameDatas.coin[int.Parse(str[1])];
            case "tools16": return GameDatas.tools16[int.Parse(str[1])];
            default: return null;
        }
    }

    public static bool GoldChange(int gold)
    {
        if (gold < 0 && -gold > GameDatas.HourMinuteGold.z)
        {
            EventHandler.CallPrintSystemMassageEvent("골드가 부족합니다");
            gold = 0;
            return false;
        }

        Vector3Int hmg = GameDatas.HourMinuteGold;
        if (hmg.z + gold > 1000000000)
        {
            gold = (hmg.z + gold) - 1000000000;
            EventHandler.CallPrintSystemMassageEvent("보유 골드 한도를 초과합니다");
            GameDatas.HourMinuteGold = new Vector3Int(hmg.x, hmg.y, 1000000000);
        }
        else if (hmg.z + gold < 0)
        {
            gold = 0;
            GameDatas.HourMinuteGold = new Vector3Int(hmg.x, hmg.y, 0);
        }
        else
        {
            GameDatas.HourMinuteGold = new Vector3Int(hmg.x, hmg.y, hmg.z + gold);
            gold = 0;
        }
        EventHandler.CallGoldChangeEvent();
        return true;
    }

    #region UIMovement
    public static IEnumerator MovingUIWithLocalPosition(RectTransform rect, Vector3 to, float timeToMove)
    {
        Vector3 from = rect.localPosition;
        float time = 0.0f;
        while (time < timeToMove)
        {
            time += Time.deltaTime;
            if (time > timeToMove) time = timeToMove;
            rect.localPosition = Vector3.Lerp(from, to, time / timeToMove);
            yield return null;
        }
        rect.localPosition = to;
    }

    public static IEnumerator Fading(CanvasGroup fadeUI, float to, float timeToFade, UnityAction done = null)
    {
        float from = fadeUI.alpha;
        float time = 0.0f;
        while (time < timeToFade)
        {
            time += Time.deltaTime;
            if (time > timeToFade) time = timeToFade;
            fadeUI.alpha = Mathf.Lerp(from, to, time / timeToFade);
            yield return null;
        }
        fadeUI.alpha = to;
        done?.Invoke();
    }

    public static IEnumerator Fading(SpriteRenderer sr, Color to, float timeToFade, UnityAction done = null)
    {
        Color from = sr.color;
        float time = 0.0f;
        while (time < timeToFade)
        {
            /*if (!sr.gameObject.activeSelf)
                break;*/
            time += Time.deltaTime;
            if (time > timeToFade) time = timeToFade;
            sr.color = Color.Lerp(from, to, time / timeToFade);
            yield return null;
        }
        sr.color = to;
        done?.Invoke();
    }
    #endregion

    /* public static bool Random(int prabability, int range) => UnityEngine.Random.Range(0, range) < prabability;*/


    public static IEnumerator DelayCall(float delay, UnityAction func)
    {
        while (delay > 0f) { delay -= Time.deltaTime; yield return null; }
        func?.Invoke();
    }

    //public static bool RandomByGrade(int gradeIdx) => Random.Range(0, 100) < GameDatas.gradeStackPercentage[gradeIdx];
    public static int GetRandomGrade()
    {
        int ran = Random.Range(1, 101);
        int _grade = (int)Grade.legendary;

        while (ran > 0)
        {
            if (ran <= GameDatas.gradePercentage[_grade])
                break;

            ran -= GameDatas.gradePercentage[_grade--];
        }
        return _grade;
    }


    public static int[] ShuffleArray(int length, int shuffle = 100)
    {
        int[] result = new int[length];
        for (int i = 0; i < length; i++) result[i] = i;

        int a = 0;
        int b = 0;
        for (int i = 0; i < shuffle; i++)
        {
            a = Random.Range(0, length);
            b = Random.Range(0, length);
            if (a == b) continue;

            result[a] = result[a] ^ result[b];
            result[b] = result[a] ^ result[b];
            result[a] = result[a] ^ result[b];
        }

        return result;
    }

}
