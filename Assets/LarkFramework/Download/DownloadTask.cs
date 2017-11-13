using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace LarkFramework.Download
{
    /// <summary>
    /// 单个下载任务
    /// </summary>
    public class DownloadTask
    {

        public string m_FileName { get; set; }                    //文件名
        public string m_Url { get; set; }                         //下载地址

        public LoadSuccessCallback m_LoadSuccessCallback;
        public LoadFailureCallback m_LoadFailureCallback;
        public LoadUpdateCallback m_LoadUpdateCallback;

        private string m_SavePath;                                //保存地址
        private int m_FlushSize;                                  //缓冲区大小
        private int m_TimeOut;                                    //超时时间
        private object m_UserData;                                //用户自定义数据
        private ThreadPriority m_ThreadPriority;                  //线程优先级

        const int ReadWriteTimeOut = 2 * 1000;              //读写流时的超时（毫秒）。

        /// <summary>
        /// 下载进度
        /// </summary>
        public float progress { get; private set; }

        /// <summary>
        /// 表示下载是否完成
        /// </summary>
        public bool isDone { get; private set; }

        //已经下载的文件大小
        public long fileLength { get; set; }
        //文件总长度
        public long totalLength { get; set; }

        //涉及子线程要注意,Unity关闭的时候子线程不会关闭，所以要有一个标识
        private bool isStop;
        //子线程负责下载，否则会阻塞主线程，Unity界面会卡住
        private Thread thread;

        /// <summary>
        /// 初始化下载任务的的新实例
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="url">下载地址</param>
        /// <param name="savePath">保存地址</param>
        /// <param name="flushSize">缓冲区大小</param>
        /// <param name="timeOut">超时时间(毫秒)</param>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="loadSuccessCallback">成功回调</param>
        /// <param name="loadFailureCallback">失败回调</param>
        /// <param name="loadUpdateCallback">下载中回调</param>
        /// <param name="threadPriority">线程优先级</param>
        public DownloadTask(string fileName, string url, string savePath, int flushSize, int timeOut, object userData=null, LoadSuccessCallback loadSuccessCallback =null, LoadFailureCallback loadFailureCallback=null, LoadUpdateCallback loadUpdateCallback=null, ThreadPriority threadPriority = ThreadPriority.Normal)
        {
            m_FileName = fileName;
            m_Url = url;
            m_SavePath = savePath;
            m_FlushSize = flushSize;
            m_TimeOut = timeOut;
            m_UserData = userData;
            m_ThreadPriority = threadPriority;

            m_LoadSuccessCallback = loadSuccessCallback;
            m_LoadFailureCallback = loadFailureCallback;
            m_LoadUpdateCallback = loadUpdateCallback;
        }

        public void Download()
        {
            Debuger.Log("开始下载:"+m_FileName);
            isStop = false;
            Stopwatch stopWatch = new Stopwatch();
            //开启子线程下载,使用匿名方法

            //开启子线程下载,使用匿名方法
            thread = new Thread(delegate () {
                stopWatch.Start();

                //判断保存路径是否存在
                if (!Directory.Exists(m_SavePath))
                {
                    Directory.CreateDirectory(m_SavePath);
                }
                //else
                //{
                //    Directory.Delete(m_SavePath);
                //    Directory.CreateDirectory(m_SavePath);
                //}

                //这是要下载的文件名，比如从服务器下载a.zip到D盘，保存的文件名是test
                string filePath = m_SavePath + "/" + m_FileName;

                //使用流操作文件
                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                //获取文件现在的长度
                fileLength = fs.Length;
                //获取下载文件的总长度
                totalLength = GetLength(m_Url);

                Debuger.Log("<color=red>文件:" + m_FileName + " 已下载{" + fileLength / 1024 / 1024 + "}M，剩余{" + ((totalLength - fileLength) / 1024 / 1024) + "}M</color>");

                //如果没下载完
                if (fileLength < totalLength)
                {
                    //断点续传核心，设置本地文件流的起始位置
                    fs.Seek(fileLength, SeekOrigin.Begin);

                    HttpWebRequest request = WebRequest.Create(m_Url) as HttpWebRequest;

                    if (request != null)
                    {
                        request.ReadWriteTimeout = ReadWriteTimeOut;
                        request.Timeout = m_TimeOut;

                        //断点续传核心，设置远程访问文件流的起始位置
                        request.AddRange((int)fileLength);

                        Stream stream = request.GetResponse().GetResponseStream();
                        byte[] buffer = new byte[1024];
                        //使用流读取内容到buffer中
                        //注意方法返回值代表读取的实际长度,并不是buffer有多大，stream就会读进去多少
                        if (stream != null)
                        {
                            int length = stream.Read(buffer, 0, buffer.Length);
                            //Debuger.Log("<color=red>length:{"+ length + "}</color>");
                            while (length > 0)
                            {

                                //如果Unity客户端关闭，停止下载
                                if (isStop)
                                {
                                    m_LoadFailureCallback.Invoke();
                                    break;
                                }

                                //将内容再写入本地文件中
                                fs.Write(buffer, 0, length);
                                //计算进度
                                fileLength += length;
                                progress = (float)fileLength / (float)totalLength;
                                //UnityEngine.Debug.Log(progress);
                                //类似尾递归
                                length = stream.Read(buffer, 0, buffer.Length);

                            }
                        }
                        stream.Close();
                        stream.Dispose();
                    }
                }
                else
                {
                    progress = 1;
                }
                stopWatch.Stop();
                Debuger.Log("耗时: " + stopWatch.ElapsedMilliseconds);
                fs.Close();
                fs.Dispose();

                //如果下载完毕，执行回调
                if (progress == 1)
                {
                    isDone = true;

                    if (m_LoadSuccessCallback != null)
                    {
                        m_LoadSuccessCallback.Invoke();
                    }

                    thread.Abort();
                    Debuger.Log(m_FileName + " 下载完成");
                }
            });

            //开启子线程
            thread.IsBackground = true;
            thread.Priority = m_ThreadPriority;
            thread.Start();
            Debuger.Log("开启线程：" + thread.Name);
        }

        /// <summary>
        /// 获取下载文件的大小
        /// </summary>
        /// <returns>The length.</returns>
        /// <param name="url">URL.</param>
        long GetLength(string url)
        {
            UnityEngine.Debug.Log(url);

            HttpWebRequest requet = HttpWebRequest.Create(url) as HttpWebRequest;
            requet.Method = "HEAD";
            HttpWebResponse response = requet.GetResponse() as HttpWebResponse;
            return response.ContentLength;
        }

        public void Close()
        {
            isStop = true;
        }

    }
}
