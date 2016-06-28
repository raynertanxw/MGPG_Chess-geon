using UnityEngine;
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
		ControlAreaManager.SetCardPanelVisibility(CardType.Movement, false);

		// Organise the cards.
		DeckManager.Instance.ReorganiseCards();
	}

	public void UpdatePanel()
	{
		// Set all tile buttons to not interactable to be safe.
		// Also hide all the enemy piece sprites.
		for (int i = 0; i < mTileButtons.Length; i++)
		{
			mTileButtons[i].interactable = false;
			mTileButtonImages[i].color = Color.white;
			mTilePieceImages[i].enabled = false;
		}



		// Settle tile and piece sprites.
		int PlayerPosX = GameManager.Instance.Player.PosX;
		int PlayerPosY = GameManager.Instance.Player.PosY;
		for (int i = 0; i < mTileButtonImages.Length; i++)
		{
			int x = PlayerPosX + ((i % 5) - 2);
			int y = PlayerPosY + ((i / 5) - 2);

			// Check for special tiles (Shop & Exit)
			// TODO: Check shop tile.
			// Check if Exit Tile.
			if (x == DungeonManager.Instance.ExitPosX && y == DungeonManager.Instance.ExitPosY)
			{
				mTileButtonImages[i].sprite = DungeonManager.Instance.exitSprite;

				if (DungeonManager.Instance.IsEnemyPos(x, y))
				{
					// Check and display the enemy piece
					mTilePieceImages[i].enabled = true;
					mTilePieceImages[i].sprite = DungeonManager.Instance.DungeonBlocks[x, y].Enemy.mSpriteRen.sprite;
				}

				continue;
			}


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
				mTilePieceImages[i].sprite = DungeonManager.Instance.DungeonBlocks[x, y].Enemy.mSpriteRen.sprite;
			}
			// Else check if it is out of bounds.
			else
			{
				mTileButtonImages[i].sprite = DungeonManager.Instance.wallTileSprite;
				if (DungeonManager.Instance.IsValidCell(x, y) == false)
					mTileButtonImages[i].color = Color.gray;
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
			else if (DungeonManager.Instance.IsExitCell(PlayerPosX, PlayerPosY + 1))
			{
				int tileID = (PlayerPosY + 1 - PlayerPosY + 2) * 5 + (PlayerPosX - PlayerPosX + 2);
				mTileButtons[tileID].interactable = true;
				mTileButtonImages[tileID].sprite = DungeonManager.Instance.exitSelectableSprite;
			}

			// Check bottom
			if (DungeonManager.Instance.IsCellEmpty(PlayerPosX, PlayerPosY - 1))
			{
				int tileID = (PlayerPosY - 1 - PlayerPosY + 2) * 5 + (PlayerPosX - PlayerPosX + 2);
				mTileButtons[tileID].interactable = true;
				mTileButtonImages[tileID].sprite = DungeonManager.Instance.selectableSprite;
			}
			else if (DungeonManager.Instance.IsExitCell(PlayerPosX, PlayerPosY + 1))
			{
				int tileID = (PlayerPosY - 1 - PlayerPosY + 2) * 5 + (PlayerPosX - PlayerPosX + 2);
				mTileButtons[tileID].interactable = true;
				mTileButtonImages[tileID].sprite = DungeonManager.Instance.exitSelectableSprite;
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
				// If still not, then could be exit tile.
				if (DungeonManager.Instance.IsCellEmpty(curNode.PosX, curNode.PosY) ||
					DungeonManager.Instance.IsEnemyPos(curNode.PosX, curNode.PosY))
				{
					int tileID = (curNode.PosY - PlayerPosY + 2) * 5 + (curNode.PosX - PlayerPosX + 2);
					mTileButtons[tileID].interactable = true;
					mTileButtonImages[tileID].sprite = DungeonManager.Instance.selectableSprite;
				}
				else if (DungeonManager.Instance.IsExitCell(curNode.PosX, curNode.PosY))
				{
					int tileID = (curNode.PosY - PlayerPosY + 2) * 5 + (curNode.PosX - PlayerPosX + 2);
					mTileButtons[tileID].interactable = true;
					mTileButtonImages[tileID].sprite = DungeonManager.Instance.exitSelectableSprite;
				}
			}
			break;
		}
	}
}
