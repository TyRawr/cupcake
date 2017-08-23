using System;
using System.Collections.Generic;

public class CellResult {
	private int points;
	private int fromRow;
	private int fromCol;
	private PieceModel piece; // this is a new pieceColor?
    //The color of the piece that was destroyed;
    private CellState state;
	private bool stateChange = false;
    private bool destroyIndex;
    private MATCHTYPE matchType;


    public CellResult (int point,MATCHTYPE matchTypeForViewToSpawn = MATCHTYPE.NORMAL) {
		this.points = point;
        this.matchType = matchTypeForViewToSpawn;
    }

    public MATCHTYPE GetMatchType()
    {
        return matchType;
    }

    public void SetMatchType(MATCHTYPE matchType)
    {
        this.matchType = matchType;
    }

	public int GetPoints() {
		return points;
	}

    public void SetDestroy(bool toDestroy)
    {
        destroyIndex = toDestroy;
    }

    public bool GetDestroy()
    {
        return destroyIndex;
    }

	public void AddPoints(int pointsToAdd) {
		this.points += pointsToAdd;
	}

	public int GetFromCol() {
		return this.fromCol;
	}

	public void SetFromCol(int fromCol) {
		this.fromCol = fromCol;
	}

	public int GetFromRow() {
		return this.fromRow;
	}

	public void SetFromRow(int fromRow) {
		this.fromRow = fromRow;
	}
		
	public PieceModel GetPiece() {
		return piece;
	}

	public PieceColor GetPieceColor() {
		if (this.piece == null) {
			return PieceColor.NULL;
		}
		return this.piece.GetPieceColor();
	}

	public PieceType GetPieceType() {
		if (this.piece == null) {
			return PieceType.NULL;
		}
		return this.piece.GetPieceType();
	}

    public CellState GetState()
    {
        return state;
    }

    public void SetState(CellState newState)
    {
        this.state = newState;
    }

	public void SetCellStateChange (bool stateChange) {
		this.stateChange = stateChange;
	}
	public bool GetCellStateChange () {
		return stateChange;
	}
	public void SetPiece(PieceModel piece) {
		this.piece = piece;
	}

    bool spawnSpecialPiece = false;
    public void SetSpawnSpecialPiece(PieceModel pm)
    {
        spawnSpecialPiece = true;
        SetSpawnSpecialPieceType(pm.GetPieceColor(), pm.GetPieceType());
    }
    public bool GetSpawnSpecialPiece()
    {
        return spawnSpecialPiece;
    }

    PieceType spawnSpecialPieceType = PieceType.NORMAL;
    PieceColor spawnSpecialPieceColor = PieceColor.NULL;
    public void SetSpawnSpecialPieceType(PieceColor color, PieceType type)
    {
        spawnSpecialPieceType = type;
        spawnSpecialPieceColor = color;
    }

    public PieceType GetSpawnSpecialPieceType()
    {
        return spawnSpecialPieceType;
    }

    public PieceColor GetSpawnSpecialPieceColor()
    {
        return spawnSpecialPieceColor;
    }

	public void Set(CellModel cell) {
		this.fromCol = cell.GetCol();
		this.fromRow = cell.GetRow();
		this.piece = cell.GetPiece();
        this.state = cell.GetState();
	}
}


