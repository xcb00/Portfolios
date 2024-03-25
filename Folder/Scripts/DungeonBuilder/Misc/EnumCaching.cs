using System;
using System.Collections.Generic;

// ToString()�� ����� ��� �ڽ��� �Ͼ�� ������, Caching�� ���� ���� ����ϴ� enum���� ����
public static class EnumCaching
{
    static Dictionary<Enum, string> enumDic = new Dictionary<Enum, string>();
    public static string ToString(Enum key)
    {
        // ���� enumDic�� key���� �ִٸ� value�� ��ȯ
        if (enumDic.TryGetValue(key, out string value))
            return value;

        // ���� enumDic�� key���� ���ٸ� enumDic�� �߰��� �� ��ȯ
        enumDic[key] = key.ToString();
        return enumDic[key];
    }
}