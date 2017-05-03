using System;
using System.Collections.Generic;

public class ResultSet
{
	
	private CellResult[,] matches;
	private List<PieceModel>[] newPieces; // iterate columns, these are new pieces

	public ResultSet (CellResult[,] matches, List<PieceModel>[] pieces)
	{
		this.matches = matches;
		this.newPieces = pieces;
	}

	public CellResult[,] GetMatches () {
		return matches; 
	}

	public List<PieceModel>[]  GetNewPieces() {
		return newPieces;
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


