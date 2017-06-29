using System;
using System.Collections.Generic;

public class MatchModel
{
	private List<CellModel> cells;
	private int minRow;
	private int maxRow;
	private int minCol;
	private int maxCol;

	public MatchModel(){}

	public MatchModel (List<CellModel> cells)
	{
		this.cells = cells;
	
		CellModel cell = cells [0];
		int row = cell.GetRow ();
		int col = cell.GetCol ();
		minRow = row;
		maxRow = row;
		minCol = col;
		maxCol = col;

		for (int index = 1; index < cells.Count; index++) 
		{
			cell = cells [index];
			row = cell.GetRow ();
			col = cell.GetCol ();

			if (row > maxRow) {
				maxRow = row;
			} else if (row < minRow) {
				minRow = row;
			}

			if (col > maxCol) {
				maxCol = col;
			} else if (col < minCol) {
				minCol = col;
			}

		}
	}

	public override bool Equals(Object obj) {
		if (obj == null || GetType () != obj.GetType ()) 
		{
			return false;
		}
		MatchModel model = (MatchModel)obj;
		if (this.minRow != model.GetMinRow()) 
		{
			return false;
		}
		if (this.maxRow != model.GetMaxRow()) 
		{
			return false;
		}
		if (this.minCol != model.GetMinCol()) 
		{
			return false;
		}
		if (this.maxCol != model.GetMaxCol()) 
		{
			return false;
		}
		return true;
	}


	public List<CellModel> GetCells() 
	{
		return cells; 
	}
	public int GetMinRow() 
	{
		return minRow;
	}
	public int GetMaxRow() 
	{
		return maxRow;
	}
	public int GetMinCol() 
	{
		return minCol;
	}
	public int GetMaxCol() 
	{
		return maxCol;
	}

	public bool IsVertical() {
		return maxCol - minCol == 0;
	}

	public bool IsHorizontal() {
		return maxRow - minRow ==0;
	}
}

