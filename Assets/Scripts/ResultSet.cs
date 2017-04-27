using System;
using System.Collections.Generic;

public class ResultSet
{
	
	private Stack<List<CellResult>> matches;
	private List<List<PieceModel>> columns;

	public ResultSet (Stack<List<CellResult>> matches, List<List<PieceModel>> columns)
	{
		this.matches = matches;
		this.columns = columns;
	}

	public Stack<List<CellResult>> GetMatches () {
		return matches; 
	}

	public List<List<PieceModel>> GetColumns() {
		return columns;
	}

}

public class CellResult {
	private int row;
	private int col;
	private int points;

	public CellResult (int row, int col, int point) {
		this.row = row;
		this.col = col;
		this.points = points;
	}

	private int GetRow() {
		return row;
	}

	private int GetCol() {
		return col;
	}

	private int GetPoints() {
		return points;
	}

}


