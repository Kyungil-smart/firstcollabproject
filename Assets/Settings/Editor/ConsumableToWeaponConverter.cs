using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CreateAssetMenu(fileName = "ConsumableToWeaponConverter", menuName = "EditorTools/ConsumableToWeaponConverter")]
public class ConsumableToWeaponConverter : ScriptableObject
{
    [Header("컨버팅할 데이터들")]
    [Tooltip("Google SpreadSheet에서 받아온 ConsumableSheetData (원본)")]
    public ConsumableSheetData[] consumableDatas;

    [Tooltip("실제 게임에서 사용할 WeaponSO (대상)")]
    public WeaponSO[] targetWeaponDatas;
}

[CustomEditor(typeof(ConsumableToWeaponConverter))]
public class ConsumableToWeaponConverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ConsumableToWeaponConverter converter = (ConsumableToWeaponConverter)target;

        GUILayout.Space(20);

        if (GUILayout.Button("Convert ConsumableData to WeaponSO", GUILayout.Height(40)))
        {
            ConvertData(converter);
        }
    }

    private void ConvertData(ConsumableToWeaponConverter converter)
    {
        if (converter.consumableDatas == null || converter.targetWeaponDatas == null)
        {
            Debug.LogError("Consumable Datas 또는 Target Weapon Datas 배열이 할당되지 않았습니다.");
            return;
        }

        if (converter.consumableDatas.Length != converter.targetWeaponDatas.Length)
        {
            Debug.LogWarning($"배열 길이가 다릅니다! 원본: {converter.consumableDatas.Length}개, 대상: {converter.targetWeaponDatas.Length}개. 가능한 만큼만 변환합니다.");
        }

        int count = Mathf.Min(converter.consumableDatas.Length, converter.targetWeaponDatas.Length);
        int convertCount = 0;

        for (int i = 0; i < count; i++)
        {
            ConsumableSheetData source = converter.consumableDatas[i];
            WeaponSO targetSO = converter.targetWeaponDatas[i];

            if (source == null || targetSO == null) continue;

            Undo.RecordObject(targetSO, "Convert Consumable to WeaponSO");

            //==================== 데이터 이동 ====================//
            targetSO.id = source.id;
            targetSO.Name = source.Name;
            targetSO.attackType = AttackType.Consume;
            
            targetSO.damageBase = source.damage;
            targetSO.rangeValue = source.range;
            
            targetSO.maxAmmo = source.maxStack;

            targetSO.splashEnable = true;
            targetSO.splashRadius = source.splashRadius;

            targetSO.stunEnable = (source.effectID == ControlType.Hard_CC);
            targetSO.stunTime = 1f;

            EditorUtility.SetDirty(targetSO);
            convertCount++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"<color=green>Convert Complete!</color> {convertCount}개의 데이터를 WeaponSO로 이동했습니다");
    }
}
#endif
