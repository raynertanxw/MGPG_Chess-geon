﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EnemyType { Black, Stone, Slime, Glass, Gold, Cursed };

public class EnemyPiece : MonoBehaviour
{
	#region Object Pooling
	private static List<EnemyPiece> enemyPool = null;
	private static Sprite[] enemySprites = null;
	private static string[] enemySpriteNames = null;

	private void Awake()
	{
		if (enemyPool == null)
		{
			enemyPool = new List<EnemyPiece>();

			enemySprites = Resources.LoadAll<Sprite>("Sprites/chessgeon_pieces");
			enemySpriteNames = new string[enemySprites.Length];

			for(int i = 0; i < enemySpriteNames.Length; i++) {
				enemySpriteNames[i] = enemySprites[i].name;
			}
		}

		Setup();
		ReturnToPool();
		enemyPool.Add(this);
	}

	private void OnDestroy()
	{
		enemyPool.Remove(this);
		if (enemyPool.Count == 0)
		{
			enemyPool = null;
			enemySprites = null;
			enemySpriteNames = null;
		}
	}

	public static EnemyPiece Spawn(int _posX, int _posY, int _health, GridType _movementType, EnemyType _unitType)
	{
		EnemyPiece curPiece = null;

		for (int i = 0; i < enemyPool.Count; i++)
		{
			if (enemyPool[i].IsAlive == false)
			{
				curPiece = enemyPool[i];
				curPiece.mbIsAlive = true;
				curPiece.mSpriteRen.enabled = true;

				curPiece.mnPosX = _posX;
				curPiece.mnPosY = _posY;
				curPiece.mnHealth = _health;
				curPiece.SetMovementType(_movementType);
				curPiece.SetUnitType(_unitType);
				int spriteIndex = Array.IndexOf(enemySpriteNames, _unitType.ToString() + "_" + _movementType.ToString());
				curPiece.mSpriteRen.sprite = enemySprites[spriteIndex];

				DungeonManager.Instance.PlaceEnemy(_posX, _posY);
				curPiece.transform.position = DungeonManager.Instance.GridPosToWorldPos(_posX, _posY);

				break;
			}
		}

		#if UNITY_EDITOR
		if (curPiece == null)
			Debug.LogWarning("EnemyPiece pool out of objects. Consider increasing pool size");
		#endif

		return curPiece;
	}

	private void ReturnToPool()
	{
		mbIsAlive = false;
		mSpriteRen.enabled = false;
	}
	#endregion

	public SpriteRenderer mSpriteRen;

	private void Setup()
	{
		mSpriteRen = GetComponent<SpriteRenderer>();

		transform.localScale *= DungeonManager.Instance.ScaleMultiplier;
	}

	private bool mbIsAlive = false;
	public bool IsAlive { get { return mbIsAlive; } }

	private GridType mMovementType = GridType.Pawn;
	public GridType MovementType { get { return mMovementType; } }
	public void SetMovementType(GridType _movementType) { mMovementType = _movementType; }
	private EnemyType mUnitType = EnemyType.Black;
	public EnemyType UnitType { get { return mUnitType; } }
	public void SetUnitType(EnemyType _unitType) { mUnitType = _unitType; }

	protected int mnPosX, mnPosY;
	public int PosX { get { return mnPosX; } }
	public int PosY { get { return mnPosY; } }

	protected int mnHealth = -1;
	public int Health { get { return mnHealth; } }

	public void SetPosition(int _newX, int _newY)
	{
		// TODO: Checking???

		mnPosX = _newX;
		mnPosY = _newY;
	}

	public void TakeDamage(int _damage)
	{
		mnHealth -= _damage;

		// TODO: Check if piece died. Handling for dying.
	}
}