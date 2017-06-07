using System;
using System.Collections.Generic;

public class CellResult {
	private int points;
	private int fromRow;
	private int fromCol;
	private Constants.PieceColor pieceColor;
	private Constants.PieceType pieceType;

	public CellResult (int point) {
		this.points = point;
	}

	public int GetPoints() {
		return points;
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

	public void SetPiece(Constants.PieceColor pieceColor, Constants.PieceType pieceType = Constants.PieceType.NORMAL) {
		this.pieceColor = pieceColor;
		this.pieceType = pieceType;
	}

	public void Set(CellModel cell) {
		this.fromCol = cell.GetCol();
		this.fromRow = cell.GetRow();
		this.pieceColor = cell.GetPieceColor();
		this.pieceType = cell.GetPieceType();
	}
}


