using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mygame.Editor
{
    [CustomEditor(typeof(AxeData))]
    public class ItemDataEditor : UnityEditor.Editor
    {
        private Texture2D texture;

        public override void OnInspectorGUI()
        {
            ItemData itemData = (ItemData)target;
        
            if (itemData.icon != null)
            {
                texture = AssetPreview.GetAssetPreview(itemData.icon);        
            }

            GUILayout.Box(texture, GUILayout.Width(75), GUILayout.Height(75));
            
            // Draw rest of the inspector
            DrawDefaultInspector();
        }
    }
}