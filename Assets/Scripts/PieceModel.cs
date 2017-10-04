﻿using System;
using System.Collections.Generic;
using System.Linq;

[Serializable()]
public class PieceModel : ICloneable
{
	private PieceColor color;
	private PieceType type;
	private List<Point> path;
    private bool spawn = true;
    /*
	public PieceModel (PieceColor color, PieceType type = PieceType.NORMAL)
	{
		this.color = color;
		this.type = type;
		this.path = new List<Point> ();
	}
    */
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

    public bool GetSpawn()
    {
        return spawn;
    }

    public void SetSpawn(bool _spawn)
    {
        spawn = _spawn;
    }

	public void AddToPath(int row, int col) {
		path.Add(new Point(row, col));
        SortPath();
    }

    public bool PathContainsRowCol(int row, int col)
    {
        foreach(var p in path)
        {
            if (p.row == row && p.col == col)
                return true;
        }
        return false;
    }

    public void SortPath()
    {
        path = path.OrderBy(p => p.row).ToList();
    }

    public void ClearPath(int newRow, int newCol)
    {
        path.Clear();
        path.Add(new Point(newRow, newCol));
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

    public object Clone()
    {
        return this.MemberwiseClone();
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