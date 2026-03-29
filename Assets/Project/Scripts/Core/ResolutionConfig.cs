using UnityEngine;

/// <summary>
/// 빌드 시 해상도 및 화면 모드를 강제 적용합니다
/// 씬 로드 전에 자동 실행
/// </summary>
public static class ResolutionConfig
{
    const int TargetWidth  = 1080;
    const int TargetHeight = 1920;
    const FullScreenMode TargetMode = FullScreenMode.FullScreenWindow;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Apply()
    {
        Screen.SetResolution(TargetWidth, TargetHeight, TargetMode);
    }
}
