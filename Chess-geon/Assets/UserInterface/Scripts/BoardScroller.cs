﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class BoardScroller : MonoBehaviour
{
	private float mfScrollSensitivity;
	// The following three floats are in pixels.
	private float mfDesignWidth = 1080.0f;
	private float mfDesignHeight = 1920.0f;
	private float mfFrameBorderWidth = 12.0f;
	private Vector2 mvec2MinPos, mvec2MaxPos;
	Transform camTransform;

	void Awake()
	{
		camTransform = Camera.main.transform;

		mfScrollSensitivity = mfDesignHeight / Screen.height / 100.0f;
		mvec2MinPos = mvec2MaxPos = Vector2.zero;
		mvec2MaxPos.x = DungeonManager.Instance.SizeX * DungeonManager.Instance.BlockSize;
		mvec2MaxPos.y = DungeonManager.Instance.SizeY * DungeonManager.Instance.BlockSize;
		mvec2MaxPos.x -= (mfDesignWidth - mfFrameBorderWidth * 2.0f) / 100.0f;
		mvec2MaxPos.y -= (mfDesignWidth - mfFrameBorderWidth * 2.0f) / 100.0f;
	}

	void Start()
	{
		SendMiniMapValues();
		float anchorX = (mfFrameBorderWidth * 2.0f) / mfDesignWidth;
		float anchorY = (mfDesignHeight - mfDesignWidth + mfFrameBorderWidth * 2.0f) / mfDesignHeight;
		CameraGLDrawer.Instance.SetMiniMapAnchor(anchorX, anchorY);
	}

	private void ConstrainCamera()
	{
		Vector3 camPos = camTransform.position;
		camPos.x = Mathf.Clamp(camPos.x, mvec2MinPos.x, mvec2MaxPos.x);
		camPos.y = Mathf.Clamp(camPos.y, mvec2MinPos.y, mvec2MaxPos.y);

		camTransform.position = camPos;
	}

	public void OnDrag(BaseEventData _eventData)
	{
		PointerEventData _pointerData = (PointerEventData) _eventData;
		Vector3 offset = new Vector3(-_pointerData.delta.x, -_pointerData.delta.y, 0.0f);
		offset *= mfScrollSensitivity;
		camTransform.position += offset;

		ConstrainCamera();
		SendMiniMapValues();
	}

	private void SendMiniMapValues()
	{
		float Xscaler = 1.0f / mfDesignWidth * 100.0f;
		float Yscaler = 1.0f / mfDesignHeight * 100.0f;

		float BGWidth = DungeonManager.Instance.SizeX * DungeonManager.Instance.BlockSize * Xscaler;
		float BGHeight = DungeonManager.Instance.SizeY * DungeonManager.Instance.BlockSize * Yscaler;
		float CamWidth = (mfDesignWidth - (mfFrameBorderWidth * 2.0f)) / 100.0f * Xscaler;
		float CamHeight = (mfDesignWidth - (mfFrameBorderWidth * 2.0f)) / 100.0f * Yscaler;
		Vector2 CamOffset = camTransform.position;
		CamOffset.x -= mvec2MinPos.x;
		CamOffset.x *= Xscaler;
		CamOffset.y -= mvec2MinPos.y;
		CamOffset.y *= Yscaler;

		CameraGLDrawer.Instance.SetMiniMapValues(BGWidth, BGHeight, CamWidth, CamHeight, CamOffset);
	}
}
