using UnityEngine;
using UnityEditor;
using UGC.Tabview;
using System.Collections.Generic;
using System.Linq;

namespace UGC.Tabview.Editor
{
    /// <summary>
    /// TabViewController的自定义编辑器
    /// </summary>
    [CustomEditor(typeof(TabViewController))]
    public class TabViewControllerEditor : UnityEditor.Editor
    {
        // 序列化属性
        private SerializedProperty modeProperty;
        private SerializedProperty enableStylerProperty;
        
        // 折叠状态
        private bool showTabs = true;
        private bool showPages = true;
        private bool showStyle = true;
        private bool showAnimation = true;
        
        // 临时变量
        private string newTabId = "";
        private GameObject newTabButton = null;
        private string newPageId = "";
        private GameObject newPageContent = null;
        private AnimationType selectedAnimationType = AnimationType.Fade;
        private float animationDuration = 0.3f;
        private Color normalBgColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        private Color normalTextColor = Color.black;
        private Color selectedBgColor = new Color(0.2f, 0.6f, 1f, 1f);
        private Color selectedTextColor = Color.white;
        
        private void OnEnable()
        {
            // 获取序列化属性
            modeProperty = serializedObject.FindProperty("mode");
            enableStylerProperty = serializedObject.FindProperty("enableStyler");
            
            // 获取当前样式和动画设置
            TabViewController controller = (TabViewController)target;
            if (controller != null)
            {
                // 获取当前样式设置
                if (controller.GetStyler() != null)
                {
                    normalBgColor = controller.GetStyler().NormalBackgroundColor;
                    normalTextColor = controller.GetStyler().NormalTextColor;
                    selectedBgColor = controller.GetStyler().SelectedBackgroundColor;
                    selectedTextColor = controller.GetStyler().SelectedTextColor;
                }
                
                // 获取当前动画设置
                if (controller.GetAnimator() != null)
                {
                    selectedAnimationType = controller.GetAnimator().AnimationType;
                    animationDuration = controller.GetAnimator().Duration;
                }
            }
        }
        
        // 当前选中的标签页索引
        private int selectedTabIndex = 0;
        private readonly string[] tabTitles = new string[] { "基本设置", "标签和页面管理", "样式设置", "动画设置" };
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            TabViewController controller = (TabViewController)target;
            
            EditorGUILayout.Space();
            
            // 使用标签页形式展示不同设置
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabTitles);
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // 根据选中的标签页显示不同的设置面板
            switch (selectedTabIndex)
            {
                case 0: // 基本设置
                    DrawBasicSettings(controller);
                    break;
                    
                case 1: // 标签和页面管理
                    DrawTabAndPageManagement(controller);
                    break;
                    
                case 2: // 样式设置
                    DrawStyleSettings(controller);
                    break;
                    
                case 3: // 动画设置
                    DrawAnimationSettings(controller);
                    break;
            }
            
            EditorGUILayout.EndVertical();
            
