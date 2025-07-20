using System;

namespace UGC.Tabview
{
    /// <summary>
    /// 标签页模式枚举
    /// </summary>
    public enum TabViewMode
    {
        /// <summary>
        /// 页面切换模式 - 切换标签时显示/隐藏不同页面
        /// </summary>
        PageSwitch,
        
        /// <summary>
        /// 数据刷新模式 - 切换标签时刷新数据
        /// </summary>
        DataRefresh
    }

    /// <summary>
    /// 动画类型枚举
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// 无动画
        /// </summary>
        None,
        
        /// <summary>
        /// 淡入淡出
        /// </summary>
        Fade,
        
        /// <summary>
        /// 滑动
        /// </summary>
        Slide,
        
        /// <summary>
        /// 缩放
        /// </summary>
        Scale
    }
}