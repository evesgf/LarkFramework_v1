using LarkFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkFramework.Tick
{
    public class TickComponent : SingletonMono<TickComponent>
    {
        #region 全局生命周期回调
        public delegate void LifeCircleCallback();

        public LifeCircleCallback onUpdate = null;
        public LifeCircleCallback onFixedUpdate = null;
        public LifeCircleCallback onLatedUpdate = null;
        public LifeCircleCallback onGUI = null;
        public LifeCircleCallback onDestroy = null;
        public LifeCircleCallback onApplicationQuit = null;

        void Update()
        {
            if (this.onUpdate != null)
                this.onUpdate();
        }

        void FixedUpdate()
        {
            if (this.onFixedUpdate != null)
                this.onFixedUpdate();

        }

        void LatedUpdate()
        {
            if (this.onLatedUpdate != null)
                this.onLatedUpdate();
        }

        void OnGUI()
        {
            if (this.onGUI != null)
                this.onGUI();
        }

        protected void OnDestroy()
        {
            if (this.onDestroy != null)
                this.onDestroy();
        }

        void OnApplicationQuit()
        {
            if (this.onApplicationQuit != null)
                this.onApplicationQuit();
        }
        #endregion
    }
}
