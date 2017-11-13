using LarkFramework.Module;
using LarkFramework.Tick;
using System;
using System.Collections;
using System.Collections.Generic;


namespace LarkFramework.Download
{
    /// <summary>
    /// 下载管理器
    /// </summary>
    public class DownloadManager : ServiceModule<DownloadManager>
    {
        /// <summary>
        /// 最大限制
        /// </summary>
        public int MAX_LOAD_REQUEST = 4;

        private Queue<DownloadTask> m_DownQue;                      //下载队列
        private Queue<DownloadTask> m_WaitQue;                      //等待下载队列
        private Queue<DownloadTask> completeQue;                    //下载完成队列

        private int m_FlushSize;                                    //缓冲区大小
        private float m_Timeout;                                    //超时时间

        /// <summary>
        /// 初始化操作
        /// </summary>
        public void Init()
        {
            CheckSingleton();

            m_DownQue = new Queue<DownloadTask>();
            m_WaitQue = new Queue<DownloadTask>();
            completeQue = new Queue<DownloadTask>();

            m_FlushSize = 1024 * 1024;
            m_Timeout = 30f;

            TickComponent.Instance.onUpdate += Update;
        }

        /// <summary>
        /// 有限状态机管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_WaitQue.Count > 0)
            {
                if (m_DownQue.Count < MAX_LOAD_REQUEST)
                {
                    //移入下载队列
                    MoveTaskFromWaitDicToDwonDict();
                }
                //else
                //{
                //    //等待下载队列
                //    Debuger.Log("等待空余下载队列");
                //}
            }
            //else
            //{
            //    //当前没有下载任务
            //    Debuger.Log("当前没有等待下载的任务");
            //}

            if (m_DownQue.Count > 0)
            {
                //输出下载队列状态
                foreach (var item in m_DownQue)
                {
                    item.m_LoadUpdateCallback.Invoke(item.progress);
                }
            }
        }

        /// <summary>
        /// 根据优先级从等待队列移动一个任务到下载队列
        /// </summary>
        public void MoveTaskFromWaitDicToDwonDict()
        {
            var d = m_WaitQue.Dequeue();
            m_DownQue.Enqueue(d);
            d.Download();

            Debuger.Log("新增下载项:"+d.m_FileName);
        }

        /// <summary>
        /// 增加下载任务
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <param name="userData"></param>
        /// <param name="loadSuccessCallback">成功回调</param>
        /// <param name="loadFailureCallback">失败回调</param>
        /// <param name="loadUpdateCallback">下载中回调</param>
        public void AddDownload(string fileName, string url, string savePath, object userData = null, LoadSuccessCallback loadSuccessCallback = null, LoadFailureCallback loadFailureCallback = null, LoadUpdateCallback loadUpdateCallback = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception("Download fileName is invalid.");
            }

            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Download url is invalid.");
            }

            if (string.IsNullOrEmpty(savePath))
            {
                throw new Exception("Download savePath is invalid.");
            }

            DownloadTask downloadTask = new DownloadTask(fileName,url,savePath, m_FlushSize, m_Timeout, userData, loadSuccessCallback, loadFailureCallback, loadUpdateCallback);

            m_WaitQue.Enqueue(downloadTask);
        }
    }
}
