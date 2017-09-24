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
    private List<PieceModel> piecesThatMoved;
    private List<PieceModel> piecesThatAreSpawned;
    private Order updatedOrder;
    private int score;
    private MATCHTYPE matchType;
    //TODO also represent any updated special move (if they got any items during the frame)

    public Result(CellResult[,] _cellResults,Order _updatedOrder,int score, List<PieceModel> piecesThatMoved, List<PieceModel> piecesThatSpawned, MATCHTYPE matchT = MATCHTYPE.NORMAL)
    {
        this.matchType = matchT;
        this.cellResult = _cellResults;
        this.updatedOrder = _updatedOrder;
        this.score = score;
        this.piecesThatMoved = piecesThatMoved;
        this.piecesThatAreSpawned = piecesThatSpawned;
    }

    public CellResult[,] GetCellResult()
    {
        return cellResult;
    }

    public List<PieceModel> GetPiecesThatSpawned()
    {
        return piecesThatAreSpawned;
    }

    public List<PieceModel> GetPiecesThatMoved()
    {
        return piecesThatMoved;
    }

    public Order GetOrder()
    {
        return updatedOrder;
    }
     
    public int GetScore()
    {
        return this.score;
    }
    public MATCHTYPE GetMatchType()
    {
        return matchType;
    }
}

public class Order : System.ICloneable
{
    private Dictionary<PieceColor, int> currentOrder;
    private Dictionary<PieceColor, int> neededOrder;

    //initially only have 6 colors for the game. Need holiday update pieces as well.
    private int totalBlueForOrder, totalPinkForOrder, totalYellowForOrder, totalGreenForOrder, totalPurpleForOrder, totalOrangeForOrder, totalFrostingForOrder;
    private int matchedBlue, matchedPink, matchedYellow, matchedGreen, matchedPurple, matchedOrange,matchedFrosting;

    //Add to an old order, some weird memory thing Tyler doesnt under stand is the cause of this overload constructor. Also, tried IClonable->Close() which didnt work :(
    public Order(Order orderToAddTo)
    {
        this.totalBlueForOrder      = orderToAddTo.totalBlueForOrder;
        this.totalPinkForOrder      = orderToAddTo.totalPinkForOrder;
        this.totalYellowForOrder    = orderToAddTo.totalYellowForOrder;
        this.totalGreenForOrder     = orderToAddTo.totalGreenForOrder;
        this.totalPurpleForOrder    = orderToAddTo.totalPurpleForOrder;
        this.totalOrangeForOrder    = orderToAddTo.totalOrangeForOrder;
        this.totalFrostingForOrder  = orderToAddTo.totalFrostingForOrder;

        this.matchedBlue      = orderToAddTo.GetAmountFromColor(PieceColor.BLUE);
        this.matchedPink      = orderToAddTo.GetAmountFromColor(PieceColor.PINK);
        this.matchedYellow    = orderToAddTo.GetAmountFromColor(PieceColor.YELLOW);
        this.matchedGreen     = orderToAddTo.GetAmountFromColor(PieceColor.GREEN);
        this.matchedPurple    = orderToAddTo.GetAmountFromColor(PieceColor.PURPLE);
        this.matchedOrange    = orderToAddTo.GetAmountFromColor(PieceColor.ORANGE);
//        this.matchedFrosting  = orderToAddTo.GetAmountFromColor(Constants.PieceColor.FROSTING);

        // setup the needed order with the totals
        this.neededOrder = new Dictionary<PieceColor, int>();
        this.neededOrder.Add(PieceColor.BLUE, totalBlueForOrder);
        this.neededOrder.Add(PieceColor.PINK, totalPinkForOrder);
        this.neededOrder.Add(PieceColor.YELLOW, totalYellowForOrder);
        this.neededOrder.Add(PieceColor.GREEN, totalGreenForOrder);
        this.neededOrder.Add(PieceColor.PURPLE, totalPurpleForOrder);
        this.neededOrder.Add(PieceColor.ORANGE, totalOrangeForOrder);
//        this.neededOrder.Add(Constants.PieceColor.FROSTING, totalFrostingForOrder);
//        this.neededOrder.Add(Constants.PieceColor.NULL, 0);

        //setup the current order with nothing in em
        this.currentOrder = new Dictionary<PieceColor, int>();
        this.currentOrder.Add(PieceColor.BLUE, matchedBlue);
        this.currentOrder.Add(PieceColor.PINK, matchedPink);
        this.currentOrder.Add(PieceColor.YELLOW, matchedYellow);
        this.currentOrder.Add(PieceColor.GREEN, matchedGreen);
        this.currentOrder.Add(PieceColor.PURPLE, matchedPurple);
        this.currentOrder.Add(PieceColor.ORANGE, matchedOrange);
//        this.currentOrder.Add(PieceColor.FROSTING, matchedFrosting);
//        this.currentOrder.Add(PieceColor.NULL, 0);
    }
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
        this.neededOrder = new Dictionary<PieceColor, int>();
        this.neededOrder.Add(PieceColor.BLUE,totalBlueForOrder);
        this.neededOrder.Add(PieceColor.PINK, totalPinkForOrder);
        this.neededOrder.Add(PieceColor.YELLOW, totalYellowForOrder);
        this.neededOrder.Add(PieceColor.GREEN, totalGreenForOrder);
        this.neededOrder.Add(PieceColor.PURPLE, totalPurpleForOrder);
        this.neededOrder.Add(PieceColor.ORANGE, totalOrangeForOrder);
//        this.neededOrder.Add(PieceColor.FROSTING, totalFrostingForOrder);
//        this.neededOrder.Add(null, 0);

        //setup the current order with nothing in em
        this.currentOrder = new Dictionary<PieceColor, int>();
        this.currentOrder.Add(PieceColor.BLUE, 0);
        this.currentOrder.Add(PieceColor.PINK, 0);
        this.currentOrder.Add(PieceColor.YELLOW, 0);
        this.currentOrder.Add(PieceColor.GREEN, 0);
        this.currentOrder.Add(PieceColor.PURPLE, 0);
        this.currentOrder.Add(PieceColor.ORANGE, 0);
//        this.currentOrder.Add(PieceColor.FROSTING, 0);
//        this.currentOrder.Add(PieceColor.NULL, 0);
    }

    public void AddColorToOrder(PieceColor color)
    {
        int i = currentOrder[color];
        currentOrder[color] = ++i;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public int GetAmountFromColor(PieceColor color)
    {
        return currentOrder[color];
    }

    public int GetTotalNeededFromColor(PieceColor color)
    {
        return neededOrder[color];
    }

    public bool IsComplete()
    {
        //assume yes it is complete
        foreach(KeyValuePair<PieceColor,int> entry in currentOrder)
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

public enum MATCHTYPE
{
    NORMAL,
    ROW,
    COL,
    BOMB,
    ALL_OF
}
