using UnityEngine;
using System;
using System.Globalization;

public abstract class SheetDataSOBase : ScriptableObject
{
    public int id; // 파싱 기준값
    public int row; // DataRequestSet이 주입

    public abstract void SetData(string[] data);

    protected int ParseInt(string raw)
    {
        if (int.TryParse(raw, out int result)) return result;
        LogWarn(raw, "int");
        return default;
    }
    protected float ParseFloat(string raw)
    {
        if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float result)) return result;
        LogWarn(raw, "float");
        return default;
    }
    protected bool ParseBool(string raw)
    {
        if (bool.TryParse(raw, out bool result)) return result;
        LogWarn(raw, "bool");
        return default;
    }
    protected T ParseEnum<T>(string raw) where T : struct, Enum
    {
        if (Enum.TryParse(raw, ignoreCase: true, out T result)) return result;
        LogWarn(raw, typeof(T).Name);
        return default;
    }


    private void LogWarn(string raw, string expectedType)
    {
        Debug.LogWarning(
            $"[파싱실패]  <b>{row}행</b> \n 입력값: \"{raw}\" | 기대타입: {expectedType}"
        );
    }
}