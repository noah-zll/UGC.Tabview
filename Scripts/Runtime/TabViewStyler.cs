using System;
using UnityEngine;

namespace UGC.Tabview
{
    /// <summary>
    /// 用于自定义标签页样式的类
    /// </summary>
    [Serializable]
    public class TabViewStyler
    {
        /// <summary>
        /// 正常状态背景颜色
        /// </summary>
        public Color NormalBackgroundColor { get; private set; } = new Color(0.8f, 0.8f, 0.8f, 1f);
        
        /// <summary>
        /// 正常状态文本颜色
        /// </summary>
        public Color NormalTextColor { get; private set; } = Color.black;
        
        /// <summary>
        /// 选中状态背景颜色
        /// </summary>
        public Color SelectedBackgroundColor { get; private set; } = new Color(0.2f, 0.6f, 1f, 1f);
        
        /// <summary>
        /// 选中状态文本颜色
        /// </summary>
        public Color SelectedTextColor { get; private set; } = Color.white;
        
        /// <summary>
        /// 设置正常状态样式
        /// </summary>
        /// <param name="backgroundColor">背景颜色</param>
        /// <param name="textColor">文本颜色</param>
        public void SetNormalStyle(Color backgroundColor, Color textColor)
        {
            NormalBackgroundColor = backgroundColor;
            NormalTextColor = textColor;
        }
        
        /// <summary>
        /// 设置选中状态样式
        /// </summary>
        /// <param name="backgroundColor">背景颜色</param>
        /// <param name="textColor">文本颜色</param>
        public void SetSelectedStyle(Color backgroundColor, Color textColor)
        {
            SelectedBackgroundColor = backgroundColor;
            SelectedTextColor = textColor;
        }
        
        /// <summary>
        /// 应用样式到标签页
        /// </summary>
        /// <param name="tabItem">标签页</param>
        /// <param name="isSelected">是否选中</param>
        public void ApplyStyle(TabItem tabItem, bool isSelected)
        {
            if (tabItem == null) return;
            
            if (isSelected)
            {
                tabItem.ApplyStyle(SelectedBackgroundColor, SelectedTextColor);
            }
            else
            {
                tabItem.ApplyStyle(NormalBackgroundColor, NormalTextColor);
            }
        }
    }
}