using LarkFramework.Tick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkFramework.Procedure
{
    /// <summary>
    /// 流程基类
    /// </summary>
    public abstract class ProcedureBase : MonoBehaviour
    {
        /// <summary>
        /// 状态初始化
        /// </summary>
        public virtual void OnInit()
        {
            TickComponent.Instance.onUpdate += OnUpDate;
        }

        /// <summary>
        /// 进入状态时调用
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// 状态轮询时调用
        /// </summary>
        public abstract void OnUpDate();

        /// <summary>
        /// 离开状态时调用
        /// </summary>
        public virtual void OnLeave()
        {
            TickComponent.Instance.onUpdate -= OnUpDate;
        }

        ///// <summary>
        ///// 状态销毁时调用
        ///// </summary>
        //public abstract void OnDestroy();
    }
}
