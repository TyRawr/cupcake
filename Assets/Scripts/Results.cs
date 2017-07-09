using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results {

	private List<Result> results;
	private List<CellModel> recommendedMatch;
	private bool hadToShuffle;
	private int updatedMove;
    private bool gameOver;

	public Results(List<Result> _results, List<CellModel> _recommendedMatch, bool _hadToShuffle, int _updatedMove, bool gameOver) {
		this.results = _results;
		this.recommendedMatch = _recommendedMatch;
		this.hadToShuffle = _hadToShuffle;
		this.updatedMove = _updatedMove;
	}

	public List<Result> GetCellResults() {
		return results;
	}
	public List<CellModel> GetRecommendedMatch() {
		return recommendedMatch;
	}
	public bool GetHadToShuffle() {
		return hadToShuffle;
	}
	public int GetMoves() {
		return updatedMove;
	}
    public bool GetGameOver() {
        return gameOver;
    }
}

public class Result
{
    private CellResult[,] cellResult;
    private Order updatedOrder;
    //TODO also represent any updated special move (if they got any items during the frame)

    public Result(CellResult[,] _cellResults,Order _updatedOrder,int score)
    {
        this.cellResult = _cellResults;
        this.updatedOrder = _updatedOrder;
    }

    public CellResult[,] GetCellResult()
    {
        return cellResult;
    }

    public Order GetOrder()
    {
        return updatedOrder;
    }
     
}

public class Order : System.ICloneable
{
    //initially only have 6 colors for the game. Need holiday update pieces as well.
    private int totalBlueForOrder, totalPinkForOrder, totalYellowForOrder, totalGreenForOrder, totalPurpleForOrder, totalOrangeForOrder, totalFrostingForOrder;
    private int matchedBlue, matchedPink, matchedYellow, matchedGreen, matchedPurple, matchedOrange,matchedFrosting;
    //Setup the initial order for the level
    public Order(int totalBlueForOrder, int totalPinkForOrder, int totalYellowForOrder, int totalGreenForOrder, int totalPurpleForOrder, int totalOrangeForOrder,int totalFrostingForOrder) {
        this.totalBlueForOrder = totalBlueForOrder;
        this.totalPinkForOrder = totalPinkForOrder;
        this.totalYellowForOrder = totalYellowForOrder;
        this.totalGreenForOrder = totalGreenForOrder;
        this.totalPurpleForOrder = totalPurpleForOrder;
        this.totalOrangeForOrder = totalOrangeForOrder;
        this.totalFrostingForOrder = totalFrostingForOrder;
    }

    public void AddColorToOrder(Constants.PieceColor color)
    {
        if(color == Constants.PieceColor.BLUE)
        {
            matchedBlue++;
        }
        if (color == Constants.PieceColor.PINK)
        {
            matchedPink++;
        }
        if (color == Constants.PieceColor.YELLOW)
        {
            matchedYellow++;
        }
        if (color == Constants.PieceColor.GREEN)
        {
            matchedGreen++;
        }
        if (color == Constants.PieceColor.PURPLE)
        {
            matchedPurple++;
        }
        if (color == Constants.PieceColor.ORANGE)
        {
            matchedOrange++;
        }
        if (color == Constants.PieceColor.FROSTING)
        {
            matchedFrosting++;
        }
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public int GetAmountFromColor(Constants.PieceColor color)
    {
        if (color == Constants.PieceColor.BLUE)
        {
            return matchedBlue;
        }
        if (color == Constants.PieceColor.PINK)
        {
            return matchedPink;
        }
        if (color == Constants.PieceColor.YELLOW)
        {
            return matchedYellow;
        }
        if (color == Constants.PieceColor.GREEN)
        {
            return matchedGreen;
        }
        if (color == Constants.PieceColor.PURPLE)
        {
            return matchedPurple;
        }
        if (color == Constants.PieceColor.ORANGE)
        {
            return matchedOrange;
        }
        if (color == Constants.PieceColor.FROSTING)
        {
            return matchedFrosting;
        }
        return 0;
    }

    public int GetTotalNeededFromColor(Constants.PieceColor color)
    {
        if (color == Constants.PieceColor.BLUE)
        {
            return totalBlueForOrder;
        }
        if (color == Constants.PieceColor.PINK)
        {
            return totalPinkForOrder;
        }
        if (color == Constants.PieceColor.YELLOW)
        {
            return totalYellowForOrder;
        }
        if (color == Constants.PieceColor.GREEN)
        {
            return totalGreenForOrder;
        }
        if (color == Constants.PieceColor.PURPLE)
        {
            return totalPurpleForOrder;
        }
        if (color == Constants.PieceColor.ORANGE)
        {
            return totalOrangeForOrder;
        }
        if (color == Constants.PieceColor.FROSTING)
        {
            return totalFrostingForOrder;
        }
        return 0;
    }
}