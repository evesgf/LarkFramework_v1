using UnityEngine;
using System.Collections;

namespace LarkFramework.Test
{
    public class FSM_Example : MonoBehaviour
    {
        FSM fsm = new FSM();

        //创建状态
        FSM.FSMState state_Idle = new FSM.FSMState("idle");
        FSM.FSMState state_Run = new FSM.FSMState("run");
        FSM.FSMState state_Jump = new FSM.FSMState("jump");

        // Use this for initialization
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(fsm.mCurState.name.ToString());

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                fsm.Start(state_Run);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                fsm.Start(state_Jump);
            }
        }

        void Init()
        {

            //创建跳转
            FSM.FSMTranslation touchTranslation1 = new FSM.FSMTranslation(state_Idle, "touch_down", state_Jump,Jump);
            FSM.FSMTranslation landTranslation1 = new FSM.FSMTranslation(state_Jump, "land", state_Run,Run);

            //添加状态
            fsm.AddState(state_Idle);
            fsm.AddState(state_Jump);
            fsm.AddState(state_Run);

            //添加跳转
            fsm.AddTranslation(touchTranslation1);
            fsm.AddTranslation(landTranslation1);

            fsm.Start(state_Run);
        }

        void Jump()
        {
            Debug.Log("this is jump Func");
        }

        void Run()
        {
            Debug.Log("this is Run Func");
        }
    }
}