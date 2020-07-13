using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MosMos.DownloadAssetBundle
{
    public class Canvas_Downloading : MonoBehaviour
    {
        public TextMeshProUGUI txt_percent;
        public Image img_fillAmount;
        [SerializeField] private GameObject panel_Load;
        [SerializeField] private GameObject panel_AsyncLoad;
        [Header("for errors")]
        [SerializeField] private GameObject errorNetwork;
        [SerializeField] private GameObject errorDevice;
        [SerializeField] private GameObject errorHTTP;

        public void PanelLoad_on()
        {
            panel_Load.SetActive(true);
            errorHTTP.SetActive(false);
            errorDevice.SetActive(false);
            errorNetwork.SetActive(false);
        }
        public void PanelAsyncLoad_on()
        {
            panel_Load.SetActive(false);
            errorHTTP.SetActive(false);
            errorDevice.SetActive(false);
            errorNetwork.SetActive(false);
        }
        public void ErrorHTTP_on()
        {
            panel_Load.SetActive(false);
            errorHTTP.SetActive(true);
            errorDevice.SetActive(false);
            errorNetwork.SetActive(false);
        }
        public void ErrorDevice_on()
        {
            panel_Load.SetActive(false);
            errorHTTP.SetActive(false);
            errorDevice.SetActive(true);
            errorNetwork.SetActive(false);
        }
        public void ErrorNetwork_on()
        {
            panel_Load.SetActive(false);
            errorHTTP.SetActive(false);
            errorDevice.SetActive(false);
            errorNetwork.SetActive(true);
        }

        public void DisableLoadingWindow()
        {
            panel_Load.SetActive(false);
        }
    }
}
