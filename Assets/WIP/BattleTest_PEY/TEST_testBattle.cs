using UnityEngine;
using TMPro;
using Monster;

public class TEST_testBattle : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _enemyCount;

    private void Update()
    {
        _enemyCount.text = "Enemy: " + Registry<MonsterAction>.Count;
    }
}
