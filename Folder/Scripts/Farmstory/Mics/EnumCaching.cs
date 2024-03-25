using System;
using System.Collections.Generic;

public static class EnumCaching
{
    static Dictionary<Enum, string> enum2str = new Dictionary<Enum, string>();
    public static string ToString(Enum key)
    {
        if (enum2str.TryGetValue(key, out string value))
            return value;

        string stringValue = key.ToString();
        enum2str[key] = stringValue;
        return stringValue;
    }
}
