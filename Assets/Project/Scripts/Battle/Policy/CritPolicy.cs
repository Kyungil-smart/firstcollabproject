using UnityEngine;

public struct CritPolicy
{
    public float rate;
    public CritPolicy(float rate) => this.rate = rate;
    public static CritPolicy Get(float rate) => new CritPolicy(rate);
    public bool Roll() => Random.value <= Mathf.Clamp01(rate);
}
