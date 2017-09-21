﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LarkFramework.UI
{
    public class UIComponent:MonoBehaviour
    {
        public const string LOG_TAG = "UIComponent";

        /// <summary>
        /// 从UIRoot下通过类型寻找一个组件对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Find<T>() where T : MonoBehaviour
        {
            string name = typeof(T).Name;
            GameObject obj = Find(name);
            if (obj != null)
            {
                return obj.GetComponent<T>();
            }

            return null;
        }

        /// <summary>
        /// 从UIRoot下通过名字和类型寻找一个组件对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Find<T>(string name) where T : MonoBehaviour
        {
            GameObject obj = Find(name);
            if (obj != null)
            {
                return obj.GetComponent<T>();
            }

            return null;
        }

        /// <summary>
        /// 在UIRoot下通过名字寻找一个GameObject对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject Find(string name)
        {
            Transform obj = null;
            GameObject root = FindUIComponent();
            if (root != null)
            {
                obj = root.transform.Find(name);
            }

            if (obj != null)
            {
                return obj.gameObject;
            }

            return null;
        }

        /// <summary>
        /// 当前场景中寻找UIRoot对象
        /// </summary>
        /// <returns></returns>
        public static GameObject FindUIComponent()
        {
            GameObject root = GameObject.Find("UIComponent");
            if (root != null && root.GetComponent<UIComponent>() != null)
            {
                return root;
            }

            Debuger.LogError(LOG_TAG, "FindUIComponent() UIComponent Is Not Exist!!!");
            return root;
        }

        /// <summary>
        /// 当一个UIPage/UIWindow/UIWidget添加到UIRoot下面
        /// </summary>
        /// <param name="child"></param>
        public static void AddChild(UIPanel child)
        {
            GameObject root = FindUIComponent();
            if (root == null || child == null)
            {
                return;
            }

            child.transform.SetParent(root.transform, false);
            return;
        }
    }
}
