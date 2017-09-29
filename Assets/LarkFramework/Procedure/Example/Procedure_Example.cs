using LarkFramework.Module;
using LarkFramework.Tick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkFramework.Procedure.Example
{
    public class Procedure_Example : MonoBehaviour
    {
        
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
