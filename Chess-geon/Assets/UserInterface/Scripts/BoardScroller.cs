using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DaburuTools.Action;
using DaburuTools;

public class BoardScroller : MonoBehaviour
{
	private static BoardScroller sInstance = null;
	public static BoardScroller Instance { get { return sInstance; } }

	private void OnDestroy()
	{
		if (this == sInstance)
			sInstance = null;
	}

	private float mfScrollSensitivity;
	// The following three floats are in pixels.
	private float mfDesignWidth = 1080.0f;
	private float mfDesignHeight = 1920.0f;
	private float mfFrameBorderWidth = 12.0f;
	private Vector2 mvec2MinPos, mvec2MaxPos;
	Transform camTransform;

	// For offseting the "center" point when focusing camera.
	private float mfCenterYOffset;

	// The following values are for MiniMap calculations.
	private float Xscaler;
	private float Yscaler;

	void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			Setup();
		}
		else if (sInstance != this)
		{
			Destroy(this.gameObject);
			return;
		}
	}

	private void Setup()
	{
		camTransform = Camera.main.transform;

		mfScrollSensitivity = mfDesignHeight / Screen.height / 100.0f;
		mvec2MinPos = mvec2MaxPos = Vector2.zero;
		mvec2MaxPos.x = DungeonManager.Instance.SizeX * DungeonManager.Instance.BlockSize;
		mvec2MaxPos.y = DungeonManager.Instance.SizeY * DungeonManager.Instance.BlockSize;
		mvec2MaxPos.x -= (mfDesignWidth - mfFrameBorderWidth * 2.0f) / 100.0f;
		mvec2MaxPos.y -= (mfDesignWidth - mfFrameBorderWidth * 2.0f) / 100.0f;

		// MiniMap
		Xscaler = 1.0f / mfDesignWidth * 100.0f;

		// Getting ratio of design width : actual width scaled to design ratio.
		// In other words, getting the ratio of design resolution,
		// to the actual width scaled to design size.
		float ratioAdjuster = mfDesignWidth / (Screen.width * mfDesignHeight / Screen.height);

		Xscaler *= ratioAdjuster;

		Yscaler = 1.0f / mfDesignHeight * 100.0f;

		float trueCenter = mfDesignHeight / 2.0f;
		float boardCenter = mfDesignHeight - (mfDesignWidth / 2.0f);
		mfCenterYOffset = (boardCenter - trueCenter) / 100.0f;
	}

	void Start()
	{
		SetupMiniMapValues();
	}

	private void ConstrainCamera()
	{
		camTransform.position = ConstrainVectorToCameraBounds(camTransform.position);
	}

	public Vector3 ConstrainVectorToCameraBounds(Vector3 _vec3)
	{
		_vec3.y -= mfCenterYOffset;

		_vec3.x = Mathf.Clamp(_vec3.x, mvec2MinPos.x, mvec2MaxPos.x);
		_vec3.y = Mathf.Clamp(_vec3.y, mvec2MinPos.y, mvec2MaxPos.y);
		_vec3.z = camTransform.position.z;

		return _vec3;
	}

	public void FocusCameraToPos(Vector3 _focalPos, float _focusDuration, Graph _focusGraph)
	{
		_focalPos = ConstrainVectorToCameraBounds(_focalPos);
		MoveToAction focusToPos = new MoveToAction(camTransform, _focusGraph, _focalPos, _focusDuration);
		ActionHandler.RunAction(focusToPos);
	}

	public void OnDrag(BaseEventData _eventData)
	{
		PointerEventData _pointerData = (PointerEventData) _eventData;
		Vector3 offset = new Vector3(-_pointerData.delta.x, -_pointerData.delta.y, 0.0f);
		offset *= mfScrollSensitivity;
		camTransform.position += offset;

		ConstrainCamera();
		UpdateMiniMap();
	}

	#region MiniMap
	private void SetupMiniMapValues()
	{
		float BGWidth = DungeonManager.Instance.SizeX * DungeonManager.Instance.BlockSize * Xscaler;
		float BGHeight = DungeonManager.Instance.SizeY * DungeonManager.Instance.BlockSize * Yscaler;
		float CamWidth = (mfDesignWidth - (mfFrameBorderWidth * 2.0f)) * Xscaler / 100.0f;
		float CamHeight = (mfDesignWidth - (mfFrameBorderWidth * 2.0f)) * Yscaler / 100.0f;
		CameraGLDrawer.Instance.SetMiniMapValues(BGWidth, BGHeight, CamWidth, CamHeight);

		float actualWidthScaled = Screen.width * mfDesignHeight / Screen.height;
		float halfFrame = (mfDesignWidth / 2.0f - (mfFrameBorderWidth * 2.0f)) / actualWidthScaled;
		float anchorX = 0.5f - halfFrame;
		float anchorY = (mfDesignHeight - mfDesignWidth + mfFrameBorderWidth * 2.0f) / mfDesignHeight;
		CameraGLDrawer.Instance.SetMiniMapAnchor(anchorX, anchorY);
	}

	private void UpdateMiniMap()
	{
		Vector2 CamOffset = camTransform.position;
		CamOffset.x -= mvec2MinPos.x;
		CamOffset.x *= Xscaler;
		CamOffset.y -= mvec2MinPos.y;
		CamOffset.y *= Yscaler;

		CameraGLDrawer.Instance.UpdateMiniMap(CamOffset);
	}
	#endregion
}
