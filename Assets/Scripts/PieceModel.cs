using System;
using System.Collections.Generic;

public class PieceModel
{
	private PieceColor color;
	private PieceType type;
	private List<Point> path;

	public PieceModel (PieceColor color, PieceType type = PieceType.NORMAL)
	{
		this.color = color;
		this.type = type;
		this.path = new List<Point> ();
	}

	/**
	 * Construction method for Special Pieces that spawn w/in board mid life-cycle
	 */
	public PieceModel (PieceColor color, PieceType type, Point origin)
	{
		this.color = color;
		this.type = type;
		this.path = new List<Point> ();
		this.path.Add (origin);
	}

	public void AddToPath(int row, int col) {
		path.Add(new Point(row, col));
	}

	// GETTERS/SETTERS =============================

	public PieceColor GetPieceColor() {
		return color;
	}

	public PieceType GetPieceType() {
		return type;
	}

	public void SetPieceType(PieceType type) {
		this.type = type;
	}

	public List<Point> GetPath() {
		return path;
	}

	public void SetPath(List<Point> path) {
		this.path = path;
	}
}

// ENUMS ==========================================

public enum PieceColor 
{
	BLUE,
	GREEN,
	ORANGE,
	PINK,
	PURPLE,
	YELLOW,
	NULL
}

public enum PieceType 
{
	NORMAL,
	ALL,
	BOMB,
	FROSTING,
	STRIPED_ROW,
	STRIPED_COL,
	NULL
}	