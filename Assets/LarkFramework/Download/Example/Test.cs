﻿using LarkFramework.Download;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using ThreadPriority = System.Threading.ThreadPriority;

/// <summary>
/// 断点续传测试
/// 参考：http://blog.csdn.net/dingxiaowei2013/article/details/77814966
/// 参考：http://blog.csdn.net/damenhanter/article/details/50273303
/// </summary>
public class Test : MonoBehaviour
{
    public string file;
    public string u;
    public string s;

    /// <summary>
    /// 下载文件名
    /// </summary>
    public string m_FileName { get; set; }
    /// <summary>
    /// 下载地址
    /// </summary>
    public string m_Url { get; set; }

    private string m_SavePath;                              //文件保存地址
    private int m_TimeOut=5*1000;                           //连接超时时间（毫秒）
    private int m_RWTimeOut=3*1000;                         //读写超时时间（毫秒）
    private int m_ReStartCount = 3;                         //断线重连次数
    private int m_ReStartInterval = 1000;                   //断线重连间隔时间
    private int m_FlushSize;                                //缓冲区大小
    private ThreadPriority m_ThreadPriority;                //线程优先级

    public LoadSuccessCallback m_LoadSuccessCallback;       //成功回调
    public LoadFailureCallback m_LoadFailureCallback;       //失败回调
    public LoadUpdateCallback m_LoadUpdateCallback;         //Tick回调

    /// <summary>
    /// 下载进度
    /// </summary>
    public float progress { get; set; }
    /// <summary>
    /// 下载是否完成
    /// </summary>
    public bool isDone { get; set; }
    /// <summary>
    /// 已经下载的文件大小
    /// </summary>
    public long loadLength { get; set; }
    /// <summary>
    /// 文件总长度
    /// </summary>
    public long totalLength { get; set; }

    private Stopwatch stopWatch;                            //下载计时
    private FileStream fs;                                  //下载文件流
    private HttpWebRequest request;                         //文件流

    //涉及子线程要注意,Unity关闭的时候子线程不会关闭，所以要有一个标识
    private bool isStop;
    //子线程负责下载，否则会阻塞主线程，Unity界面会卡住
    private Thread thread;

    private void Start()
    {
        Init(file,u,s,1024,5000,30,1000,
            delegate { print("成功"); },
            delegate { print("失败"); },
            delegate { print("更新"); },
            ThreadPriority.Normal);
    }

    public void Init(string fileName, string url, string savePath, int flushSize, int timeOut,int reStartCount,int reStartInterval, LoadSuccessCallback loadSuccessCallback = null, LoadFailureCallback loadFailureCallback = null, LoadUpdateCallback loadUpdateCallback = null, ThreadPriority threadPriority = ThreadPriority.Normal)
    {
        m_FileName = fileName;
        m_Url = url;
        m_SavePath = savePath;
        m_FlushSize = flushSize;
        m_TimeOut = timeOut;
        m_ReStartCount = reStartCount;
        m_ReStartInterval = reStartInterval;

        m_ThreadPriority = threadPriority;

        m_LoadSuccessCallback = loadSuccessCallback;
        m_LoadFailureCallback = loadFailureCallback;
        m_LoadUpdateCallback = loadUpdateCallback;
    }

    public void DownLoad()
    {
        thread = new Thread(delegate () {
            try
            {
                print("开始下载:" + m_FileName);

                isStop = false;
                stopWatch = new Stopwatch();
                stopWatch.Start();

                //判断保存路径是否存在
                if (!Directory.Exists(m_SavePath))
                {
                    Directory.CreateDirectory(m_SavePath);
                }

                //这是要下载的文件名，比如从服务器下载a.zip到D盘，保存的文件名是test
                string filePath = m_SavePath + "/" + m_FileName;

                //使用流操作文件
                fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                //获取文件现在的长度
                loadLength = fs.Length;
                //获取下载文件的总长度
                totalLength = GetLength(m_Url);

                print("<color=red>文件:" + m_FileName + " 已下载" + loadLength / 1024 + "K，剩余" + ((totalLength - loadLength) / 1024) + "K</color>");

                //如果没下载完
                if (loadLength < totalLength)
                {
                    //断点续传核心，设置本地文件流的起始位置
                    fs.Seek(loadLength, SeekOrigin.Begin);

                    request = WebRequest.Create(m_Url) as HttpWebRequest;

                    if (request != null)
                    {
                        request.ReadWriteTimeout = m_RWTimeOut;
                        request.Timeout = m_TimeOut;

                        //断点续传核心，设置远程访问文件流的起始位置
                        request.AddRange((int)loadLength);

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
                                    break;
                                }

                                //将内容再写入本地文件中
                                fs.Write(buffer, 0, length);
                                //计算进度
                                loadLength += length;
                                progress = (float)loadLength / (float)totalLength;
                                //UnityEngine.Debug.Log(progress);
                                //类似尾递归
                                length = stream.Read(buffer, 0, buffer.Length);
                            }
                        }
                        else
                        {
                            Debuger.Log("stream is null");
                        }
                        stream.Close();
                        stream.Dispose();
                    }
                    else
                    {
                        Debuger.Log("request is null");
                    }
                }
                else
                {
                    progress = 1;
                }

                print("耗时: " + stopWatch.ElapsedMilliseconds);
            }
            catch (WebException ex)
            {
                print(m_FileName + " 下载失败："+ex);
                throw ex;
            }
            finally
            {
                print("这波完了："+Thread.CurrentThread.ManagedThreadId);

                stopWatch.Stop();
                fs.Close();
                fs.Dispose();

                if (request != null) request.Abort();

                //如果下载完毕，执行回调
                if (progress == 1)
                {
                    isDone = true;

                    if (m_LoadSuccessCallback != null) m_LoadSuccessCallback.Invoke();

                    print(m_FileName + " 下载完成");
                }
                else
                {
                    if (m_ReStartCount > 0)
                    {
                        m_ReStartCount-=1;

                        Thread.Sleep(m_ReStartInterval);
                        print("重试:"+m_ReStartCount);
                        

                        if (thread != null) thread.Abort();
                    }
                    else
                    {

                        if (m_LoadFailureCallback != null) m_LoadFailureCallback.Invoke();
                        print(m_FileName + " 下载终止");
                    }
                }

                if (thread != null) thread.Abort();
            }
        });

        //开启子线程
        thread.IsBackground = true;
        thread.Priority = m_ThreadPriority;
        thread.Start();
        Debuger.Log("开启下载线程：" + Thread.CurrentThread.ManagedThreadId);
    }

    /// <summary>
    /// 获取下载文件的大小
    /// </summary>
    /// <returns>The length.</returns>
    /// <param name="url">URL.</param>
    long GetLength(string url)
    {
        HttpWebRequest re = WebRequest.Create(url) as HttpWebRequest;
        re.Method = "HEAD";
        HttpWebResponse response = re.GetResponse() as HttpWebResponse;
        return response.ContentLength;
    }
}
