using System.Collections.Generic;
using System;

#region Enum Caching
public static class Enums
{
    static Dictionary<Enum, string> enumDic = new Dictionary<Enum, string>();

    public static string ToString(Enum key)
    {
        if (enumDic.TryGetValue(key, out string value))
            return value;
        else
        {
            enumDic[key] = key.ToString();
            return enumDic[key];
        }
    }
}
#endregion

#region Enums
    #region Editor Enums
    #if UNITY_EDITOR
        #region Editor Keyboard Input Enum
        public enum EditorKeyboardInput
        {
            None,
            Ctrl_S,
            Ctrl_A,
            Ctrl_V,
            Ctrl_D,
            Ctrl_E,
            Shift_E,
            ESC
        }

        public enum EditorMouseInput
        {
            None,
            LeftDown, LeftUp, LeftDrag,
            RightDown, RightUp, RightDrag,
        }

        public enum MouseDownEvent
        {
            NoneOver,
            NodeOver
        }
#endregion


        #region Debbug Enum
        public enum ErrorCode
        {
            NullReferenceException,         // 오브젝트나 클래스의 인스턴스가 아직 생성되지 않았는데 참조할 경우
            IndexOutOfRangeException,       // 배열이나 리스트 등의 컬렉션에서 범위를 초과하는 인덱스에 접근할 경우
            KeyNotFoundException
            //ArgumentException,              // 잘못된 인수를 메소드에 전달할 경우
            //InvalidOperationException,      // 메소드 호출이 객체의 현재 상태에 대해 유효하지 않을 경우(열거형이 수정된 상태에서 열거형을 계속 사용할 경우 >> foreach문으로 list를 반복하는 중에 list를 삭제/추가할 경우)
            //MissingReferenceException,      // Destory된 오브젝트를 참조할 경우
            //UnassingedReferenceException    // 인스펙터에서 참조를 할당하지 않은 상태에서 참조할 경우(public을 사용하지 않은 변수를 참조할 경우)
        }
        #endregion


        #region NodeTexture Enum
        public enum NodeNumber
        {
            node1, node2, node3, node4, node5, node6
        }

        public enum NodeStyle
        {
            none, on
        }
        #endregion
    #endif
    #endregion

    #region Play Enums
    public enum RoomType
    {
        None = 0,
        SmallRoom, MediumRoom, LargeRoom, ChestRoom,
        Count,
        Entrance, BossRoom, // 노드에서 선택하지 못하게 타입을 Count보다 크게 설정 >> 
    }
    #endregion
#endregion
