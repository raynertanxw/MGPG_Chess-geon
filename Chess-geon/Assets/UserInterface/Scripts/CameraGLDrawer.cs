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

		SetUp();
	}

	private void SetUp()
	{
		MiniMapSetUp();
	}


	#region GL Minimap
	// For GL MiniMap
	static Material lineMaterial;

	// Ratio of screen.
	private float mfScaleRatio = 0.05f;
	private float mfMiniMapBGAreaWidth;
	private float mfMiniMapBGAreaHeight;
	private float mfMiniMapCamAreaWidth;
	private float mfMiniMapCamAreaHeight;
	private float mfMiniMapAnchorX, mfMiniMapAnchorY;
	private Vector2 mVec2MiniMapCamAnchorOffset;

	private void MiniMapSetUp()
	{
		mfMiniMapAnchorX = 0.05f;
		mfMiniMapAnchorY = 0.5f;
	}

	public void SetMiniMapAnchor(float _anchorX, float _anchorY)
	{
		mfMiniMapAnchorX = _anchorX;
		mfMiniMapAnchorY = _anchorY;
	}

	public void SetMiniMapValues(float _BGWidth, float _BGHeight, float _CamWidth, float _CamHeight, Vector2 _CamOffset)
	{
		mfMiniMapBGAreaWidth = _BGWidth * mfScaleRatio;
		mfMiniMapBGAreaHeight = _BGHeight * mfScaleRatio;
		mfMiniMapCamAreaWidth = _CamWidth * mfScaleRatio;
		mfMiniMapCamAreaHeight = _CamHeight * mfScaleRatio;
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

		GL.Color(Color.blue);
		GL.Vertex(new Vector3(mfMiniMapAnchorX, mfMiniMapAnchorY, 0.0f));
		GL.Vertex(new Vector3(mfMiniMapAnchorX, mfMiniMapAnchorY + mfMiniMapBGAreaHeight, 0.0f));
		GL.Vertex(new Vector3(mfMiniMapAnchorX + mfMiniMapBGAreaWidth, mfMiniMapAnchorY + mfMiniMapBGAreaHeight, 0.0f));
		GL.Vertex(new Vector3(mfMiniMapAnchorX + mfMiniMapBGAreaWidth, mfMiniMapAnchorY, 0.0f));

		GL.Color(Color.red);
		float camAnchorX = mfMiniMapAnchorX + mVec2MiniMapCamAnchorOffset.x;
		float camAnchorY = mfMiniMapAnchorY + mVec2MiniMapCamAnchorOffset.y;
		GL.Vertex(new Vector3(camAnchorX, camAnchorY, 0.0f));
		GL.Vertex(new Vector3(camAnchorX, camAnchorY + mfMiniMapCamAreaHeight, 0.0f));
		GL.Vertex(new Vector3(camAnchorX + mfMiniMapCamAreaWidth, camAnchorY + mfMiniMapCamAreaHeight, 0.0f));
		GL.Vertex(new Vector3(camAnchorX + mfMiniMapCamAreaWidth, camAnchorY, 0.0f));

		GL.End();


		GL.Begin(GL.LINES);

		GL.Color(Color.green);
		GL.Vertex(new Vector3(0.5f, 0.5f, 0.0f));
//		GL.Vertex(new Vector3(0.0f, Input.mousePosition.y / Screen.height, 0.0f));
		GL.Vertex(new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0.0f));
//		GL.Vertex(new Vector3(Input.mousePosition.x / Screen.width, 0.0f, 0.0f));

		GL.End();

		GL.PopMatrix();
	}
	#endregion
}
