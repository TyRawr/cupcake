using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results {

	private List<CellResult[,]> results;
	private List<CellModel> recommendedMatch;
	private bool hadToShuffle;
	private int updatedMove;

	public Results(List<CellResult[,]> _results, List<CellModel> _recommendedMatch, bool _hadToShuffle, int _updatedMove) {
		this.results = _results;
		this.recommendedMatch = _recommendedMatch;
		this.hadToShuffle = _hadToShuffle;
		this.updatedMove = _updatedMove;
	}

	public List<CellResult[,]> GetCellResults() {
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
