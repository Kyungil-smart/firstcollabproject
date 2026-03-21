using UnityEngine;

public class BattleTeamA : MonoBehaviour
{
    [SerializeField] BattleInputReader input;

    [SerializeField] MonoBehaviour IWeapon;
    [SerializeField] MonoBehaviour IDamageable;
    public IWeapon weapon;
    public IDamageable target;

#if UNITY_EDITOR
    private void Reset()
    {
        // BattleInputReader SO 를 프로젝트 폴더에서 찾아서 할당
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BattleInputReader");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            input = UnityEditor.AssetDatabase.LoadAssetAtPath<BattleInputReader>(path);
        }
        else
        {
            Debug.LogWarning("BattleInputReader SO를 찾을 수 없습니다");
        }
    }
#endif
    private void Awake()
    {
        weapon = IWeapon as IWeapon;
        target = IDamageable as IDamageable;
    }

    private void Start()
    {
        input.Enable();
        input.on1 += () => Debug.Log("1");
        input.on2 += () => Debug.Log("2");
        input.on3 += () => Debug.Log("3");
        input.onAttack += Attack;
    }
    private void OnDisable()
    {
        //input.on1 = null;
        //input.on2 = null;
        //input.on3 = null;
        input.onAttack -= Attack;
    }

    private void Attack()
    {
        if (target != null)
        {
            weapon.Use(target);
        }
    }
}
