using System.Collections.Generic;
using System;

public static class EnumCaching
{
    static Dictionary<Enum, string> dic = new Dictionary<Enum, string>();
    public static string ToString(Enum key)
    {
        if (dic.TryGetValue(key, out string value)) return value;

        dic[key] = key.ToString();
        return dic[key];
    }
}