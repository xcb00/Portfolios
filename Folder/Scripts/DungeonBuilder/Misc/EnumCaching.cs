using System;
using System.Collections.Generic;

// ToString()을 사용할 경우 박싱이 일어나기 때문에, Caching을 통해 자주 사용하는 enum값을 저장
public static class EnumCaching
{
    static Dictionary<Enum, string> enumDic = new Dictionary<Enum, string>();
    public static string ToString(Enum key)
    {
        // 만약 enumDic에 key값이 있다면 value를 반환
        if (enumDic.TryGetValue(key, out string value))
            return value;

        // 만약 enumDic에 key값이 없다면 enumDic에 추가한 후 반환
        enumDic[key] = key.ToString();
        return enumDic[key];
    }
}