using UnityEngine;
using UnityEditor;
using MyGame.Ultilities;

[CustomEditor(typeof(RecipeData))]
public class RecipeScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RecipeData recipeScriptableObject = (RecipeData)target;
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("OUTPUT", new GUIStyle { fontStyle = FontStyle.Bold });
        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical();

        Texture2D[] textures = new Texture2D[10];
        if (recipeScriptableObject.outputItem != null)
        {
            textures[9] = AssetPreview.GetAssetPreview(recipeScriptableObject.outputItem.icon);
        }

        GUILayout.Box(textures[9], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outputItem"), GUIContent.none, true, GUILayout.Width(150));

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("QUANTITY", new GUIStyle { fontStyle = FontStyle.Bold });
        EditorGUILayout.PropertyField(serializedObject.FindProperty("quantityItemOutput"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();




        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        GUILayout.Label("RECIPE", new GUIStyle { fontStyle = FontStyle.Bold });

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        if (recipeScriptableObject.item00 != null)
        {
            textures[0] = AssetPreview.GetAssetPreview(recipeScriptableObject.item00.icon);
        }
        GUILayout.Box(textures[0], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item00"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (recipeScriptableObject.item10 != null)
        {
            textures[1] = AssetPreview.GetAssetPreview(recipeScriptableObject.item10.icon);
        }
        GUILayout.Box(textures[1], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item10"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (recipeScriptableObject.item20 != null)
        {
            textures[2] = AssetPreview.GetAssetPreview(recipeScriptableObject.item20.icon);
        }
        GUILayout.Box(textures[2], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item20"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();






        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        if (recipeScriptableObject.item01 != null)
        {
            textures[3] = AssetPreview.GetAssetPreview(recipeScriptableObject.item01.icon);
        }
        GUILayout.Box(textures[3], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item01"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (recipeScriptableObject.item11 != null)
        {
            textures[4] = AssetPreview.GetAssetPreview(recipeScriptableObject.item11.icon);
        }
        GUILayout.Box(textures[4], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item11"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (recipeScriptableObject.item21 != null)
        {
            textures[5] = AssetPreview.GetAssetPreview(recipeScriptableObject.item21.icon);
        }
        GUILayout.Box(textures[5], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item21"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();





        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        if (recipeScriptableObject.item02 != null)
        {
            textures[6] = AssetPreview.GetAssetPreview(recipeScriptableObject.item02.icon);
        }
        GUILayout.Box(textures[6], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item02"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        if (recipeScriptableObject.item12 != null)
        {
            textures[7] = AssetPreview.GetAssetPreview(recipeScriptableObject.item12.icon);
        }
        GUILayout.Box(textures[7], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item12"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();

        if (recipeScriptableObject.item22 != null)
        {
            textures[8] = AssetPreview.GetAssetPreview(recipeScriptableObject.item22.icon);
        }
        GUILayout.Box(textures[8], GUILayout.Width(150), GUILayout.Height(150));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item22"), GUIContent.none, true, GUILayout.Width(150));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();



        serializedObject.ApplyModifiedProperties();

        // Scale Texuture if changed
        if (EditorGUI.EndChangeCheck())
        {
            //Debug.Log("Change at crafting grid");
            ResizeTextures(textures, 140, 140);
        }
    }

    private void ResizeTextures(Texture2D[] textures, int width, int height)
    {
        foreach (var texture in textures)
        {
            if (texture != null)
            {
                TextureScale.Point(texture, width, height);
            }
        }
    }
}


