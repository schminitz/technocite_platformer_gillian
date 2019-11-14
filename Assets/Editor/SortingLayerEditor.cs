using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.Reflection;

[CustomEditor(typeof(SortingLayer))]
[CanEditMultipleObjects]
public class SortingLayerEditor : Editor
{
	SerializedProperty sortingLayer;
	SerializedProperty sortingOrder;

	private MeshRenderer meshRenderer;

	string[] options;
	int[] picks;

	void OnEnable()
	{
		GameObject go = Selection.activeGameObject;
		if (go != null)
		{
			meshRenderer = go.GetComponent<MeshRenderer>();
		}

		sortingLayer = serializedObject.FindProperty("sortingLayer");
		sortingOrder = serializedObject.FindProperty("sortingOrder");

		PrepareOptions();
	}

	void PrepareOptions()
	{
		options = GetSortingLayerNames();
		picks = new int[options.Length];
		for(int i = 0; i < options.Length; i++)
		{
			picks[i] = i;
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUI.BeginChangeCheck();

		sortingLayer.intValue = EditorGUILayout.IntPopup("Sorting Layer", sortingLayer.intValue, options, picks);
		sortingOrder.intValue = EditorGUILayout.IntField("Sorting Order", sortingOrder.intValue);

		if(EditorGUI.EndChangeCheck())
			UpdateLayer();

		serializedObject.ApplyModifiedProperties();
	}

	void UpdateLayer()
	{
		meshRenderer.sortingLayerName = options[sortingLayer.intValue];
		meshRenderer.sortingOrder = sortingOrder.intValue;
	}

	public string[] GetSortingLayerNames()
	{
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}
}
