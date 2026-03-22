using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSOBase : ScriptableObject
{
    static readonly List<RuntimeSOBase> _instances = new();

    void OnEnable() => _instances.Add(this);
    void OnDisable() => _instances.Remove(this);

    public abstract void OnReset();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetAll()
    {
        foreach (var instance in _instances)
        {
            instance.OnReset();
        }
    }
}
