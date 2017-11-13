using LarkFramework.Download;
using LarkFramework.Module;
using LarkFramework.Tick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Download_Example : MonoBehaviour {

    public DownList[] downList;

    [System.Serializable]
    public class DownList
    {
        public string fileName;
        public string url;
    }

    public Slider slider;
    private LoadUpdateCallback m_LoadUpdateCallback;

    // Use this for initialization
    void Start () {
        Init();
    }

    public void Init()
    {
        Debuger.EnableLog = true;

        ModuleManager.Instance.Init("LarkFramework.Module.Example");

        TickManager.Instance.Init();
        DownloadManager.Instance.Init(2,1024*1024,30*1000);
    }

    public void Down()
    {
        m_LoadUpdateCallback += ShowSlider;

        foreach (var item in downList)
        {

            DownloadManager.Instance.AddDownload(item.fileName, item.url, Application.streamingAssetsPath, null, delegate { print(item.fileName+":下完啦。。。。"); }, delegate { print("咋回事啊？"); }, m_LoadUpdateCallback);
        }
    }

    public void ShowSlider(float processValue, long fileTotalSize = 0)
    {
        slider.value = processValue;
    }
}
