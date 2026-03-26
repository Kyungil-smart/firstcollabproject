using UnityEngine;

public struct CritPolicy
{
    public float rate;
    public CritPolicy(float rate) => this.rate = rate;
    public static CritPolicy Get(float rate) => new CritPolicy(rate);
    public bool Roll()
    {
        return UnityEngine.Random.value <= Chance(rate);
    }
    public float Chance(float luck) => Mathf.Clamp01(luck);

}
