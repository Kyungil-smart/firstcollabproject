using System.Collections.Generic;
using UnityEngine;

public interface ICritPolicy
{
    float Chance(float luck); // 확률 반환
    bool Roll();    // 실제 결과 반환
}

public struct CritPolicy : ICritPolicy
{
    public float rate;
    public CritPolicy(float rate) => this.rate = rate;
    public static CritPolicy Get(float rate) => new CritPolicy(rate);
    public bool Roll()
    {
        Debug.Log(rate);
        return UnityEngine.Random.value <= Chance(rate);
    }
    public float Chance(float luck) => Mathf.Clamp01(luck);

}
