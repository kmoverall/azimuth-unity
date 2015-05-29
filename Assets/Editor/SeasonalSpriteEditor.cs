using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
[CustomEditor(typeof(SeasonalSpriteRenderer))]
public class SeasonalSpriteEditor : Editor {
	SerializedProperty maskSprite;
	SerializedProperty springSprite;
	SerializedProperty summerSprite;
	SerializedProperty autumnSprite;
	SerializedProperty winterSprite;

	public void OnEnable() {
		SeasonalSpriteRenderer targetComponent = (SeasonalSpriteRenderer)target;
		targetComponent.render = targetComponent.GetComponent<SpriteRenderer>();
		
		//targetComponent.render.sharedMaterial = Resources.Load<Material>("SeasonSprite");
		//targetComponent.render.sharedMaterial.hideFlags = HideFlags.HideInInspector;
		//targetComponent.render.hideFlags = HideFlags.HideInInspector;

		maskSprite = serializedObject.FindProperty("maskSprite");
		springSprite = serializedObject.FindProperty("springSprite");
		summerSprite = serializedObject.FindProperty("summerSprite");
		autumnSprite = serializedObject.FindProperty("autumnSprite");
		winterSprite = serializedObject.FindProperty("winterSprite");

		targetComponent.render.sprite = targetComponent.maskSprite;
		targetComponent.UpdatePropertyBlock();

		serializedObject.ApplyModifiedProperties();
	}

	public override void OnInspectorGUI() {
		SeasonalSpriteRenderer targetComponent = (SeasonalSpriteRenderer)target;

		EditorGUILayout.PropertyField(maskSprite, new GUIContent("Sprite Mask"));
		EditorGUILayout.PropertyField(springSprite, new GUIContent("Spring Texture"));
		EditorGUILayout.PropertyField(summerSprite, new GUIContent("Summer Texture"));
		EditorGUILayout.PropertyField(autumnSprite, new GUIContent("Autumn Texture"));
		EditorGUILayout.PropertyField(winterSprite, new GUIContent("Winter Texture"));

		targetComponent.render.sprite = targetComponent.maskSprite;
		//targetComponent.UpdatePropertyBlock();

		serializedObject.ApplyModifiedProperties();

		if (GUI.changed)
			EditorUtility.SetDirty(targetComponent);
	}

	public void Update() {

	}
}
