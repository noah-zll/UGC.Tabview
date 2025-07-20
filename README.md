# UGC.Tabview

一个灵活的Unity UI标签页组件，支持页面切换和数据刷新功能。

## 特性

- 支持多种标签页切换方式
- 可自定义标签页外观和行为
- 支持页面切换和数据刷新两种模式
- 提供丰富的事件回调
- 易于扩展和定制

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

## 快速开始

### 基本用法

```csharp
// 创建标签页控制器
var tabViewController = gameObject.AddComponent<TabViewController>();

// 添加标签页
var tab1 = tabViewController.AddTab("Tab1", tab1Content);
var tab2 = tabViewController.AddTab("Tab2", tab2Content);

// 切换到指定标签页
tabViewController.SwitchToTab("Tab1");

// 监听标签页切换事件
tabViewController.OnTabChanged.AddListener((tabId) => {
    Debug.Log($"Switched to tab: {tabId}");
});
```

### 页面切换模式

```csharp
// 设置为页面切换模式
tabViewController.SetMode(TabViewMode.PageSwitch);

// 添加页面
tabViewController.AddPage("Page1", page1GameObject);
tabViewController.AddPage("Page2", page2GameObject);

// 切换页面
tabViewController.SwitchToPage("Page1");
```

### 数据刷新模式

```csharp
// 设置为数据刷新模式
tabViewController.SetMode(TabViewMode.DataRefresh);

// 设置数据提供者
tabViewController.SetDataProvider(new MyDataProvider());

// 切换数据
tabViewController.SwitchToData("Data1");
```

## 文档

详细文档请参阅 [Documentation](Documentation/API.md) 文件夹。

## 示例

示例场景和预制体可在 [Tests](Tests/) 文件夹中找到。

## 许可

此项目采用 MIT 许可证 - 详情请参阅 [LICENSE](LICENSE) 文件。