            serializedObject.ApplyModifiedProperties();
        }
        
        // 绘制基本设置
        private void DrawBasicSettings(TabViewController controller)
        {
            EditorGUILayout.LabelField("基本设置", EditorStyles.boldLabel);
            
            // 模式选择
            EditorGUILayout.PropertyField(modeProperty, new GUIContent("模式"));
            
            EditorGUILayout.Space();
            
            // 当前状态信息
            EditorGUILayout.LabelField("当前状态", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("当前标签", string.IsNullOrEmpty(controller.CurrentTabId) ? "无" : controller.CurrentTabId);
            EditorGUILayout.LabelField("标签数量", controller.TabCount.ToString());
        }
        
        // 绘制标签和页面管理
        private void DrawTabAndPageManagement(TabViewController controller)
        {
            EditorGUILayout.LabelField("标签和页面管理", EditorStyles.boldLabel);
            
            // 显示现有标签页列表
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("现有标签页", EditorStyles.boldLabel);
            
            // 如果没有标签页，显示提示信息
            if (controller.TabCount == 0)
            {
                EditorGUILayout.HelpBox("当前没有标签页。请添加标签页。", MessageType.Info);
            }
            else
            {
                // 创建标签页按钮
                string[] tabIds = new string[controller.TabCount];
                int index = 0;
                int selectedIndex = -1;
                
                // 收集所有标签页ID
                foreach (var tabId in GetTabIds(controller))
                {
                    tabIds[index] = tabId;
                    if (tabId == controller.CurrentTabId)
                    {
                        selectedIndex = index;
                    }
                    index++;
                }
                
                // 显示标签页按钮
                EditorGUI.BeginChangeCheck();
                int newSelectedIndex = GUILayout.Toolbar(selectedIndex, tabIds);
                if (EditorGUI.EndChangeCheck() && newSelectedIndex != selectedIndex && newSelectedIndex >= 0 && newSelectedIndex < tabIds.Length)
                {
                    // 切换到选中的标签页
                    controller.SwitchToTab(tabIds[newSelectedIndex]);
                }
                
                // 显示当前选中标签页的详细信息
                if (selectedIndex >= 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    
                    string currentTabId = tabIds[selectedIndex];
                    EditorGUILayout.LabelField("当前标签页: " + currentTabId, EditorStyles.boldLabel);
                    
                    // 显示标签页按钮对象
                    TabItem currentTab = controller.GetTab(currentTabId);
                    if (currentTab != null)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField("标签按钮", currentTab.Button, typeof(GameObject), true);
                        EditorGUI.EndDisabledGroup();
                    }
                    
                    // 如果是页面切换模式，显示关联的页面
                    if (controller.Mode == TabViewMode.PageSwitch)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("关联页面", EditorStyles.boldLabel);
                        
                        // 显示页面对象字段（如果有）
                        GameObject pageContent = GetPageContent(controller, currentTabId);
                        if (pageContent != null)
                        {
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.ObjectField("页面内容", pageContent, typeof(GameObject), true);
                            EditorGUI.EndDisabledGroup();
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("当前标签页没有关联页面。", MessageType.Warning);
                            
                            // 允许为当前标签页添加页面
                            newPageContent = (GameObject)EditorGUILayout.ObjectField("页面内容", newPageContent, typeof(GameObject), true);
                            
                            if (GUILayout.Button("添加页面") && newPageContent != null)
                            {
                                controller.AddPage(currentTabId, newPageContent);
                                newPageContent = null;
                            }
                        }
                    }
                    
                    // 添加删除标签页按钮
                    EditorGUILayout.Space();
                    if (GUILayout.Button("删除此标签页", GUILayout.Height(30)))
                    {
                        if (EditorUtility.DisplayDialog("确认删除", $"确定要删除标签页 '{currentTabId}' 吗？", "删除", "取消"))
                        {
                            controller.RemoveTab(currentTabId);
                        }
                    }
                    
                    EditorGUILayout.EndVertical();
                }
            }
            
            // 添加新标签页
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("添加新标签页", EditorStyles.boldLabel);
            
            newTabId = EditorGUILayout.TextField("标签ID", newTabId);
            newTabButton = (GameObject)EditorGUILayout.ObjectField("标签按钮", newTabButton, typeof(GameObject), true);
            
            // 如果是页面切换模式，同时添加页面
            if (controller.Mode == TabViewMode.PageSwitch)
            {
                newPageContent = (GameObject)EditorGUILayout.ObjectField("页面内容", newPageContent, typeof(GameObject), true);
            }
            
            if (GUILayout.Button("添加标签页", GUILayout.Height(30)) && !string.IsNullOrEmpty(newTabId) && newTabButton != null)
            {
                // 添加标签页
                controller.AddTab(newTabId, newTabButton);
                
                // 如果是页面切换模式且提供了页面内容，则添加页面
                if (controller.Mode == TabViewMode.PageSwitch && newPageContent != null)
                {
                    controller.AddPage(newTabId, newPageContent);
                }
                
                // 切换到新添加的标签页
                controller.SwitchToTab(newTabId);
                
                // 清空输入字段
                newTabId = "";
                newTabButton = null;
                newPageContent = null;
            }
            
            EditorGUILayout.EndVertical();
        }
        
        // 获取所有标签页ID
        private string[] GetTabIds(TabViewController controller)
        {
            List<string> tabIds = new List<string>();
            for (int i = 0; i < controller.TabCount; i++)
            {
                string tabId = GetTabIdByIndex(controller, i);
                if (!string.IsNullOrEmpty(tabId))
                {
                    tabIds.Add(tabId);
                }
            }
            return tabIds.ToArray();
        }
        
        // 通过索引获取标签页ID
        private string GetTabIdByIndex(TabViewController controller, int index)
        {
            int currentIndex = 0;
            foreach (var tabId in GetAllTabIds(controller))
            {
                if (currentIndex == index)
                {
                    return tabId;
                }
                currentIndex++;
            }
            return null;
        }
        
        // 获取所有标签页ID（通过反射）
        private IEnumerable<string> GetAllTabIds(TabViewController controller)
        {
            // 使用反射获取私有字段tabs
            var tabsField = typeof(TabViewController).GetField("tabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (tabsField != null)
            {
                var tabs = tabsField.GetValue(controller) as System.Collections.Generic.Dictionary<string, TabItem>;
                if (tabs != null)
                {
                    return tabs.Keys;
                }
            }
            return new List<string>();
        }
        
        // 获取页面内容（通过反射）
        private GameObject GetPageContent(TabViewController controller, string pageId)
        {
            // 使用反射获取私有字段pages
            var pagesField = typeof(TabViewController).GetField("pages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (pagesField != null)
            {
                var pages = pagesField.GetValue(controller) as System.Collections.Generic.Dictionary<string, GameObject>;
                if (pages != null && pages.ContainsKey(pageId))
                {
                    return pages[pageId];
                }
            }
            return null;
        }
        
        // 绘制样式设置
        private void DrawStyleSettings(TabViewController controller)
        {
            EditorGUILayout.LabelField("样式设置", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(enableStylerProperty, new GUIContent("启用样式器"));
            
            EditorGUILayout.Space();
            
            if (enableStylerProperty.boolValue)
            {
                EditorGUILayout.LabelField("正常状态", EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                normalBgColor = EditorGUILayout.ColorField("背景颜色", normalBgColor);
                normalTextColor = EditorGUILayout.ColorField("文本颜色", normalTextColor);
                
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("选中状态", EditorStyles.boldLabel);
                selectedBgColor = EditorGUILayout.ColorField("背景颜色", selectedBgColor);
                selectedTextColor = EditorGUILayout.ColorField("文本颜色", selectedTextColor);
                
                if (EditorGUI.EndChangeCheck())
                {
                    TabViewStyler styler = new TabViewStyler();
                    styler.SetNormalStyle(normalBgColor, normalTextColor);
                    styler.SetSelectedStyle(selectedBgColor, selectedTextColor);
                    controller.SetStyler(styler);
                }
            }
        }
        
        // 绘制动画设置
        private void DrawAnimationSettings(TabViewController controller)
        {
            EditorGUILayout.LabelField("动画设置", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            selectedAnimationType = (AnimationType)EditorGUILayout.EnumPopup("动画类型", selectedAnimationType);
            animationDuration = EditorGUILayout.Slider("持续时间 (秒)", animationDuration, 0.1f, 2f);
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(controller, "Change TabView Animation");
                TabViewAnimator animator = new TabViewAnimator();
                animator.SetPageSwitchAnimation(selectedAnimationType, animationDuration);
                controller.SetAnimator(animator);
                EditorUtility.SetDirty(controller);
                PrefabUtility.RecordPrefabInstancePropertyModifications(controller);
            }
        }
    }
}
