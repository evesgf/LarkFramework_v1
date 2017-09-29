using LarkFramework.FSM;
using LarkFramework.Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkFramework.Procedure
{
    public class ProcedureManager : ServiceModule<ProcedureManager>
    {
        public const string LOG_TAG = "ProcedureManager";

        /// <summary>
        /// 流程状态机
        /// </summary>
        public static FSM<ProcedureManager> m_ProcedureFSM;

        public ProcedureManager()
        {

        }

        public void Init()
        {

        }
    }
}
