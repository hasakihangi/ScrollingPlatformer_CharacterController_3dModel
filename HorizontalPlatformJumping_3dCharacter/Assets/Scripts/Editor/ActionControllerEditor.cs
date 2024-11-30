using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

// [CustomEditor(typeof(ActionController))]
public class ActionControllerEditor: Editor
{
	private BoxBoundsHandle handle = new BoxBoundsHandle();

	protected virtual void OnSceneGUI()
	{
		ActionController controller = target as ActionController;
		handle.center = controller.attackBox.center;
		handle.size = controller.attackBox.size;
		handle.wireframeColor = new Color(0.8f, 0.4f, 0.2f);
		handle.handleColor = new Color(0.2f, 0.5f, 0.6f);
		EditorGUI.BeginChangeCheck();
		handle.DrawHandle();
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(controller, "Change Rect");
			Bounds bounds = new Bounds();
			bounds.center = handle.center;
			bounds.size = handle.size;
			controller.attackBox = bounds;
		}
	}
}
