using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results {

	private List<Result> results;
	private List<CellModel> recommendedMatch;
	private bool hadToShuffle;
	private int updatedMove;

	public Results(List<Result> _results, List<CellModel> _recommendedMatch, bool _hadToShuffle, int _updatedMove) {
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
}

public class Result
{
    private CellResult[,] cellResult;
    private Order updatedOrder;
    //TODO also represent any updated special move (if they got any items during the frame)

    public Result(CellResult[,] _cellResults,Order _updatedOrder)
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

public class Order
{
    public Order() { }
}
