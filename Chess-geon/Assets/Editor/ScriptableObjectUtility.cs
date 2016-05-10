using UnityEngine;
using UnityEditor;

public class ScriptableObjectUtility
{
	public static void CreateAsset<T>() where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();
		ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
	}

	#region MenuItems
	[MenuItem("Assets/Create/Chessgeon/RoomPattern")]
	public static void CreateRoomPattern()
	{
		ScriptableObjectUtility.CreateAsset<RoomPattern>();
	}
	#endregion
}
