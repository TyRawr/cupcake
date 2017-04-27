using System;
using System.Collections.Generic;

public class ResultSet
{
	
	private List<List<CellResult>> matches;
	private List<List<PieceModel>> columns;

	public ResultSet (List<List<CellResult>> matches, List<List<PieceModel>> columns)
	{
		this.matches = matches;
		this.columns = columns;
	}

	public List<List<CellResult>> GetMatches () {
		return matches; 
	}

	public List<List<PieceModel>> GetColumns() {
		return columns;
	}

}

public class CellResult {
	private int points;

	public CellResult (int point) {
		this.points = point;
	}

	public int GetPoints() {
		return points;
	}

	public void AddPoints(int pointsToAdd) {
		this.points += pointsToAdd;
	}

}


