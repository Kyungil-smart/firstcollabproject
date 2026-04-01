using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// LoadScene(빌드인덱스)로 씬을 비동기 로드하며 로딩시간을 표시해 줍니다
/// </summary>
public static class SceneLoader
{
    public static float Progress => _operation == null ? 0f : _operation.progress;

    static bool _isLoading;
    static AsyncOperation _operation;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // static은 도메인 리로드 안하면 초기화 안되서 명시적 초기화
    static void Initialize()
    {
        _isLoading = false;
        _operation = null;
    }

    public static async Awaitable LoadScene(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (_isLoading)
        {
            UnityEngine.Debug.LogWarning($"[씬로더] 이미 씬 로딩 중입니다. 인덱스: {buildIndex}"); return;
        }

        //await Awaitable.MainThreadAsync();
        _isLoading = true;

        var sw = Stopwatch.StartNew();

        try
        {
            _operation = SceneManager.LoadSceneAsync(buildIndex, mode);
            if (_operation == null)
            {
                UnityEngine.Debug.LogError($"[씬로더] 씬 로딩실패! 인덱스: {buildIndex}"); return;
            }

            await Awaitable.FromAsyncOperation(_operation);
            sw.Stop();
            UnityEngine.Debug.Log($"[씬로더] 씬 로딩 시간: {sw.Elapsed.TotalSeconds:F1}초 (인덱스: {buildIndex})");
        }
        catch
        {
            sw.Stop();
            throw;
        }
        finally
        {
            _operation = null;
            _isLoading = false;
        }
    }
}
