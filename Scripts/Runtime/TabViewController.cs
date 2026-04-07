using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UGC.Tabview
{
    /// <summary>
    /// 标签页控制器，管理标签页的切换和事件处理
    /// </summary>
    public class TabViewController : MonoBehaviour
    {
        #region 属性和字段

        /// <summary>
        /// 标签页集合
        /// </summary>
        private Dictionary<string, TabItem> tabs = new Dictionary<string, TabItem>();

        /// <summary>
        /// 页面集合（页面切换模式）
        /// </summary>
        private Dictionary<string, GameObject> pages = new Dictionary<string, GameObject>();

        /// <summary>
        /// 数据提供者（数据刷新模式）
        /// </summary>
        private ITabDataProvider dataProvider;

        /// <summary>
        /// 当前选中的标签页ID
        /// </summary>
        public string CurrentTabId { get; private set; }

        /// <summary>
        /// 标签页数量
        /// </summary>
        public int TabCount => tabs.Count;

        /// <summary>
        /// 标签页模式
        /// </summary>
        [SerializeField]
        private TabViewMode mode = TabViewMode.PageSwitch;

        /// <summary>
        /// 标签页模式
        /// </summary>
        public TabViewMode Mode
        {
            get { return mode; }
            private set { mode = value; }
        }

        /// <summary>
        /// 是否启用样式器
        /// </summary>
        [SerializeField]
        private bool enableStyler = true;

        /// <summary>
        /// 样式器
        /// </summary>
        private TabViewStyler styler = new TabViewStyler();

        /// <summary>
        /// 动画器
        /// </summary>
        private TabViewAnimator animator = new TabViewAnimator();

        /// <summary>
        /// 当前数据
        /// </summary>
        private object currentData;

        #endregion

        #region 事件

        /// <summary>
        /// 标签页切换事件
        /// </summary>
        public UnityEvent<string> OnTabChanged = new UnityEvent<string>();

        /// <summary>
        /// 页面切换事件（页面切换模式）
        /// </summary>
        public UnityEvent<string> OnPageChanged = new UnityEvent<string>();

        /// <summary>
        /// 数据切换事件（数据刷新模式）
        /// </summary>
        public UnityEvent<string> OnDataChanged = new UnityEvent<string>();

        #endregion

        #region Unity生命周期

        private void Start()
        {
            // 确保样式器和动画器已初始化
            if (styler == null)
                styler = new TabViewStyler();
            if (animator == null)
                animator = new TabViewAnimator();
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 添加标签页
        /// </summary>
        /// <param name="tabId">标签页ID</param>
        /// <param name="tabButton">标签页按钮对象</param>
        /// <returns>创建的标签页</returns>
        public TabItem AddTab(string tabId, GameObject tabButton)
        {
            if (string.IsNullOrEmpty(tabId))
            {
                Debug.LogError("Tab ID cannot be null or empty");
                return null;
            }

            if (tabButton == null)
            {
                Debug.LogError($"Tab button for {tabId} cannot be null");
                return null;
            }

            // 确保按钮对象有Button组件
            if (tabButton.GetComponent<UnityEngine.UI.Button>() == null)
            {
                Debug.LogError($"Tab button for {tabId} must have a Button component");
                return null;
            }

            if (tabs.ContainsKey(tabId))
            {
                Debug.LogWarning($"Tab with ID {tabId} already exists, replacing it");
                RemoveTab(tabId);
            }

            // 确保样式器已初始化
            if (styler == null)
            {
                styler = new TabViewStyler();
            }

            // 创建标签页
            TabItem tabItem = new TabItem(tabId, tabButton);

            // 设置点击事件
            tabItem.SetOnClickListener(() => SwitchToTab(tabId));

            // 应用样式
            if (enableStyler)
            {
                styler.ApplyStyle(tabItem, false);
            }

            // 添加到集合
            tabs.Add(tabId, tabItem);

            return tabItem;
        }

        /// <summary>
        /// 移除标签页
        /// </summary>
        /// <param name="tabId">标签页ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveTab(string tabId)
        {
            if (!tabs.TryGetValue(tabId, out TabItem tabItem))
            {
                return false;
            }

            // 如果移除的是当前选中的标签页，则切换到其他标签页
            if (tabId == CurrentTabId && tabs.Count > 1)
            {
                foreach (var id in tabs.Keys)
                {
                    if (id != tabId)
                    {
                        SwitchToTab(id);
                        break;
                    }
                }
            }

            // 从集合中移除
            tabs.Remove(tabId);

            // 如果是页面切换模式，同时移除对应的页面
            if (Mode == TabViewMode.PageSwitch && pages.ContainsKey(tabId))
            {
                pages.Remove(tabId);
            }

            return true;
        }

        /// <summary>
        /// 切换到指定标签页
        /// </summary>
        /// <param name="tabId">标签页ID</param>
        public void SwitchToTab(string tabId)
        {
            if (!tabs.ContainsKey(tabId))
            {
                Debug.LogError($"Tab with ID {tabId} does not exist");
                return;
            }

            // 如果已经是当前标签页，则不做任何操作
            if (tabId == CurrentTabId)
            {
                return;
            }

            // 取消选中当前标签页
            if (!string.IsNullOrEmpty(CurrentTabId) && tabs.TryGetValue(CurrentTabId, out TabItem currentTab))
            {
                currentTab.Deselect();
                if (enableStyler)
                {
                    styler.ApplyStyle(currentTab, false);
                }
            }

            // 选中新标签页
            TabItem newTab = tabs[tabId];
            newTab.Select();
            if (enableStyler)
            {
                styler.ApplyStyle(newTab, true);
            }

            // 更新当前标签页ID
            string previousTabId = CurrentTabId;
            CurrentTabId = tabId;

            // 根据模式执行不同的操作
            switch (Mode)
            {
                case TabViewMode.PageSwitch:
                    SwitchToPage(tabId);
                    break;

                case TabViewMode.DataRefresh:
                    SwitchToData(tabId);
                    break;
            }

            // 触发标签页切换事件
            OnTabChanged.Invoke(tabId);
        }

        /// <summary>
        /// 获取标签页
        /// </summary>
        /// <param name="tabId">标签页ID</param>
        /// <returns>标签页</returns>
        public TabItem GetTab(string tabId)
        {
            if (tabs.TryGetValue(tabId, out TabItem tabItem))
            {
                return tabItem;
            }

            return null;
        }

        /// <summary>
        /// 设置标签页模式
        /// </summary>
        /// <param name="mode">模式</param>
        public void SetMode(TabViewMode mode)
        {
            Mode = mode;
        }

        /// <summary>
        /// 设置样式器
        /// </summary>
        /// <param name="styler">样式器</param>
        public void SetStyler(TabViewStyler styler)
        {
            this.styler = styler ?? new TabViewStyler();

            // 应用样式到所有标签页
            if (enableStyler)
            {
                foreach (var tabItem in tabs.Values)
                {
                    this.styler.ApplyStyle(tabItem, tabItem.IsSelected);
                }
            }
        }

        /// <summary>
        /// 设置动画器
        /// </summary>
        /// <param name="animator">动画器</param>
        public void SetAnimator(TabViewAnimator animator)
        {
            TabViewAnimator oldAnimator = this.animator;
            this.animator = animator ?? new TabViewAnimator();

            // 如果从Fade切换到其他类型，需要重置所有页面的CanvasGroup.alpha值
            if (oldAnimator != null && oldAnimator.AnimationType == AnimationType.Fade &&
                this.animator.AnimationType != AnimationType.Fade)
            {
                ResetAllPagesCanvasGroupAlpha();
            }
        }

        /// <summary>
        /// 重置所有页面的CanvasGroup.alpha值
        /// </summary>
        private void ResetAllPagesCanvasGroupAlpha()
        {
            foreach (var page in pages.Values)
            {
                if (page != null)
                {
                    CanvasGroup canvasGroup = page.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 1f;
                    }
                }
            }
        }

        /// <summary>
        /// 获取样式器
        /// </summary>
        /// <returns>样式器</returns>
        public TabViewStyler GetStyler()
        {
            return styler;
        }

        /// <summary>
        /// 获取动画器
        /// </summary>
        /// <returns>动画器</returns>
        public TabViewAnimator GetAnimator()
        {
            return animator;
        }

        #endregion

        #region 页面切换模式

        /// <summary>
        /// 添加页面
        /// </summary>
        /// <param name="pageId">页面ID</param>
        /// <param name="pageContent">页面内容对象</param>
        public void AddPage(string pageId, GameObject pageContent)
        {
            if (string.IsNullOrEmpty(pageId))
            {
                Debug.LogError("Page ID cannot be null or empty");
                return;
            }

            if (pageContent == null)
            {
                Debug.LogError($"Page content for {pageId} cannot be null");
                return;
            }

            if (pages.ContainsKey(pageId))
            {
                Debug.LogWarning($"Page with ID {pageId} already exists, replacing it");
                pages.Remove(pageId);
            }

            // 初始时隐藏页面
            pageContent.SetActive(false);

            // 添加到集合
            pages.Add(pageId, pageContent);
        }

        /// <summary>
        /// 移除页面
        /// </summary>
        /// <param name="pageId">页面ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemovePage(string pageId)
        {
            return pages.Remove(pageId);
        }

        /// <summary>
        /// 切换到指定页面
        /// </summary>
        /// <param name="pageId">页面ID</param>
        public void SwitchToPage(string pageId)
        {
            if (!pages.ContainsKey(pageId))
            {
                Debug.LogError($"Page with ID {pageId} does not exist");
                return;
            }

            // 隐藏当前页面
            foreach (var page in pages.Values)
            {
                if (page.activeSelf)
                {
                    StartCoroutine(animator.PlayPageExitAnimation(page));
                }
            }

            // 显示新页面
            GameObject newPage = pages[pageId];
            StartCoroutine(animator.PlayPageEnterAnimation(newPage));

            // 触发页面切换事件
            OnPageChanged.Invoke(pageId);
        }

        #endregion

        #region 数据刷新模式

        /// <summary>
        /// 设置数据提供者
        /// </summary>
        /// <param name="dataProvider">数据提供者</param>
        public void SetDataProvider(ITabDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        /// <summary>
        /// 切换到指定数据
        /// </summary>
        /// <param name="dataId">数据ID</param>
        public void SwitchToData(string dataId)
        {
            if (dataProvider == null)
            {
                Debug.LogError("Data provider is not set");
                return;
            }

            // 获取数据
            currentData = dataProvider.GetData(dataId);

            // 触发数据切换事件
            OnDataChanged.Invoke(dataId);
        }

        /// <summary>
        /// 刷新当前数据
        /// </summary>
        public void RefreshCurrentData()
        {
            if (dataProvider == null || string.IsNullOrEmpty(CurrentTabId))
            {
                return;
            }

            // 刷新数据
            dataProvider.RefreshData(CurrentTabId);

            // 重新获取数据
            currentData = dataProvider.GetData(CurrentTabId);

            // 触发数据切换事件
            OnDataChanged.Invoke(CurrentTabId);
        }

        /// <summary>
        /// 获取当前数据
        /// </summary>
        /// <returns>当前数据</returns>
        public object GetCurrentData()
        {
            return currentData;
        }

        /// <summary>
        /// 清除当前标签页缓存
        /// </summary>
        public void ClearCurrentTab()
        {
            CurrentTabId = null;
            currentData = null;
        }

        #endregion
    }
}