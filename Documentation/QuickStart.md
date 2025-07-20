# UGC.Tabview 快速入门

本指南将帮助你快速上手UGC.Tabview组件，创建自定义的标签页界面。

## 安装

### 通过Unity Package Manager

1. 打开Unity编辑器
2. 打开Package Manager (Window > Package Manager)
3. 点击 "+" 按钮
4. 选择 "Add package from git URL..."
5. 输入 `https://github.com/ugc/UGC.Tabview.git`
6. 点击 "Add"

### 手动安装

1. 下载此仓库
2. 将UGC.Tabview文件夹复制到你的Unity项目的Packages文件夹中

## 创建基本标签页

### 1. 创建UI结构

首先，创建一个基本的UI结构：

```
- Canvas
  - TabView (Panel)
    - TabButtons (Horizontal Layout Group)
      - Tab1Button (Button)
      - Tab2Button (Button)
    - TabPages (Panel)
      - Page1 (GameObject)
      - Page2 (GameObject)
```

### 2. 添加TabViewController组件

选择TabView对象，添加TabViewController组件：

```csharp
var tabViewController = tabViewGameObject.AddComponent<TabViewController>();
```

或者通过Inspector面板添加组件。

### 3. 设置标签页和页面

```csharp
// 获取TabViewController组件
var tabViewController = GetComponent<TabViewController>();

// 添加标签页
var tab1 = tabViewController.AddTab("Tab1", tab1ButtonGameObject);
var tab2 = tabViewController.AddTab("Tab2", tab2ButtonGameObject);

// 设置为页面切换模式
tabViewController.SetMode(TabViewMode.PageSwitch);

// 添加页面
tabViewController.AddPage("Tab1", page1GameObject);
tabViewController.AddPage("Tab2", page2GameObject);

// 切换到第一个标签页
tabViewController.SwitchToTab("Tab1");
```

### 4. 监听标签页切换事件

```csharp
// 监听标签页切换事件
tabViewController.OnTabChanged.AddListener((tabId) => {
    Debug.Log($"Switched to tab: {tabId}");
});
```

## 使用数据刷新模式

如果你希望在同一个页面上根据不同标签显示不同数据，可以使用数据刷新模式：

### 1. 创建数据提供者

```csharp
public class MyDataProvider : ITabDataProvider
{
    public object GetData(string dataId)
    {
        // 返回对应的数据
        switch (dataId)
        {
            case "Data1":
                return new MyData { Title = "Data 1", Content = "This is data 1" };
            case "Data2":
                return new MyData { Title = "Data 2", Content = "This is data 2" };
            default:
                return null;
        }
    }
    
    public void RefreshData(string dataId)
    {
        // 刷新数据逻辑
        Debug.Log($"Refreshing data: {dataId}");
    }
}

// 数据类
public class MyData
{
    public string Title { get; set; }
    public string Content { get; set; }
}
```

### 2. 设置数据刷新模式

```csharp
// 获取TabViewController组件
var tabViewController = GetComponent<TabViewController>();

// 添加标签页
var tab1 = tabViewController.AddTab("Data1", tab1ButtonGameObject);
var tab2 = tabViewController.AddTab("Data2", tab2ButtonGameObject);

// 设置为数据刷新模式
tabViewController.SetMode(TabViewMode.DataRefresh);

// 设置数据提供者
tabViewController.SetDataProvider(new MyDataProvider());

// 切换到第一个数据
tabViewController.SwitchToData("Data1");
```

### 3. 显示数据

```csharp
// 监听数据切换事件
tabViewController.OnDataChanged.AddListener((dataId) => {
    // 获取数据
    var data = tabViewController.GetCurrentData() as MyData;
    if (data != null)
    {
        // 更新UI
        titleText.text = data.Title;
        contentText.text = data.Content;
    }
});
```

## 自定义标签页样式

你可以使用TabViewStyler来自定义标签页的样式：

```csharp
// 获取TabViewController组件
var tabViewController = GetComponent<TabViewController>();

// 创建样式器
var styler = new TabViewStyler();

// 设置正常状态样式
styler.SetNormalStyle(Color.gray, Color.white);

// 设置选中状态样式
styler.SetSelectedStyle(Color.blue, Color.white);

// 应用样式到标签页控制器
tabViewController.SetStyler(styler);
```

## 添加切换动画

你可以使用TabViewAnimator为页面切换添加动画效果：

```csharp
// 获取TabViewController组件
var tabViewController = GetComponent<TabViewController>();

// 创建动画器
var animator = new TabViewAnimator();

// 设置页面切换动画
animator.SetPageSwitchAnimation(AnimationType.Fade, 0.3f);

// 应用动画器到标签页控制器
tabViewController.SetAnimator(animator);
```

## 更多示例

更多示例请参考 [Tests](../Tests/) 文件夹中的示例场景和预制体。