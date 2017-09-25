using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LarkFramework.UI
{
    public abstract partial class UIPanel
    {
        public GUIAnim[] guiAnims;

        public void GUIAniOpen()
        {
            foreach (var item in guiAnims)
            {
                item.MoveIn();
            }
        }

        /// <summary>
        /// GUIAni退出
        /// </summary>
        public void GUIAniClose()
        {
            foreach (var item in guiAnims)
            {
                item.MoveOut();
            }
        }
    }
}