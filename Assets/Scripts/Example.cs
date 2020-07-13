using Data;
using MosMos.DownloadAssetBundle;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Example : MonoBehaviour
    {
        [SerializeField] private DataUrl url;

        private void Start()
        {
            DownloadResourseManager.Url = url;
            StartCoroutine(TryToDownload());
        }

        private IEnumerator TryToDownload()
        {
            yield return Downloader.DownloadBundle(DownloadResourseManager.Url.Voices, OnDownload);
        }
        private void OnDownload(AssetBundle bundle)
        {
            print($"Bundle <color=green>{bundle.name}</color> is downloaded.");
        }
    }
}