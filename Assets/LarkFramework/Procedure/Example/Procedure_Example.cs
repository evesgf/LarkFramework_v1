using LarkFramework.Module;
using LarkFramework.Tick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkFramework.Procedure.Example
{
    public class Procedure_Example : MonoBehaviour
    {
        public static FSM.FSMState m_ProcedureA;
        public static FSM.FSMState m_ProcedureB;

        // Use this for initialization
        void Start()
        {
            ModuleManager.Instance.Init("LarkFramework.Procedure.Example");

            //InitTick
            TickManager.Instance.Init();

            ProcedureManager.Instance.Init();

            //创建状态
            m_ProcedureA = new FSM.FSMState("ProcedureA");
            m_ProcedureB = new FSM.FSMState("ProcedureB");

            ProcedureManager.m_ProcedureFSM.AddState(m_ProcedureA);
            ProcedureManager.m_ProcedureFSM.AddState(m_ProcedureB);

            //创建跳转
            FSM.FSMTranslation touchTranslation1 = new FSM.FSMTranslation(m_ProcedureA, "toB", m_ProcedureB, DoA);
            FSM.FSMTranslation landTranslation1 = new FSM.FSMTranslation(m_ProcedureA, "toA", m_ProcedureA, DoB);

            ProcedureManager.m_ProcedureFSM.Start(m_ProcedureA);
        }

        public void DoA()
        {
            Debug.Log("DoA");
        }
        public void DoB()
        {
            Debug.Log("DoB");
        }


        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ProcedureManager.m_ProcedureFSM.Start(m_ProcedureA);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ProcedureManager.m_ProcedureFSM.Start(m_ProcedureB);
            }
        }
    }

    public class ProcedureA : ProcedureBase
    {
        public override void OnEnter()
        {
            Debug.Log("ProcedureA OnEnter");
        }

        public override void OnInit()
        {
            Debug.Log("ProcedureA OnInit");
        }

        public override void OnLeave()
        {
            Debug.Log("ProcedureA OnLeave");
        }

        public override void OnUpDate()
        {
            Debug.Log("ProcedureA OnUpDate");
        }
    }

    public class ProcedureB : ProcedureBase
    {
        public override void OnEnter()
        {
            Debug.Log("ProcedureB OnEnter");
        }

        public override void OnInit()
        {
            Debug.Log("ProcedureB OnInit");
        }

        public override void OnLeave()
        {
            Debug.Log("ProcedureB OnLeave");
        }

        public override void OnUpDate()
        {
            Debug.Log("ProcedureB OnUpDate");
        }
    }
}
