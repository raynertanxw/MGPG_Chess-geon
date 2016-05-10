using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoomPattern))]
[CanEditMultipleObjects]
public class RoomPatternEditor : Editor
{
	private SerializedProperty mSP_RoomSizeX;
	private SerializedProperty mSP_RoomSizeY;
	private SerializedProperty mSP_BlockTerrainType;

	private void OnEnable()
	{
		mSP_RoomSizeX			= serializedObject.FindProperty("RoomSizeX");
		mSP_RoomSizeY			= serializedObject.FindProperty("RoomSizeY");
		mSP_BlockTerrainType	= serializedObject.FindProperty("BlockTerrainType");

		int arrSize = mSP_RoomSizeX.intValue * mSP_RoomSizeY.intValue;

		if (mSP_BlockTerrainType.arraySize != arrSize)
		{
			mSP_BlockTerrainType.arraySize = arrSize;
		}

		serializedObject.ApplyModifiedProperties();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(mSP_RoomSizeX);
		EditorGUILayout.PropertyField(mSP_RoomSizeY);
		if (EditorGUI.EndChangeCheck())
		{
			mSP_BlockTerrainType.arraySize = mSP_RoomSizeX.intValue * mSP_RoomSizeY.intValue;
		}

		serializedObject.ApplyModifiedProperties();

		EditorGUILayout.Space();

		serializedObject.Update();

		for (int y = mSP_RoomSizeY.intValue - 1; y > -1; y--)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			for (int x = 0; x < mSP_RoomSizeX.intValue; x++)
			{
				int index = y * mSP_RoomSizeY.intValue + x;
				bool toggleVal = false;
				bool inspectorVal;
				switch (mSP_BlockTerrainType.GetArrayElementAtIndex(index).enumValueIndex)
				{
				case 0:
					toggleVal = false;
					break;
				case 1:
					toggleVal = true;
					break;
				}

				EditorGUI.BeginChangeCheck();
				inspectorVal = EditorGUILayout.Toggle(toggleVal, GUILayout.MaxWidth(20.0f));
				if (EditorGUI.EndChangeCheck())
				{
					if (inspectorVal)
						mSP_BlockTerrainType.GetArrayElementAtIndex(index).enumValueIndex = 1;
					else
						mSP_BlockTerrainType.GetArrayElementAtIndex(index).enumValueIndex = 0;
				}
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		serializedObject.ApplyModifiedProperties();
	}
}
