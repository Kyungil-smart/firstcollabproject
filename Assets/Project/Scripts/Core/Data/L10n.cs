using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

/// <summary>
/// "Localization UI Table"에서 키로 번역된 문자열을 가져오는 헬퍼
/// </summary>
public static class L10n
{
    const string TableName = "Localization UI Table";

    public static string Get(string key)
    {
        var table = LocalizationSettings.StringDatabase.GetTable(TableName);
        if (table == null) return key;
        var entry = table.GetEntry(key);
        if (entry == null) return key;
        return entry.GetLocalizedString();
    }
}
