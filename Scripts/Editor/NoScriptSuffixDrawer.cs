
using UnityEditor;
using UnityEngine;

namespace UGC.Tabview.Editor
{
    [CustomPropertyDrawer(typeof(NoScriptSuffixAttribute))]
    public class NoScriptSuffixDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 移除"(Script)"并重写标签
            string customLabel = label.text.Replace(" (Script)", "");
            EditorGUI.PropertyField(position, property, new GUIContent(customLabel));
        }
    }
}