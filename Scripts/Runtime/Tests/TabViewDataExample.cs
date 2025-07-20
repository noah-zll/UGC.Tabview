using UnityEngine;
using UnityEngine.UI;
using UGC.Tabview;
using System.Collections.Generic;

namespace UGC.Tabview.Tests
{
    /// <summary>
    /// TabViewController数据刷新模式的示例使用脚本
    /// </summary>
    public class TabViewDataExample : MonoBehaviour
    {
        [Header("Tab View Controller")]
        [SerializeField] private TabViewController tabViewController;
        
        [Header("Tab Buttons")]
        [SerializeField] private GameObject dataTab1Button;
        [SerializeField] private GameObject dataTab2Button;
        [SerializeField] private GameObject dataTab3Button;
        
        [Header("UI References")]
        [SerializeField] private Text titleText;
        [SerializeField] private Text contentText;
        [SerializeField] private Text statusText;
        [SerializeField] private Button refreshButton;
        
        // 数据提供者
        private ExampleDataProvider dataProvider;
        
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
            
            // 设置为数据刷新模式
            tabViewController.SetMode(TabViewMode.DataRefresh);
            
            // 创建数据提供者
            dataProvider = new ExampleDataProvider();
            tabViewController.SetDataProvider(dataProvider);
            
            // 添加标签页
            tabViewController.AddTab("Data1", dataTab1Button);
            tabViewController.AddTab("Data2", dataTab2Button);
            tabViewController.AddTab("Data3", dataTab3Button);
            
            // 设置样式
            TabViewStyler styler = new TabViewStyler();
            styler.SetNormalStyle(new Color(0.8f, 0.8f, 0.8f, 1f), Color.black);
            styler.SetSelectedStyle(new Color(0.2f, 0.6f, 1f, 1f), Color.white);
            tabViewController.SetStyler(styler);
            
            // 监听事件
            tabViewController.OnDataChanged.AddListener(OnDataChanged);
            
            // 设置刷新按钮点击事件
            if (refreshButton != null)
            {
                refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            }
            
            // 默认选中第一个标签页
            tabViewController.SwitchToTab("Data1");
        }
        
        /// <summary>
        /// 数据切换事件处理
        /// </summary>
        /// <param name="dataId">数据ID</param>
        private void OnDataChanged(string dataId)
        {
            Debug.Log($"Data changed to: {dataId}");
            
            // 获取数据
            ExampleData data = tabViewController.GetCurrentData() as ExampleData;
            if (data != null)
            {
                // 更新UI
                UpdateUI(data);
            }
            
            // 更新状态文本
            if (statusText != null)
            {
                statusText.text = $"Current Data: {dataId}";
            }
        }
        
        /// <summary>
        /// 刷新按钮点击事件处理
        /// </summary>
        private void OnRefreshButtonClicked()
        {
            // 刷新当前数据
            tabViewController.RefreshCurrentData();
        }
        
        /// <summary>
        /// 更新UI
        /// </summary>
        /// <param name="data">数据</param>
        private void UpdateUI(ExampleData data)
        {
            if (titleText != null)
            {
                titleText.text = data.Title;
            }
            
            if (contentText != null)
            {
                contentText.text = data.Content;
            }
        }
    }
    
    /// <summary>
    /// 示例数据类
    /// </summary>
    public class ExampleData
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int RefreshCount { get; set; }
        
        public ExampleData(string title, string content)
        {
            Title = title;
            Content = content;
            RefreshCount = 0;
        }
    }
    
    /// <summary>
    /// 示例数据提供者
    /// </summary>
    public class ExampleDataProvider : ITabDataProvider
    {
        private Dictionary<string, ExampleData> dataDict = new Dictionary<string, ExampleData>();
        
        public ExampleDataProvider()
        {
            // 初始化数据
            dataDict.Add("Data1", new ExampleData("数据1", "这是数据1的内容"));
            dataDict.Add("Data2", new ExampleData("数据2", "这是数据2的内容"));
            dataDict.Add("Data3", new ExampleData("数据3", "这是数据3的内容"));
        }
        
        public object GetData(string dataId)
        {
            if (dataDict.TryGetValue(dataId, out ExampleData data))
            {
                return data;
            }
            
            return null;
        }
        
        public void RefreshData(string dataId)
        {
            if (dataDict.TryGetValue(dataId, out ExampleData data))
            {
                // 更新数据
                data.RefreshCount++;
                data.Content = $"这是数据{dataId.Substring(4)}的内容，已刷新{data.RefreshCount}次";
            }
        }
    }
}