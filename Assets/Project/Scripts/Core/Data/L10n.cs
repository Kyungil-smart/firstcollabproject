using UnityEngine.Localization.Settings;

/// <summary>
/// "Localization UI Table"에서 키로 번역된 문자열을 가져오는 헬퍼
/// </summary>
public static class L10n
{
    const string Table = "Localization UI Table";

    public static string Get(string key)
    {
        var table = LocalizationSettings.StringDatabase.GetTable(Table);
        return table?.GetEntry(key)?.GetLocalizedString() ?? key;
    }
}
