using System.Collections.Generic;
using UnityEngine;

public interface ICritPolicy
{
    float Chance(float luck); // 확률 반환
    bool Roll(float luck);    // 실제 결과 반환
}

public class CritPolicy : ICritPolicy
{
    public float rate;
    static readonly Dictionary<float, CritPolicy> cache = new();

    private CritPolicy(float rate) => this.rate = rate;

    public float Chance(float luck) => Mathf.Clamp01(luck);
    public bool Roll(float luck) => UnityEngine.Random.value < Chance(luck);

    public static CritPolicy Get(float rate = 0.05f)
    {
        if (!cache.TryGetValue(rate, out var policy))
        {
            policy = new CritPolicy(rate);
            cache[rate] = policy;
        }
        return policy;
    }
}
