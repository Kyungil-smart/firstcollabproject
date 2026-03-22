using UnityEngine;

public class BattleTeamB : MonoBehaviour, IDamageable
{
    public float hp = 100;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log(hp);
    }

}
