using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class AssetBundleMeta
    {
        [SerializeField] private string name;
        [SerializeField] private string url;

        public string Name
        {
            get
            {
                string coolName = name;
#if UNITY_IOS
                coolName = coolName.Replace("android", "ios");
#endif
                return coolName;
            }
        }
        public string Url
        {
            get
            {
                string coolUrl = url;
#if UNITY_IOS
                coolUrl = coolUrl.Replace("android", "ios");
#endif
                return coolUrl;
            }
        }
    }
    [CreateAssetMenu(fileName = "DataURL_", menuName = "Data/DataURL")]
    public class DataUrl : ScriptableObject
    {
        [Header("Voices")]
        [SerializeField] private AssetBundleMeta voices;
        public AssetBundleMeta Voices => voices;
    }
}