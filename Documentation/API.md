# UGC.Tabview API 文档

## 核心类

### TabViewController

标签页控制器，管理标签页的切换和事件处理。

#### 属性

| 属性名 | 类型 | 描述 |
|--------|------|------|
| CurrentTabId | string | 获取当前选中的标签页ID |
| TabCount | int | 获取标签页数量 |
| Mode | TabViewMode | 获取或设置标签页模式（页面切换/数据刷新） |

#### 方法

##### 基本操作

```csharp
// 添加标签页
public TabItem AddTab(string tabId, GameObject tabButton);

// 移除标签页
public bool RemoveTab(string tabId);

// 切换到指定标签页
public void SwitchToTab(string tabId);

// 获取标签页
public TabItem GetTab(string tabId);

// 设置标签页模式
public void SetMode(TabViewMode mode);
```

##### 页面切换模式

```csharp
// 添加页面
public void AddPage(string pageId, GameObject pageContent);

// 移除页面
public bool RemovePage(string pageId);

// 切换到指定页面
public void SwitchToPage(string pageId);
```

##### 数据刷新模式

```csharp
// 设置数据提供者
public void SetDataProvider(ITabDataProvider dataProvider);

// 切换到指定数据
public void SwitchToData(string dataId);

// 刷新当前数据
public void RefreshCurrentData();
```

#### 事件

```csharp
// 标签页切换事件
public UnityEvent<string> OnTabChanged;

// 页面切换事件（页面切换模式）
public UnityEvent<string> OnPageChanged;

// 数据切换事件（数据刷新模式）
public UnityEvent<string> OnDataChanged;
```

### TabItem

表示单个标签页的类。

#### 属性

| 属性名 | 类型 | 描述 |
|--------|------|------|
| Id | string | 标签页ID |
| Button | GameObject | 标签页按钮对象 |
| IsSelected | bool | 是否被选中 |

#### 方法

```csharp
// 选中标签页
public void Select();

// 取消选中标签页
public void Deselect();
```

### ITabDataProvider

数据刷新模式下的数据提供者接口。

```csharp
public interface ITabDataProvider
{
    // 获取数据
    object GetData(string dataId);
    
    // 刷新数据
    void RefreshData(string dataId);
}
```

## 枚举

### TabViewMode

标签页模式枚举。

```csharp
public enum TabViewMode
{
    // 页面切换模式 - 切换标签时显示/隐藏不同页面
    PageSwitch,
    
    // 数据刷新模式 - 切换标签时刷新数据
    DataRefresh
}
```

## 扩展类

### TabViewStyler

用于自定义标签页样式的类。

#### 方法

```csharp
// 设置标签页正常状态样式
public void SetNormalStyle(Color backgroundColor, Color textColor);

// 设置标签页选中状态样式
public void SetSelectedStyle(Color backgroundColor, Color textColor);

// 应用样式到标签页
public void ApplyStyle(TabItem tabItem, bool isSelected);
```

### TabViewAnimator

用于为标签页切换添加动画效果的类。

#### 方法

```csharp
// 设置页面切换动画
public void SetPageSwitchAnimation(AnimationType animationType, float duration);

// 播放页面进入动画
public void PlayPageEnterAnimation(GameObject page);

// 播放页面退出动画
public void PlayPageExitAnimation(GameObject page);
```

## 示例

### 基本用法

```csharp
// 创建标签页控制器
var tabViewController = gameObject.AddComponent<TabViewController>();

// 添加标签页
var tab1 = tabViewController.AddTab("Tab1", tab1Button);
var tab2 = tabViewController.AddTab("Tab2", tab2Button);

// 设置为页面切换模式
tabViewController.SetMode(TabViewMode.PageSwitch);

// 添加页面
tabViewController.AddPage("Tab1", page1GameObject);
tabViewController.AddPage("Tab2", page2GameObject);

// 切换到第一个标签页
tabViewController.SwitchToTab("Tab1");

// 监听标签页切换事件
tabViewController.OnTabChanged.AddListener((tabId) => {
    Debug.Log($"Switched to tab: {tabId}");
});
```

### 数据刷新模式

```csharp
// 创建数据提供者
public class MyDataProvider : ITabDataProvider
{
    public object GetData(string dataId)
    {
        // 返回对应的数据
        return new MyData { Id = dataId };
    }
    
    public void RefreshData(string dataId)
    {
        // 刷新数据逻辑
        Debug.Log($"Refreshing data: {dataId}");
    }
}

// 创建标签页控制器
var tabViewController = gameObject.AddComponent<TabViewController>();

// 添加标签页
var tab1 = tabViewController.AddTab("Data1", tab1Button);
var tab2 = tabViewController.AddTab("Data2", tab2Button);

// 设置为数据刷新模式
tabViewController.SetMode(TabViewMode.DataRefresh);

// 设置数据提供者
tabViewController.SetDataProvider(new MyDataProvider());

// 切换到第一个数据
tabViewController.SwitchToData("Data1");

// 监听数据切换事件
tabViewController.OnDataChanged.AddListener((dataId) => {
    Debug.Log($"Data changed: {dataId}");
});
```