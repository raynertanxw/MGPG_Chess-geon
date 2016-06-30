using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DaburuTools.Action;
using DaburuTools;

public class SmashPanelControls : MonoBehaviour
{
	public static CardTier sSmashTier = CardTier.Bronze;
	private Vector2[] mArrAffectedPositions;
	private Color mAffectedTintCol;

	private Image[] mTileImages;
	private Image[] mTilePieceImages;
	private Text mExplainationText;

	void Awake()
	{
		Transform TileButtonsParent = transform.FindChild("Tiles");
		mTileImages = new Image[TileButtonsParent.childCount];
		mTilePieceImages = new Image[TileButtonsParent.childCount];
		for (int i = 0; i < mTileImages.Length; i++)
		{
			mTileImages[i] = TileButtonsParent.GetChild(i).GetComponent<Image>();
			mTileImages[i].sprite = DungeonManager.Instance.wallTileSprite;

			mTilePieceImages[i] = mTileImages[i].transform.GetChild(0).GetComponent<Image>();
			mTilePieceImages[i].enabled = false;
		}

		mAffectedTintCol = new Color(1.0f, 0.5f, 0.5f);
		mExplainationText = transform.FindChild("Explaination Text").GetComponent<Text>();
	}

	public void Use()
	{
		PlayerPiece player = GameManager.Instance.Player;

		// Focus onto player and animate player.
		BoardScroller.Instance.FocusCameraToPos(
			DungeonManager.Instance.GridPosToWorldPos(player.PosX, player.PosY),
			0.2f,
			Graph.InverseExponential);
		ScaleToAction scaleUp = new ScaleToAction(player.transform, Graph.SmoothStep, Vector3.one * DungeonManager.Instance.ScaleMultiplier * 1.75f, 0.5f);
		ScaleToAction scaleDownHit = new ScaleToAction(player.transform, Graph.Dipper, Vector3.one * DungeonManager.Instance.ScaleMultiplier * 1.1f, 0.25f);
		ActionSequence smashSeq = new ActionSequence(scaleUp, scaleDownHit);
		smashSeq.OnActionFinish += () => {
			// Dramatic shake.
			ShakeAction2D camShake = new ShakeAction2D(Camera.main.transform, 10, 1.85f, Graph.InverseLinear);
			camShake.SetShakeByDuration(0.6f, 35);
			ActionHandler.RunAction(camShake);

			// Make all things take damage.
			for (int i = 0; i < mArrAffectedPositions.Length; i++)
			{
				DungeonBlock block = DungeonManager.Instance.DungeonBlocks[(int)mArrAffectedPositions[i].x, (int)mArrAffectedPositions[i].y];
				switch (block.State)
				{
				case BlockState.EnemyPiece:
					if (RepeatPanelControls.NumRepeatsLeft > 0)
						block.Enemy.TakeDamage(1 * RepeatPanelControls.NumRepeatsLeft);
					else
						block.Enemy.TakeDamage(1);
					break;
				case BlockState.Obstacle:
					DungeonManager.Instance.DestroyBlock(block.PosX, block.PosY);
					break;
				}
			}

			RepeatPanelControls.ClearRepeats();
		};
		ActionHandler.RunAction(smashSeq);

		// Dismiss panel.
		ControlAreaManager.SetCardPanelVisibility(CardType.Smash, false);

		// Organise the cards.
		DeckManager.Instance.ReorganiseCards();
	}

	public void UpdatePanel()
	{
		// Hide all the enemy piece sprites.
		// Also turn all the tile sprites back to white colour.
		for (int i = 0; i < mTilePieceImages.Length; i++)
		{
			mTilePieceImages[i].enabled = false;
			mTileImages[i].color = Color.white;
		}



		// Settle tile and piece sprites.
		int PlayerPosX = GameManager.Instance.Player.PosX;
		int PlayerPosY = GameManager.Instance.Player.PosY;
		for (int i = 0; i < mTileImages.Length; i++)
		{
			int x = PlayerPosX + ((i % 5) - 2);
			int y = PlayerPosY + ((i / 5) - 2);
			
			// Check if Exit Tile.
			if (x == DungeonManager.Instance.ExitPosX && y == DungeonManager.Instance.ExitPosY)
			{
				mTileImages[i].sprite = DungeonManager.Instance.exitSprite;

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
					mTileImages[i].sprite = DungeonManager.Instance.whiteTileSprite;
				else
					mTileImages[i].sprite = DungeonManager.Instance.blackTileSprite;

				continue;
			}

			// Otherwise, check if it's an enemy piece or a wall.
			if (DungeonManager.Instance.IsEnemyPos(x, y))
			{
				if (DungeonManager.Instance.IsWhiteTile(x, y))
					mTileImages[i].sprite = DungeonManager.Instance.whiteTileSprite;
				else
					mTileImages[i].sprite = DungeonManager.Instance.blackTileSprite;

				// Check and display the enemy piece
				mTilePieceImages[i].enabled = true;
				mTilePieceImages[i].sprite = DungeonManager.Instance.DungeonBlocks[x, y].Enemy.mSpriteRen.sprite;
			}
			// Else check if it is out of bounds.
			else
			{
				mTileImages[i].sprite = DungeonManager.Instance.wallTileSprite;
				if (DungeonManager.Instance.IsValidCell(x, y) == false)
					mTileImages[i].color = Color.gray;
			}
		}
		// Player sprite.
		mTilePieceImages[12].enabled = true;
		mTilePieceImages[12].sprite = GameManager.Instance.Player.mSpriteRen.sprite;



		// Tint affected areas.
		mArrAffectedPositions = GetAffectedPos(sSmashTier, GameManager.Instance.Player);
		for (int i = 0; i < mArrAffectedPositions.Length; i++)
		{
			int tileID = PosToTileID((int)mArrAffectedPositions[i].x, (int)mArrAffectedPositions[i].y, GameManager.Instance.Player);
			mTileImages[tileID].color = mAffectedTintCol;
		}


		// Update Text
		if (RepeatPanelControls.NumRepeatsLeft > 0)
			mExplainationText.text = "Deals x" + (1 * RepeatPanelControls.NumRepeatsLeft).ToString() + " Damage to the tiles tinted in red.";
		else
			mExplainationText.text = "Deals x1 Damage to the tiles tinted in red.";
	}

