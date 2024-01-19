#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

public class EditorInputProcess
{
    public Dictionary<EditorMouseInput, List<Action<Event>>> MouseInputDic;
    public Dictionary<EditorKeyboardInput, Action> KeyboardInputDic;
    public int MouseEventCount(EditorMouseInput input) => MouseInputDic[input].Count;

    public EditorInputProcess()
    {
        MouseInputDic = new Dictionary<EditorMouseInput, List<Action<Event>>>();
        KeyboardInputDic = new Dictionary<EditorKeyboardInput, Action>();
    }

    /*public void InputProcess(Event _event, int index = -1)
    {
        if (_event.type == EventType.KeyDown)
            KeyInput(_event);
        else
            MouseInput(_event, index);
    }*/

    public void KeyInput(EditorKeyboardInput input)
    {
        try
        {
            if (input == EditorKeyboardInput.None)
                return;
            else if (!KeyboardInputDic.ContainsKey(input))
                throw new Exception(ErrorCode.KeyNotFoundException.ToString());
            else
                KeyboardInputDic[input]?.Invoke();
        }
        catch(Exception e)
        {
            EditorUtilities.PrintErrorMessage(e.Message, $"KeyboardInputDic에 {input}이 등록되어있지 않습니다", 3);
        }
    }

    public void MouseInput(Event _event, EditorMouseInput input, int index)
    {
        try
        {
            if (input == EditorMouseInput.None)
                return;
            else if(!MouseInputDic.ContainsKey(input))
                throw new Exception(ErrorCode.KeyNotFoundException.ToString());
            else if (MouseInputDic[input].Count <= index)
                throw new Exception(ErrorCode.IndexOutOfRangeException.ToString());
            else
                MouseInputDic[input][index]?.Invoke(_event);
        }
        catch(Exception e)
        {
            if (e.Message.Equals(ErrorCode.KeyNotFoundException.ToString()))
                EditorUtilities.PrintErrorMessage(e.Message, $"MouseInputDic에 {input}이 등록되어있지 않습니다", 3);
            else if (e.Message.Equals(ErrorCode.IndexOutOfRangeException.ToString()))
                EditorUtilities.PrintErrorMessage(e.Message, $"입력한 인덱스[{index}]가 {input}의 크기[{MouseInputDic[input].Count}]보다 크거나 같습니다", 3);
            else
                Debug.LogError($"Undefined Error Occurred : {e.Message}");
        }
    }
}

public class EditorProcessBuilder
{
    EditorInputProcess process;
    public EditorProcessBuilder()
    {
        process = new EditorInputProcess();
    }

    public EditorProcessBuilder MouseEvent(Action<Event> func, EditorMouseInput input, int index)
    {
        try
        {
            if (!process.MouseInputDic.ContainsKey(input))
                process.MouseInputDic[input] = new List<Action<Event>>();

            if (process.MouseInputDic[input].Count < index)
                throw new Exception(ErrorCode.IndexOutOfRangeException.ToString());
            else if (process.MouseInputDic[input].Count == index)
                process.MouseInputDic[input].Add(func);
            else
                process.MouseInputDic[input][index] += func;
        }
        catch(Exception e)
        {
            EditorUtilities.PrintErrorMessage(e.Message, $"입력한 인덱스[{index}]가 {input} 리스트의 크기[{process.MouseEventCount(input)}]보다 같거나 작아야합니다", 2);
        }
        return this;
    }

    public EditorProcessBuilder KeyboardEvent(Action func, EditorKeyboardInput input)
    {
        try
        {
            if (process.KeyboardInputDic.ContainsKey(input))
                throw new Exception(ErrorCode.IndexOutOfRangeException.ToString());
            else
                process.KeyboardInputDic[input] = func;
        }
        catch (Exception e)
        {
            EditorUtilities.PrintErrorMessage(e.Message, $"{input}에 이미 등록된 함수가 있습니다", 2);
        }
        return this;
    }   

    public EditorInputProcess Build()
    {
        return process;
    }
}
#endif
