using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UGC.Tabview
{
    /// <summary>
    /// 表示单个标签页的类
    /// </summary>
    [Serializable]
    public class TabItem
    {
        /// <summary>
        /// 标签页ID
        /// </summary>
        public string Id { get; private set; }
        
        /// <summary>
        /// 标签页按钮对象
        /// </summary>
        public GameObject Button { get; private set; }
        
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected { get; private set; }
        
        /// <summary>
        /// 按钮组件
        /// </summary>
        private Button buttonComponent;
        
        /// <summary>
        /// 文本组件
        /// </summary>
        private Text textComponent;

        /// <summary>
        /// TMP文本组件
        /// </summary>
        private TMP_Text tmpTextComponent;
        
        /// <summary>
        /// 图片组件
        /// </summary>
        private Image imageComponent;
        
        /// <summary>
        /// 创建标签页
        /// </summary>
        /// <param name="id">标签页ID</param>
        /// <param name="button">标签页按钮对象</param>
        public TabItem(string id, GameObject button)
        {
            Id = id;
            Button = button;
            IsSelected = false;
            
            // 获取组件
            buttonComponent = button.GetComponent<Button>();
            textComponent = button.GetComponentInChildren<Text>();
            tmpTextComponent = button.GetComponentInChildren<TMP_Text>();
            imageComponent = button.GetComponent<Image>();
            
            if (buttonComponent == null)
            {
                Debug.LogWarning($"TabItem {id}: Button component not found on {button.name}");
            }
        }
        
        /// <summary>
        /// 选中标签页
        /// </summary>
        public void Select()
        {
            IsSelected = true;
            
            // 可以在这里添加选中状态的视觉反馈
            if (buttonComponent != null)
            {
                // 禁用按钮交互，因为当前已选中
                buttonComponent.interactable = false;
            }
        }
        
        /// <summary>
        /// 取消选中标签页
        /// </summary>
        public void Deselect()
        {
            IsSelected = false;
            
            // 可以在这里添加未选中状态的视觉反馈
            if (buttonComponent != null)
            {
                // 启用按钮交互
                buttonComponent.interactable = true;
            }
        }
        
        /// <summary>
        /// 设置按钮点击事件
        /// </summary>
        /// <param name="onClick">点击事件回调</param>
        public void SetOnClickListener(Action onClick)
        {
            if (buttonComponent != null)
            {
                buttonComponent.onClick.RemoveAllListeners();
                buttonComponent.onClick.AddListener(() => onClick?.Invoke());
            }
        }
        
        /// <summary>
        /// 应用样式
        /// </summary>
        /// <param name="backgroundColor">背景颜色</param>
        /// <param name="textColor">文本颜色</param>
        public void ApplyStyle(Color backgroundColor, Color textColor)
        {
            if (imageComponent != null)
            {
                imageComponent.color = backgroundColor;
            }
            
            if (textComponent != null)
            {
                textComponent.color = textColor;
            }

            if (tmpTextComponent != null)
            {
                tmpTextComponent.color = textColor;
            }
        }
    }
}