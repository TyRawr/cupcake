using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results {

	private List<Result> results;
	private List<CellModel> recommendedMatch;
	private bool hadToShuffle;
	private int updatedMove;
    GAMEOVERSTATE gameOverState;

	public Results(List<Result> _results, List<CellModel> _recommendedMatch, bool _hadToShuffle, int _updatedMove, GAMEOVERSTATE gameOverState) {
		this.results = _results;
		this.recommendedMatch = _recommendedMatch;
		this.hadToShuffle = _hadToShuffle;
		this.updatedMove = _updatedMove;
        this.gameOverState = gameOverState;
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
    public GAMEOVERSTATE GetGameOver() {
        return gameOverState;
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
    private Dictionary<Constants.PieceColor, int> currentOrder;
    private Dictionary<Constants.PieceColor, int> neededOrder;

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

        // setup the needed order with the totals
        this.neededOrder = new Dictionary<Constants.PieceColor, int>();
        this.neededOrder.Add(Constants.PieceColor.BLUE,totalBlueForOrder);
        this.neededOrder.Add(Constants.PieceColor.PINK, totalPinkForOrder);
        this.neededOrder.Add(Constants.PieceColor.YELLOW, totalYellowForOrder);
        this.neededOrder.Add(Constants.PieceColor.GREEN, totalGreenForOrder);
        this.neededOrder.Add(Constants.PieceColor.PURPLE, totalPurpleForOrder);
        this.neededOrder.Add(Constants.PieceColor.ORANGE, totalOrangeForOrder);
        this.neededOrder.Add(Constants.PieceColor.FROSTING, totalFrostingForOrder);
        this.neededOrder.Add(Constants.PieceColor.NULL, 0);

        //setup the current order with nothing in em
        this.currentOrder = new Dictionary<Constants.PieceColor, int>();
        this.currentOrder.Add(Constants.PieceColor.BLUE, 0);
        this.currentOrder.Add(Constants.PieceColor.PINK, 0);
        this.currentOrder.Add(Constants.PieceColor.YELLOW, 0);
        this.currentOrder.Add(Constants.PieceColor.GREEN, 0);
        this.currentOrder.Add(Constants.PieceColor.PURPLE, 0);
        this.currentOrder.Add(Constants.PieceColor.ORANGE, 0);
        this.currentOrder.Add(Constants.PieceColor.FROSTING, 0);
        this.currentOrder.Add(Constants.PieceColor.NULL, 0);
    }

    public void AddColorToOrder(Constants.PieceColor color)
    {
        int i = currentOrder[color];
        currentOrder[color] = ++i;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public int GetAmountFromColor(Constants.PieceColor color)
    {
        return currentOrder[color];
    }

    public int GetTotalNeededFromColor(Constants.PieceColor color)
    {
        return neededOrder[color];
    }

    public bool IsComplete()
    {
        //assume yes it is complete
        foreach(KeyValuePair<Constants.PieceColor,int> entry in currentOrder)
        {
            if(entry.Value < neededOrder[entry.Key] )
            {
                return false;
            }
        }
        return true;
    }
}

public enum GAMEOVERSTATE
{
    NULL,
    FAILURE_OUT_OF_MOVES,
    SUCCESS_ORDER_MET
}