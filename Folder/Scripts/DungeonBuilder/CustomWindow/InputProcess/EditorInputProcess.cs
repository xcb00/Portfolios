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

    public void KeyInput(EditorKeyboardInput input)
    {
        if (input == EditorKeyboardInput.None) return;
        else if (!KeyboardInputDic.ContainsKey(input))
        {
            Debug.LogError($"KeyNotFoundException : '{input}' is not registered in the 'KeyboardInputDic'.");
            return;
        }
        else
            KeyboardInputDic[input]?.Invoke();
    }

    public void MouseInput(Event _event, EditorMouseInput input, int index)
    {
        if (input == EditorMouseInput.None) return;
        else if (!MouseInputDic.ContainsKey(input))
        {
            Debug.LogError($"KeyNotFoundException : '{input}' is not registered in the 'MouseInputDic'.");
            return;
        }
        else if (MouseInputDic[input].Count <= index)
        {
            Debug.LogError($"IndexOutOfRangeException : The entered index({index}) is outside the range of 'MouseInputDic[{input}]'.");
            return;
        }
        else
            MouseInputDic[input][index]?.Invoke(_event);
    }
}

public class EditorProcessBuilder
{
    EditorInputProcess process;

    public EditorProcessBuilder()
    {
        process = new EditorInputProcess();
    }

    public EditorProcessBuilder MouseEvent(Action<Event> method, EditorMouseInput input, int index)
    {
        if (!process.MouseInputDic.ContainsKey(input))
            process.MouseInputDic[input] = new List<Action<Event>>();

        if (process.MouseInputDic[input].Count < index)
            Debug.LogError($"IndexOutOfRangeException : The entered index({index}) is outside the range of 'MouseInputDic[{input}]'.");
        else if (process.MouseInputDic[input].Count == index)
            process.MouseInputDic[input].Add(method);
        else
            process.MouseInputDic[input][index] += method;

        return this;
    }

    public EditorProcessBuilder KeyboardEvent(Action method, EditorKeyboardInput input)
    {
        if (process.KeyboardInputDic.ContainsKey(input))
            Debug.LogError($"ArgumentException : {input} has already been added in 'KeyboardInputDic'.");
        else
            process.KeyboardInputDic[input] = method;

        return this;
    }

    public EditorInputProcess Build() => process;
}
#endif