using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static FSM;

namespace Assets.LarkFramework.FSM
{
    public class MainFSM:FSMBase
    {
        private readonly Dictionary<string, FSMState> m_States;

        private FSMState m_CurrentState;
        public FSMState CurrentState
        {
            get { return m_CurrentState; }
        }

        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return m_CurrentState != null;
            }
        }

        /// <summary>
        /// 状态机初始化
        /// </summary>
        /// <param name="name"></param>
        /// <param name="states"></param>
        public MainFSM(string name, params FSMState[] states):base(name)
        {
            m_States = new Dictionary<string, FSMState>();

            foreach (var state in states)
            {
                string stateName = state.GetType().FullName;
                if (m_States.ContainsKey(stateName))
                {
                    throw new Exception(stateName+" FSM Error");
                }

                m_States.Add(stateName, state);
                state.OnInit();
            }

            m_CurrentState = null;
        }

        public void Update()
        {
            if (m_CurrentState == null) return;

            m_CurrentState.OnUpdate();
        }

        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                throw new Exception("FSM IsRunning");
            }

            FSMState state = GetState(stateType);
            if (state == null)
            {
                throw new Exception("GetState null " + stateType.FullName);
            }

            m_CurrentState = state;
            m_CurrentState.OnEnter();
        }

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        /// <param name="stateType">要获取的有限状态机状态类型。</param>
        /// <returns>要获取的有限状态机状态。</returns>
        public FSMState GetState(Type stateType)
        {
            FSMState state = null;
            if (m_States.TryGetValue(stateType.FullName, out state))
            {
                return state;
            }

            return null;
        }

        public void ChangeState(Type stateType)
        {
            if (m_CurrentState==null)
            {
                throw new Exception("m_CurrentState null");
            }

            FSMState state = GetState(stateType);
            if (state == null)
            {
                throw new Exception("GetState null " + stateType.FullName);
            }

            m_CurrentState.OnLeave();
            m_CurrentState = state;
            m_CurrentState.OnEnter();
        }
    }
}
