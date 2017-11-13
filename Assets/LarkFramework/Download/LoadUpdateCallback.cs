﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LarkFramework.Download
{
    /// <summary>
    /// 下载中回调函数。
    /// </summary>
    public delegate void LoadUpdateCallback(float processValue, int fileTotalSize = 0);
}
