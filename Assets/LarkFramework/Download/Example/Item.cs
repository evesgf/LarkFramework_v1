using System.Collections;
using System.Collections.Generic;
using LarkFramework.Download;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Text txt_Name;
    public Text txt_Size;
    public Slider Slider;

    private long m_TotalSize;

    private LoadUpdateCallback m_LoadUpdateCallback;

    public void Init(long totalSize,string fileName, string url, string savePath, object userData = null,LoadSuccessCallback loadSuccessCallback = null, LoadFailureCallback loadFailureCallback = null, LoadUpdateCallback loadUpdateCallback = null)
    {
        txt_Name.text = fileName;
        m_TotalSize = totalSize;
        txt_Size.text = "0/" + totalSize;
        Slider.value = 0;

        loadUpdateCallback += ShowSlider;

        DownloadManager.Instance.AddDownload(fileName, url, Application.streamingAssetsPath, null, delegate { print(fileName + ":下完啦。。。。"); }, delegate { print("咋回事啊？"); }, loadUpdateCallback);
    }


    public void ShowSlider(float processValue, long fileLoadSize = 0,long fileTotalSize = 0)
    {
        UpdateItem(processValue, fileLoadSize,fileTotalSize);
    }

    public void UpdateItem(float value,long loadSize, long totalSize)
    {
        Slider.value = value;
        txt_Size.text = (loadSize / 1024+ "/"+ totalSize / 1024);
    }
}
