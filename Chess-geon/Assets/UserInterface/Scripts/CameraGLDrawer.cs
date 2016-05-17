using UnityEngine;
using System.Collections;

public class CameraGLDrawer : MonoBehaviour
{
	private static CameraGLDrawer sInstance = null;
	public static CameraGLDrawer Instance { get { return sInstance; } }

	void OnDestroy()
	{
		// Only set sInstance to null if the actual instance is destroyed.
		if (sInstance == this)
			sInstance = null;
	}

	private void Awake()
	{
		if (sInstance == null)
			sInstance = this;
		else if (sInstance != this)
		{
			Destroy(this.gameObject);
			return;
		}
	}



	#region GL Minimap
	// For GL MiniMap
	static Material lineMaterial;
	public Color MiniMapBGCol, MiniMapCamAreaCol;

	// Ratio of screen.
	private float mfScaleRatio = 0.05f;
	private float mfMiniMapBGAreaWidth;
	private float mfMiniMapBGAreaHeight;
	private float mfMiniMapCamAreaWidth;
	private float mfMiniMapCamAreaHeight;
	private float mfMiniMapAnchorX, mfMiniMapAnchorY;
	private Vector2 mVec2MiniMapCamAnchorOffset;

	public void SetMiniMapAnchor(float _anchorX, float _anchorY)
	{
		mfMiniMapAnchorX = _anchorX;
		mfMiniMapAnchorY = _anchorY;
	}

	// Values passed in should be in (Unity Unit * 100.0f) / Screen DesignWidth or DesignHeight.
	// 100.0f is because we want the pixel unit.
	public void SetMiniMapValues(float _BGWidth, float _BGHeight, float _CamWidth, float _CamHeight)
	{
		mfMiniMapBGAreaWidth = _BGWidth * mfScaleRatio;
		mfMiniMapBGAreaHeight = _BGHeight * mfScaleRatio;
		mfMiniMapCamAreaWidth = _CamWidth * mfScaleRatio;
		mfMiniMapCamAreaHeight = _CamHeight * mfScaleRatio;
	}

	// Values passed in should be in (Unity Unit * 100.0f) / Screen DesignWidth or DesignHeight.
	// 100.0f is because we want the pixel unit.
	public void UpdateMiniMap(Vector2 _CamOffset)
	{
		mVec2MiniMapCamAnchorOffset = _CamOffset * mfScaleRatio;
	}

	private static void CreateLineMaterial() {
		if (!lineMaterial)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			var shader = Shader.Find ("Hidden/Internal-Colored");
			lineMaterial = new Material (shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			lineMaterial.SetInt ("_ZWrite", 0);
		}
	}

	void OnPostRender() {
		CreateLineMaterial();
		lineMaterial.SetPass(0);

		GL.PushMatrix();
		GL.LoadOrtho();

		GL.Begin(GL.QUADS);

		GL.Color(MiniMapBGCol);
		// Btm-Left
		Vector3 vert = new Vector3(mfMiniMapAnchorX, mfMiniMapAnchorY);
		GL.Vertex(vert);
		// Top-Left
		vert.y += mfMiniMapBGAreaHeight;
		GL.Vertex(vert);
		// Top-Right
		vert.x += mfMiniMapBGAreaWidth;
		GL.Vertex(vert);
		// Btm-Right
		vert.y -= mfMiniMapBGAreaHeight;
		GL.Vertex(vert);

		GL.Color(MiniMapCamAreaCol);
		// Btm-Left
		vert = new Vector3(mfMiniMapAnchorX + mVec2MiniMapCamAnchorOffset.x, mfMiniMapAnchorY + mVec2MiniMapCamAnchorOffset.y);
		GL.Vertex(vert);
		// Top-Left
		vert.y += mfMiniMapCamAreaHeight;
		GL.Vertex(vert);
		// Top-Right
		vert.x += mfMiniMapCamAreaWidth;
		GL.Vertex(vert);
		// Btm-Right
		vert.y -= mfMiniMapCamAreaHeight;
		GL.Vertex(vert);

		GL.End();

		GL.PopMatrix();
	}
	#endregion
}
