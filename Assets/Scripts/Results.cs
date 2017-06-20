using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results {

	private List<CellResult[,]> results;
	private List<CellModel> recommendedMatch;
	private bool hadToShuffle;

	public Results(List<CellResult[,]> _results, List<CellModel> _recommendedMatch, bool _hadToShuffle = false) {
		this.results = _results;
		this.recommendedMatch = _recommendedMatch;
		this.hadToShuffle = _hadToShuffle;
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
}
