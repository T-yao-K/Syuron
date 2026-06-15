using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[InitializeOnLoad]
public static class AetherInspectorRefresh
{
    private const string ShaderName = "ACM/Aether";

    static AetherInspectorRefresh()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredEditMode)
            return;

        Object active = Selection.activeObject;
        if (!IsAetherMaterial(active))
            return;

        EditorApplication.delayCall += () =>
        {
            if (!IsAetherMaterial(active) || Selection.activeObject != active)
                return;

            Selection.activeObject = null;
            InternalEditorUtility.RepaintAllViews();
            EditorApplication.delayCall += () =>
            {
                if (Selection.activeObject != null)
                    return;

                Selection.activeObject = active;
                InternalEditorUtility.RepaintAllViews();
            };
        };
    }

    private static bool IsAetherMaterial(Object obj)
    {
        Material material = obj as Material;
        return material != null
            && material.shader != null
            && material.shader.name == ShaderName;
    }
}
