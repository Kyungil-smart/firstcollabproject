using UnityEngine;

public class BattleTeamB : MonoBehaviour, IDamageable
{
    public int hp = 100;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log(hp);
    }

}
