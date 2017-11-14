using LarkFramework.Download;
using LarkFramework.Module;
using LarkFramework.Tick;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public GameObject itemObj;
    public Transform itemRoot;

    // Use this for initialization
    void Start () {
        Init();
    }

    public void Init()
    {
        Debuger.EnableLog = true;

        ModuleManager.Instance.Init("LarkFramework.Module.Example");

        TickManager.Instance.Init();
        DownloadManager.Instance.Init(2,1024*1024,1*1000);
    }

    public void Down()
    {
        foreach (var down in downList)
        {
            var item=GameObject.Instantiate(itemObj, itemRoot).GetComponent<Item>();
            item.name = down.fileName;
            item.Init(0,down.fileName, down.url, Application.streamingAssetsPath);
        }
    }

    public void ReDown()
    {
        DownloadManager.Instance.ReStartDownList();
    }

    public void Clean()
    {
        foreach (var down in downList)
        {
            if (Directory.Exists(Application.streamingAssetsPath + "/" + down.fileName))
            {
                Directory.Delete(Application.streamingAssetsPath + "/" + down.fileName);
            }
        }
    }
}