	#region Helper functions
	private Vector2[] GetAffectedPos(CardTier _tier, PlayerPiece _player)
	{
		Vector2 playerPos = new Vector2(_player.PosX, _player.PosY);
		Vector2[] affectedPos = null;
		switch (_tier)
		{
		case CardTier.Bronze:
			affectedPos = new Vector2[4];
			// Up down left right
			affectedPos[0] = new Vector2(playerPos.x, playerPos.y + 1);
			affectedPos[1] = new Vector2(playerPos.x, playerPos.y - 1);
			affectedPos[2] = new Vector2(playerPos.x + 1, playerPos.y);
			affectedPos[3] = new Vector2(playerPos.x - 1, playerPos.y);
			break;
		case CardTier.Silver:
			affectedPos = new Vector2[8];
			// Up down left right
			affectedPos[0] = new Vector2(playerPos.x, playerPos.y + 1);
			affectedPos[1] = new Vector2(playerPos.x, playerPos.y - 1);
			affectedPos[2] = new Vector2(playerPos.x + 1, playerPos.y);
			affectedPos[3] = new Vector2(playerPos.x - 1, playerPos.y);

			// adjacent corners
			affectedPos[4] = new Vector2(playerPos.x + 1, playerPos.y + 1);
			affectedPos[6] = new Vector2(playerPos.x + 1, playerPos.y - 1);
			affectedPos[5] = new Vector2(playerPos.x - 1, playerPos.y + 1);
			affectedPos[7] = new Vector2(playerPos.x - 1, playerPos.y - 1);
			break;
		case CardTier.Gold:
			affectedPos = new Vector2[24];
			// Up down left right
			affectedPos[0] = new Vector2(playerPos.x, playerPos.y + 1);
			affectedPos[1] = new Vector2(playerPos.x, playerPos.y - 1);
			affectedPos[2] = new Vector2(playerPos.x + 1, playerPos.y);
			affectedPos[3] = new Vector2(playerPos.x - 1, playerPos.y);

			// Adjacent corners
			affectedPos[4] = new Vector2(playerPos.x + 1, playerPos.y + 1);
			affectedPos[6] = new Vector2(playerPos.x + 1, playerPos.y - 1);
			affectedPos[5] = new Vector2(playerPos.x - 1, playerPos.y + 1);
			affectedPos[7] = new Vector2(playerPos.x - 1, playerPos.y - 1);

			// Top outer row
			affectedPos[8] = new Vector2(playerPos.x - 2, playerPos.y + 2);
			affectedPos[9] = new Vector2(playerPos.x - 1, playerPos.y + 2);
			affectedPos[10] = new Vector2(playerPos.x, playerPos.y + 2);
			affectedPos[11] = new Vector2(playerPos.x + 1, playerPos.y + 2);
			affectedPos[12] = new Vector2(playerPos.x + 2, playerPos.y + 2);

			// Bottom outer row
			affectedPos[13] = new Vector2(playerPos.x - 2, playerPos.y - 2);
			affectedPos[14] = new Vector2(playerPos.x - 1, playerPos.y - 2);
			affectedPos[15] = new Vector2(playerPos.x, playerPos.y - 2);
			affectedPos[16] = new Vector2(playerPos.x + 1, playerPos.y - 2);
			affectedPos[17] = new Vector2(playerPos.x + 2, playerPos.y - 2);

			// Outer right
			affectedPos[18] = new Vector2(playerPos.x + 2, playerPos.y + 1);
			affectedPos[19] = new Vector2(playerPos.x + 2, playerPos.y);
			affectedPos[20] = new Vector2(playerPos.x + 2, playerPos.y - 1);

			// Outer left
			affectedPos[21] = new Vector2(playerPos.x - 2, playerPos.y + 1);
			affectedPos[22] = new Vector2(playerPos.x - 2, playerPos.y);
			affectedPos[23] = new Vector2(playerPos.x - 2, playerPos.y - 1);
			break;
		}

		for (int i = 0; i < affectedPos.Length; i++)
		{
			// If not a valid pos, default to player pos (which will have no effect when processed above).
			if (DungeonManager.Instance.IsValidCell((int) affectedPos[i].x, (int) affectedPos[i].y) == false)
			{
				affectedPos[i] = playerPos;
			}
		}

		return affectedPos;
	}

	private int PosToTileID(int _posX, int _posY, PlayerPiece _player)
	{
		return (_posY - _player.PosY + 2) * 5 + (_posX - _player.PosX + 2);
	}
	#endregion
}
