#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEditor;

public class DeviatableDrawer : OdinValueDrawer<Deviatable>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        var entry = this.ValueEntry;
        var val = entry.SmartValue;

        var state = this.Property.State;

        SirenixEditorGUI.BeginBox();
        SirenixEditorGUI.BeginBoxHeader();

        // HEADER
        GUILayout.BeginHorizontal();

        // Foldout + label
        state.Expanded = SirenixEditorGUI.Foldout(state.Expanded, label);

        // Small spacing so it doesn’t touch the text
        GUILayout.Space(6);

        // Toggle immediately after title (your request)
        val.deviate = GUILayout.Toggle(val.deviate, "Deviate", EditorStyles.toggle, GUILayout.Width(80));

        GUILayout.FlexibleSpace(); // keeps the rest clean

        GUILayout.EndHorizontal();

        SirenixEditorGUI.EndBoxHeader();

        // BODY
        if (state.Expanded)
        {
            EditorGUI.indentLevel++;

            if (!val.deviate)
                val.floatvalue = EditorGUILayout.FloatField("Value", val.floatvalue);
            else
                val.Vec2value = EditorGUILayout.Vector2Field("Range", val.Vec2value);

            EditorGUI.indentLevel--;
        }

        SirenixEditorGUI.EndBox();

        entry.SmartValue = val;
    }
}
#endif
