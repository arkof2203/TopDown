using UnityEngine;

namespace Netologia
{
    /// <summary>
    /// Атрибут, для блокирования модификации сериализуемых полей через инспектор
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public enum ReadOnlyMode : byte
        {
            Always,
            Runtime,
            Editor
        }

        public readonly ReadOnlyMode Mode;

        public ReadOnlyAttribute(ReadOnlyMode mode)
            => Mode = mode;

        public ReadOnlyAttribute() : this(ReadOnlyMode.Always)
        {
        }
    }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (attribute is not ReadOnlyAttribute attr) return;
            switch (attr.Mode)
            {
                case ReadOnlyAttribute.ReadOnlyMode.Always:
                    GUI.enabled = false;
                    break;
                case ReadOnlyAttribute.ReadOnlyMode.Runtime:
                    if (Application.isPlaying) GUI.enabled = false;
                    break;
                case ReadOnlyAttribute.ReadOnlyMode.Editor:
                    if (!Application.isPlaying) GUI.enabled = false;
                    break;
            }

            UnityEditor.EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
            => UnityEditor.EditorGUI.GetPropertyHeight(property, label, true);
    }

#endif
}