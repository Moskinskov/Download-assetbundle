using Data;
using System.Collections.Generic;
using UnityEngine;

namespace MosMos.DownloadAssetBundle
{
    public class DownloadResourseManager
    {
        private static Dictionary<AssetBundleMeta, AssetBundle> allAssets;
        public static Dictionary<AssetBundleMeta, AssetBundle> AllAssets
        {
            get => allAssets ?? (allAssets = new Dictionary<AssetBundleMeta, AssetBundle>());
            set
            {
                if (allAssets == null)
                    allAssets = new Dictionary<AssetBundleMeta, AssetBundle>();
                allAssets = value;
            }
        }

        private static DataUrl dataUrl;
        public static DataUrl Url
        {
            get => dataUrl;
            set => dataUrl = value;
        }
    }
}