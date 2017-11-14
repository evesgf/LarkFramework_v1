using LarkFramework.Module;
using LarkFramework.Tick;
using System;
using System.Collections;
using System.Collections.Generic;


namespace LarkFramework.Download
{
    /// <summary>
    /// 下载管理器
    /// 参考：http://blog.csdn.net/dingxiaowei2013/article/details/77814966
    /// 参考：http://blog.csdn.net/damenhanter/article/details/50273303
    /// </summary>
    public class DownloadManager : ServiceModule<DownloadManager>
    {
        /// <summary>
        /// 最大限制
        /// </summary>
        public int MAX_LOAD_REQUEST = 4;

        private List<DownloadTask> m_DownList;                      //下载队列
        private Queue<DownloadTask> m_WaitQue;                      //等待下载队列
        private Queue<DownloadTask> completeQue;                    //下载完成队列

        private int m_FlushSize= 1024 * 1024;                       //缓冲区大小
        private int m_Timeout= 30 * 1000;                           //超时时间

        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <param name="maxLoad">最大同时下载数量</param>
        /// <param name="flushSize">缓冲区大小</param>
        /// <param name="timeOut">超时时间（毫秒）</param>
        public void Init(int maxLoad=4, int flushSize = 1024 * 1024, int timeOut= 30*1000)
        {
            CheckSingleton();

            m_DownList = new List<DownloadTask>();
            m_WaitQue = new Queue<DownloadTask>();
            completeQue = new Queue<DownloadTask>();

            MAX_LOAD_REQUEST = maxLoad;
            m_FlushSize = flushSize;
            m_Timeout = timeOut;

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
                if (m_DownList.Count < MAX_LOAD_REQUEST)
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

            if (m_DownList.Count > 0)
            {
                //输出下载队列状态
                for (int i = 0; i < m_DownList.Count; i++)
                {
                    if (m_DownList[i].m_LoadUpdateCallback != null)
                    {
                        m_DownList[i].m_LoadUpdateCallback.Invoke(m_DownList[i].progress, m_DownList[i].fileLength, m_DownList[i].totalLength);
                    }

                    if (m_DownList[i].isDone)
                    {
                        //移除下载队列
                        RemoveDownload(m_DownList[i]);
                    }
                }
            }

            Debuger.Log("m_WaitQue:"+m_WaitQue.Count+ " m_DownList:" + m_DownList.Count);
        }

        /// <summary>
        /// 根据优先级从等待队列移动一个任务到下载队列
        /// </summary>
        public void MoveTaskFromWaitDicToDwonDict()
        {
            var d = m_WaitQue.Dequeue();
            m_DownList.Add(d);
            d.Download();
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

        public void ReStartDownList()
        {
            foreach (var item in m_DownList)
            {
                ReStartDownload(item);
            }
        }

        /// <summary>
        /// 重新开始下载任务
        /// </summary>
        /// <param name="task"></param>
        public void ReStartDownload(DownloadTask task)
        {
            task.Download();
        }

        /// <summary>
        /// 从下载队列中移除下载任务
        /// </summary>
        /// <param name="task"></param>
        public void RemoveDownload(DownloadTask task)
        {
            task.Close();
            m_DownList.Remove(task);
        }
    }
}
