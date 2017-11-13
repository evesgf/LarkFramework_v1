using LarkFramework.Download;
using LarkFramework.Module;
using LarkFramework.Tick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Download_Example : MonoBehaviour {

    public string fileName;
    public string url;

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
        DownloadManager.Instance.Init();
    }

    public void Down()
    {
        m_LoadUpdateCallback += ShowSlider;
        DownloadManager.Instance.AddDownload(fileName, url, Application.streamingAssetsPath,null, delegate { print("文件下完啦。。。。"); },delegate { print("咋回事啊？"); }, m_LoadUpdateCallback);
    }

    public void ShowSlider(float processValue, int fileTotalSize = 0)
    {
        slider.value = processValue;
    }
}
