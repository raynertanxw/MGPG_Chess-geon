﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MovementPanelControls : MonoBehaviour
{
	private Image[] mTileButtonImages;
	private Image[] mTilePieceImages;
	private Button[] mTileButtons;

	void Awake()
	{
		Transform TileButtonsParent = transform.FindChild("Tile Buttons");
		mTileButtonImages = new Image[TileButtonsParent.childCount];
		mTilePieceImages = new Image[TileButtonsParent.childCount];
		mTileButtons = new Button[TileButtonsParent.childCount];
		for (int i = 0; i < mTileButtonImages.Length; i++)
		{
			mTileButtonImages[i] = TileButtonsParent.GetChild(i).GetComponent<Image>();
			mTileButtonImages[i].sprite = DungeonManager.Instance.wallTileSprite;
			
			mTileButtons[i] = mTileButtonImages[i].gameObject.GetComponent<Button>();
			mTileButtons[i].interactable = false;

			mTilePieceImages[i] = mTileButtonImages[i].transform.GetChild(0).GetComponent<Image>();
			mTilePieceImages[i].enabled = false;
		}
	}

	// Assumes that only valid tiles can be pressed.
	// Checking should be done in UpdatePanel.
	public void TilePressed(int _tileID)
	{
		// Get PlayerPiece to execute move.
		int targetPosX = GameManager.Instance.Player.PosX + (_tileID % 5) - 2;
		int targetPosY = GameManager.Instance.Player.PosY + (_tileID / 5) - 2;
		GameManager.Instance.Player.ExecuteTurn(targetPosX, targetPosY);

		// Dismiss panel.
		CardAreaButtons.SetCardPanelVisibility(CardType.Movement, false);
	}

	public void UpdatePanel()
	{
		// Set all tile buttons to not interactable to be safe.
		// Also hide all the enemy piece sprites.
		for (int i = 0; i < mTileButtons.Length; i++)
		{
			mTileButtons[i].interactable = false;
			mTilePieceImages[i].enabled = false;
		}



		// Settle tile and piece sprites.
		int PlayerPosX = GameManager.Instance.Player.PosX;
		int PlayerPosY = GameManager.Instance.Player.PosY;
		for (int i = 0; i < mTileButtonImages.Length; i++)
		{
			int x = PlayerPosX + ((i % 5) - 2);
			int y = PlayerPosY + ((i / 5) - 2);

			// If it's empty, check if white or black tile.
			if (DungeonManager.Instance.IsCellEmpty(x, y))
			{
				if (DungeonManager.Instance.IsWhiteTile(x, y))
					mTileButtonImages[i].sprite = DungeonManager.Instance.whiteTileSprite;
				else
					mTileButtonImages[i].sprite = DungeonManager.Instance.blackTileSprite;

				continue;
			}

			// Otherwise, check if it's an enemy piece or a wall.
			if (DungeonManager.Instance.IsEnemyPos(x, y))
			{
				if (DungeonManager.Instance.IsWhiteTile(x, y))
					mTileButtonImages[i].sprite = DungeonManager.Instance.whiteTileSprite;
				else
					mTileButtonImages[i].sprite = DungeonManager.Instance.blackTileSprite;

				// Check and display the enemy piece
				mTilePieceImages[i].enabled = true;
				for (int i_enemy = 0; i_enemy < GameManager.Instance.mEnemyList.Count; i_enemy++)
				{
					EnemyPiece curEnemyPiece = GameManager.Instance.mEnemyList[i_enemy];
					if (curEnemyPiece.PosX != x)
						continue;
					if (curEnemyPiece.PosY != y)
						continue;

					mTilePieceImages[i].sprite = curEnemyPiece.mSpriteRen.sprite;
				}
			}
			else
			{
				mTileButtonImages[i].sprite = DungeonManager.Instance.wallTileSprite;
			}
		}
		// Player sprite.
		mTilePieceImages[12].enabled = true;
		mTilePieceImages[12].sprite = GameManager.Instance.Player.mSpriteRen.sprite;



		// Set clickable tiles.
		GridType movementType = GameManager.Instance.Player.MovementType;
		switch (movementType)
		{
		case GridType.Pawn:
			// Check top
			if (DungeonManager.Instance.IsCellEmpty(PlayerPosX, PlayerPosY + 1))
			{
				int tileID = (PlayerPosY + 1 - PlayerPosY + 2) * 5 + (PlayerPosX - PlayerPosX + 2);
				mTileButtons[tileID].interactable = true;
				mTileButtonImages[tileID].sprite = DungeonManager.Instance.selectableSprite;
			}

			// Check bottom
			if (DungeonManager.Instance.IsCellEmpty(PlayerPosX, PlayerPosY - 1))
			{
				int tileID = (PlayerPosY - 1 - PlayerPosY + 2) * 5 + (PlayerPosX - PlayerPosX + 2);
				mTileButtons[tileID].interactable = true;
				mTileButtonImages[tileID].sprite = DungeonManager.Instance.selectableSprite;
			}

			// Check corner for enemies. (Pawn only can attack if there's an enemy.
			// Top left
			if (DungeonManager.Instance.IsEnemyPos(PlayerPosX - 1, PlayerPosY + 1))
			{
				int tileID = (PlayerPosY + 1 - PlayerPosY + 2) * 5 + (PlayerPosX - 1 - PlayerPosX + 2);
				mTileButtons[tileID].interactable = true;
				mTileButtonImages[tileID].sprite = DungeonManager.Instance.selectableSprite;
			}

			// Top Right
			if (DungeonManager.Instance.IsEnemyPos(PlayerPosX + 1, PlayerPosY + 1))
			{
				int tileID = (PlayerPosY + 1 - PlayerPosY + 2) * 5 + (PlayerPosX + 1 - PlayerPosX + 2);
				mTileButtons[tileID].interactable = true;
				mTileButtonImages[tileID].sprite = DungeonManager.Instance.selectableSprite;
			}

			// Bottom Left
			if (DungeonManager.Instance.IsEnemyPos(PlayerPosX - 1, PlayerPosY - 1))
			{
				int tileID = (PlayerPosY - 1 - PlayerPosY + 2) * 5 + (PlayerPosX - 1 - PlayerPosX + 2);
				mTileButtons[tileID].interactable = true;
				mTileButtonImages[tileID].sprite = DungeonManager.Instance.selectableSprite;
			}

			// Bottom Right
			if (DungeonManager.Instance.IsEnemyPos(PlayerPosX + 1, PlayerPosY - 1))
			{
				int tileID = (PlayerPosY - 1 - PlayerPosY + 2) * 5 + (PlayerPosX + 1 - PlayerPosX + 2);
				mTileButtons[tileID].interactable = true;
				mTileButtonImages[tileID].sprite = DungeonManager.Instance.selectableSprite;
			}
			break;
		default:
			LinkedList<Node> neighbours = DungeonManager.Instance.Grids[(int)movementType].nodes[PlayerPosX, PlayerPosY].neighbours;
			for (LinkedListNode<Node> curLinkedNode = neighbours.First; curLinkedNode != null; curLinkedNode = curLinkedNode.Next)
			{
				Node curNode = curLinkedNode.Value;
				// Check if it's empty cell.
				// Otherwise if it's enemy, still movable tile.
				if (DungeonManager.Instance.IsCellEmpty(curNode.PosX, curNode.PosY) ||
					DungeonManager.Instance.IsEnemyPos(curNode.PosX, curNode.PosY))
				{
					int tileID = (curNode.PosY - PlayerPosY + 2) * 5 + (curNode.PosX - PlayerPosX + 2);
					mTileButtons[tileID].interactable = true;
					mTileButtonImages[tileID].sprite = DungeonManager.Instance.selectableSprite;
				}
			}
			break;
		}
	}
}