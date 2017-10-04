//// The strategy here is to move pieces down (not to find null);
//private void DropPieces(CellResult[,] cellResults,List<PieceModel> piecesThatSpawn) {
//	int cols = gameBoard.GetLength(1);
//	int rows = gameBoard.GetLength(0);
//	bool neededToDrop = false;
//
//	// Straight Drop
//	for (int col = cols - 1; col >= 0; col--) {
//		for (int row = rows - 2; row >= 0; row--) {
//			// try to drop piece
//			CellModel cell = gameBoard [row, col];
//			if (cell.IsDroppable()) {
//				// Drop Down
//				if (AttemptToMovePiece (gameBoard [row + 1, col], cell.GetPiece (), cellResults, false)) {
//					cell.Consume (false, null, order);
//				}
//			}
//		}
//	}
//
//	// Spawn Pieces Straight
//	for (int col = cols - 1; col >= 0; col--) {
//		AttemptToSpawnPiece(col, cellResults);
//	}
//
//	// Erosion Drop
//	for (int col = cols - 1; col >= 0; col--) {
//		for (int row = rows - 2; row >= 0; row--) {
//			// try to drop piece
//			CellModel cell = gameBoard [row, col];
//			if (cell.IsDroppable()) {
//				// Drop Down
//				if (AttemptToMovePiece (gameBoard [row + 1, col], cell.GetPiece (), cellResults, false)) {
//					cell.Consume (false, null,order);
//					continue;
//				}
//				// Drop Left
//				if (col > 0) {
//					if (AttemptToMovePiece (gameBoard [row + 1, col - 1], cell.GetPiece (), cellResults, true)) {
//						cell.Consume (false, null,order);
//						continue;
//					}
//				}
//				// Drop Right
//				if (col < gameBoard.GetLength (1) - 1) {
//					if (AttemptToMovePiece (gameBoard [row + 1, col + 1], cell.GetPiece (), cellResults, true)) {
//						cell.Consume (false, null,order);
//						continue;
//					}	
//				}
//			}
//		}
//	}
//
//	// Spawn Pieces Erosion
//	for (int col = cols - 1; col >= 0; col--) {
//		AttemptToSpawnPiece(col, cellResults, true);
//	}
//}
//
//private bool AttemptToMovePiece(CellModel cell, PieceModel piece, CellResult [,] cellResults, bool erode = false) {
//	int row = cell.GetRow ();
//	int col = cell.GetCol ();
//
//	// If we can drop past this cell, try to
//	if (cell.IsSkippable() && row < gameBoard.GetLength(0) - 1) {
//		// Attempt to move piece DOWN
//		if (AttemptToMovePiece (gameBoard [row + 1, col], piece, cellResults, erode)) {
//			return true;
//		}
//
//		if (erode) {
//			// Attempt to move piece LEFT
//			if (col > 0) {
//				if (AttemptToMovePiece (gameBoard [row + 1, col - 1], piece, cellResults, erode)) {
//					return true;
//				}
//			}
//			// Attempt to move piece RIGHT
//			if (col < gameBoard.GetLength (1) - 1) {
//				if (AttemptToMovePiece (gameBoard [row + 1, col + 1], piece, cellResults, erode)) {
//					return true;
//				}
//			}
//		}
//	}
//
//	if (cell.IsWanting ()) {
//		cell.SetPiece (piece);
//		CellResult cellResult = cellResults [row, col];
//		if (cellResult == null) {
//			cellResult = new CellResult (0);
//			cellResults [row, col] = cellResult;
//		}
//		cellResult.Set (cell);
//		return true;
//	} 
//	return false;
//}
//
//private void AttemptToSpawnPiece(int col, CellResult[,] cellResults, bool erode = false) {
//	PieceModel piece;
//	while (true) {
//		piece = SpawnPiece(new Point(0, col));
//
//		// Attempt to move piece DOWN
//		if (AttemptToMovePiece (gameBoard [0, col], piece, cellResults, erode)) {
//			continue;
//		}
//		if (erode) {
//			// Attempt to move piece LEFT
//			if (col > 0) {
//				if (AttemptToMovePiece (gameBoard [0, col - 1], piece, cellResults, erode)) {
//					continue;
//				}
//			}
//			// Attempt to move piece RIGHT
//			if (col < gameBoard.GetLength (1) - 1) {
//				if (AttemptToMovePiece (gameBoard [0, col + 1], piece, cellResults, erode)) {
//					continue;
//				}
//			}
//		}
//		break;
//	}
//}