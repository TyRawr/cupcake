using System;
using System.Collections.Generic;

public class CellResult {
	private int points;
	private int fromRow;
	private int fromCol;
	private Constants.PieceColor pieceColor; // this is a new pieceColor?
	private Constants.PieceType pieceType;
    //The color of the piece that was destroyed;
    private Constants.PieceColor colorOfPieceThatWasDestroyed = Constants.PieceColor.NULL;
    private CellState state;
    private bool destroyIndex;
    private bool isApartOfMatch;


    public CellResult (int point) {
		this.points = point;
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
		
	public Constants.PieceColor GetPieceColor() {
		return this.pieceColor;
	}

	public Constants.PieceType GetPieceType() {
		return this.pieceType;
	}

    public CellState GetState()
    {
        return state;
    }

    public void SetState(CellState newState)
    {
        this.state = newState;
    }

    public void SetColorWasDestroyed(Constants.PieceColor colorThatWasDestroyed)
    {
        colorOfPieceThatWasDestroyed = colorThatWasDestroyed;
    }
    public Constants.PieceColor GetColorThatWasDestroyed()
    {
        return colorOfPieceThatWasDestroyed;
    }

	public void SetPiece(Constants.PieceColor pieceColor, Constants.PieceType pieceType = Constants.PieceType.NORMAL) {
		this.pieceColor = pieceColor;
		this.pieceType = pieceType;
	}

	public void Set(CellModel cell) {
		this.fromCol = cell.GetCol();
		this.fromRow = cell.GetRow();
		this.pieceColor = cell.GetPieceColor();
		this.pieceType = cell.GetPieceType();
        this.state = cell.GetState();
	}
}


