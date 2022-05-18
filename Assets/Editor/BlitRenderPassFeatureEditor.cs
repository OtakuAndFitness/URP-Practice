using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BlitRenderPassFeature.BlitSettings))]
public class BlitRenderPassFeatureEditor : PropertyDrawer {

	private bool createdStyles = false;
	private GUIStyle boldLabel;

	private void CreateStyles() {
		createdStyles = true;
		boldLabel = GUI.skin.label;
		boldLabel.fontStyle = FontStyle.Bold;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		//base.OnGUI(position, property, label);
		if (!createdStyles) CreateStyles();

		// Blit Settings
		EditorGUI.BeginProperty(position, label, property);
		EditorGUI.LabelField(position, "Blit Settings", boldLabel);
		SerializedProperty _event = property.FindPropertyRelative("PassEvent");
		EditorGUILayout.PropertyField(_event);

		EditorGUILayout.PropertyField(property.FindPropertyRelative("blitMaterial"));
		EditorGUILayout.PropertyField(property.FindPropertyRelative("blitMaterialPassIndex"));
		EditorGUILayout.PropertyField(property.FindPropertyRelative("setInverseViewMatrix"));
		EditorGUILayout.PropertyField(property.FindPropertyRelative("requireDepthNormals"));

		// Source
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Source", boldLabel);
		SerializedProperty srcType = property.FindPropertyRelative("srcType");
		EditorGUILayout.PropertyField(srcType);
		int enumValue = srcType.intValue;
		if (enumValue == (int)BlitRenderPassFeature.Target.TextureID) {
			EditorGUILayout.PropertyField(property.FindPropertyRelative("srcTextureId"));
		} else if (enumValue == (int)BlitRenderPassFeature.Target.RenderTextureObject) {
			EditorGUILayout.PropertyField(property.FindPropertyRelative("srcTextureObject"));
		}

		// Destination
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Destination", boldLabel);
		SerializedProperty dstType = property.FindPropertyRelative("dstType");
		EditorGUILayout.PropertyField(dstType);
		enumValue = dstType.intValue;
		if (enumValue == (int)BlitRenderPassFeature.Target.TextureID) {
			EditorGUILayout.PropertyField(property.FindPropertyRelative("dstTextureId"));

			SerializedProperty overrideGraphicsFormat = property.FindPropertyRelative("overrideGraphicsFormat");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(overrideGraphicsFormat);
			if (overrideGraphicsFormat.boolValue) {
				EditorGUILayout.PropertyField(property.FindPropertyRelative("graphicsFormat"), GUIContent.none);
			}
			EditorGUILayout.EndHorizontal();
		} else if (enumValue == (int)BlitRenderPassFeature.Target.RenderTextureObject) {
			EditorGUILayout.PropertyField(property.FindPropertyRelative("dstTextureObject"));
		}

		EditorGUI.indentLevel = 1;
		EditorGUI.EndProperty();
		property.serializedObject.ApplyModifiedProperties();
	}
}
