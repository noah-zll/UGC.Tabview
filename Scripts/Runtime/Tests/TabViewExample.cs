using UnityEngine;
using UnityEngine.UI;
using UGC.Tabview;

namespace UGC.Tabview.Tests
{
    /// <summary>
    /// TabViewController的示例使用脚本
    /// </summary>
    public class TabViewExample : MonoBehaviour
    {
        [Header("Tab View Controller")]
        [SerializeField] private TabViewController tabViewController;
        
        [Header("Tab Buttons")]
        [SerializeField] private GameObject tab1Button;
        [SerializeField] private GameObject tab2Button;
        [SerializeField] private GameObject tab3Button;
        
        [Header("Tab Pages")]
        [SerializeField] private GameObject page1;
        [SerializeField] private GameObject page2;
        [SerializeField] private GameObject page3;
        
        [Header("UI References")]
        [SerializeField] private Text statusText;
        
        private void Start()
        {
            // 如果没有指定TabViewController，则尝试获取或添加一个
            if (tabViewController == null)
            {
                tabViewController = GetComponent<TabViewController>();
                if (tabViewController == null)
                {
                    tabViewController = gameObject.AddComponent<TabViewController>();
                }
            }
            
            // 设置为页面切换模式
            tabViewController.SetMode(TabViewMode.PageSwitch);
            
            // 添加标签页和页面
            SetupTabs();
            
            // 设置样式
            SetupStyle();
            
            // 设置动画
            SetupAnimation();
            
            // 监听事件
            SetupEvents();
            
            // 默认选中第一个标签页
            tabViewController.SwitchToTab("Tab1");
            
            // 更新状态文本
            UpdateStatusText("Tab1");
        }
        
        /// <summary>
        /// 设置标签页和页面
        /// </summary>
        private void SetupTabs()
        {
            // 添加标签页
            tabViewController.AddTab("Tab1", tab1Button);
            tabViewController.AddTab("Tab2", tab2Button);
            tabViewController.AddTab("Tab3", tab3Button);
            
            // 添加页面
            tabViewController.AddPage("Tab1", page1);
            tabViewController.AddPage("Tab2", page2);
            tabViewController.AddPage("Tab3", page3);
        }
        
        /// <summary>
        /// 设置样式
        /// </summary>
        private void SetupStyle()
        {
            TabViewStyler styler = new TabViewStyler();
            styler.SetNormalStyle(new Color(0.8f, 0.8f, 0.8f, 1f), Color.black);
            styler.SetSelectedStyle(new Color(0.2f, 0.6f, 1f, 1f), Color.white);
            tabViewController.SetStyler(styler);
        }
        
        /// <summary>
        /// 设置动画
        /// </summary>
        private void SetupAnimation()
        {
            TabViewAnimator animator = new TabViewAnimator();
            animator.SetPageSwitchAnimation(AnimationType.Fade, 0.3f);
            tabViewController.SetAnimator(animator);
        }
        
        /// <summary>
        /// 设置事件监听
        /// </summary>
        private void SetupEvents()
        {
            // 监听标签页切换事件
            tabViewController.OnTabChanged.AddListener(OnTabChanged);
            
            // 监听页面切换事件
            tabViewController.OnPageChanged.AddListener(OnPageChanged);
        }
        
        /// <summary>
        /// 标签页切换事件处理
        /// </summary>
        /// <param name="tabId">标签页ID</param>
        private void OnTabChanged(string tabId)
        {
            Debug.Log($"Tab changed to: {tabId}");
            UpdateStatusText(tabId);
        }
        
        /// <summary>
        /// 页面切换事件处理
        /// </summary>
        /// <param name="pageId">页面ID</param>
        private void OnPageChanged(string pageId)
        {
            Debug.Log($"Page changed to: {pageId}");
        }
        
        /// <summary>
        /// 更新状态文本
        /// </summary>
        /// <param name="tabId">标签页ID</param>
        private void UpdateStatusText(string tabId)
        {
            if (statusText != null)
            {
                statusText.text = $"Current Tab: {tabId}";
            }
        }
    }
}