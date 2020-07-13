using System;
using System.Collections;
using Data;
using MosMos.DownloadAssetBundle;
using UnityEngine;
using UnityEngine.Networking;


public enum StateDownload
{
    None,
    Success,
    Error
}
public class Downloader
{
    public static StateDownload State { get; private set; }
    public static event Action OnDownloaded;
    public static event Action<UnityWebRequest> OnError;
    private static Hash128 hash;

    private static UnityWebRequest uwr;
    private static UnityWebRequestAsyncOperation async;
    private static long contentLength = default;

    public static IEnumerator DownloadBundle(AssetBundleMeta meta, Action<AssetBundle> response = default)
    {
        State = StateDownload.None;

        while (!Caching.ready)
            yield return null;

        hash = Hash128.Parse(meta.Name);

        if (DownloadResourseManager.AllAssets.ContainsKey(meta))
        {
            State = StateDownload.Success;
            response?.Invoke(DownloadResourseManager.AllAssets[meta]);
            OnDownloaded?.Invoke();

            Debug.Log($"Load from Memory <color=green>{meta.Name}</color>.");
        }

        using (uwr = UnityWebRequest.Head(meta.Url))
        {
            async = uwr.SendWebRequest();
            while (!async.isDone && string.IsNullOrEmpty(uwr.error))
                yield return null;

            State = string.IsNullOrEmpty(uwr.error) ? StateDownload.Success : StateDownload.Error;

            if (State == StateDownload.Error)
            {
                using (uwr = UnityWebRequestAssetBundle.GetAssetBundle(meta.Url, hash))
                {
                    async = uwr.SendWebRequest();
                    while (!async.isDone && string.IsNullOrEmpty(uwr.error))
                        yield return null;

                    State = string.IsNullOrEmpty(uwr.error) ? StateDownload.Success : StateDownload.Error;

                    if (State == StateDownload.Error)
                    {
                        OnError?.Invoke(uwr);
                        Debug.Log($"{uwr.responseCode}\n" +
                                  $"<color=red>{uwr.error}</color>");
                        yield break;
                    }
                    else
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);

                        if (bundle != null)
                        {
                            if (DownloadResourseManager.AllAssets.ContainsKey(meta))
                                DownloadResourseManager.AllAssets.Remove(meta);
                            DownloadResourseManager.AllAssets.Add(meta, bundle);
                            response?.Invoke(bundle);
                            OnDownloaded?.Invoke();

                            Debug.Log($"Load from CACHE <color=green>{bundle.name}</color>.");
                        }
                    }
                }
            }
            else
            {
                contentLength = Convert.ToInt64(uwr.GetResponseHeader("Content-Length"));
                if (contentLength / Math.Pow(1024, 2) > SystemInfo.systemMemorySize)
                {
                    State = StateDownload.Error;
                    Debug.Log("<color=red>Not anough memory on device.</color>");
                    yield break;
                }
            }
        }
        using (uwr = UnityWebRequestAssetBundle.GetAssetBundle(meta.Url, hash))
        {
            async = uwr.SendWebRequest();
            while (!async.isDone && string.IsNullOrEmpty(uwr.error))
                yield return null;

            State = string.IsNullOrEmpty(uwr.error) ? StateDownload.Success : StateDownload.Error;

            if (State == StateDownload.Error)
            {
                OnError?.Invoke(uwr);
                Debug.Log($"{uwr.responseCode}\n" +
                          $"<color=red>{uwr.error}</color>");
                yield break;
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                if (bundle != null)
                {
                    if (DownloadResourseManager.AllAssets.ContainsKey(meta))
                        DownloadResourseManager.AllAssets.Remove(meta);
                    DownloadResourseManager.AllAssets.Add(meta, bundle);
                    response?.Invoke(bundle);
                    OnDownloaded?.Invoke();
                }
            }
        }
    }

    public static IEnumerator DownloadBundle(AssetBundleMeta meta, Canvas_Downloading canvas, Action<AssetBundle> response = default)
    {
        State = StateDownload.None;

        while (!Caching.ready)
            yield return null;

        hash = Hash128.Parse(meta.Name);

        if (DownloadResourseManager.AllAssets.ContainsKey(meta))
        {
            State = StateDownload.Success;
            response?.Invoke(DownloadResourseManager.AllAssets[meta]);
            OnDownloaded?.Invoke();

            Debug.Log($"Load from Memory <color=green>{meta.Name}</color>.");
        }

        using (uwr = UnityWebRequest.Head(meta.Url))
        {
            async = uwr.SendWebRequest();
            yield return Downloading(canvas);

            if (State == StateDownload.Error)
            {
                using (uwr = UnityWebRequestAssetBundle.GetAssetBundle(meta.Url, hash))
                {
                    async = uwr.SendWebRequest();
                    yield return Downloading(canvas);

                    if (State == StateDownload.Error)
                    {
                        ErrorHandling(canvas);
                        yield break;
                    }
                    else
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);

                        if (bundle != null)
                        {
                            if (DownloadResourseManager.AllAssets.ContainsKey(meta))
                                DownloadResourseManager.AllAssets.Remove(meta);
                            DownloadResourseManager.AllAssets.Add(meta, bundle);
                            response?.Invoke(bundle);
                            OnDownloaded?.Invoke();

                            Debug.Log($"Load from CACHE <color=green>{bundle.name}</color>.");
                            yield break;
                        }
                        else
                        {
                            State = StateDownload.Error;
                            ErrorHandling(canvas);
                            yield break;
                        }
                    }
                }
            }
            else
            {
                contentLength = Convert.ToInt64(uwr.GetResponseHeader("Content-Length"));
                if (contentLength / Math.Pow(1024, 2) > SystemInfo.systemMemorySize)
                {
                    State = StateDownload.Error;
                    Debug.Log("<color=red>Not anough memory on device.</color>");
                    canvas.ErrorDevice_on();
                    yield break;
                }
            }
        }
        using (uwr = UnityWebRequestAssetBundle.GetAssetBundle(meta.Url, hash))
        {
            async = uwr.SendWebRequest();
            yield return Downloading(canvas);

            if (State == StateDownload.Error)
            {
                ErrorHandling(canvas);
                yield break;
            }

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
            if (bundle != null)
            {
                if (DownloadResourseManager.AllAssets.ContainsKey(meta))
                    DownloadResourseManager.AllAssets.Remove(meta);
                DownloadResourseManager.AllAssets.Add(meta, bundle);
                response?.Invoke(bundle);
                OnDownloaded?.Invoke();
            }
        }
    }
    private static IEnumerator Downloading(Canvas_Downloading canvas)
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        canvas.gameObject.SetActive(true);
        canvas.PanelLoad_on();

        while (!uwr.isDone && String.IsNullOrEmpty(uwr.error))
        {
            float tempFill = 0.9f / (0.009f * 0.9f / uwr.downloadProgress);
            if (tempFill > 100f)
                tempFill = 100f;
            canvas.img_fillAmount.fillAmount = tempFill / 100f;
            canvas.txt_percent.text = (int)tempFill + " %";
            yield return null;
        }
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        State = string.IsNullOrEmpty(uwr.error) ? StateDownload.Success : StateDownload.Error;
    }
    private static void ErrorHandling(Canvas_Downloading canvas)
    {
        OnError?.Invoke(uwr);
        Debug.Log($"{uwr.responseCode}\n" +
                  $"<color=red>{uwr.error}</color>");

        if (uwr.isHttpError)
        {
            canvas.ErrorHTTP_on();
        }
        if (uwr.isNetworkError)
        {
            canvas.ErrorNetwork_on();
        }
    }
    public static void AbortDownloading()
    {
        uwr?.Abort();
    }
}
