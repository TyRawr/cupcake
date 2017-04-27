using System;

public class PieceModel
{
	private Constants.PieceColor color;
	private Constants.PieceType type;

	public PieceModel (string colorID, Constants.PieceType type = Constants.PieceType.NORMAL)
	{
		this.color = Constants.PieceIDMapping[colorID];
		this.type = type;
	}
	// GETTERS

	public Constants.PieceColor GetColor () {
		return this.color;
	}

	public Constants.PieceType GetPieceType () {
		return this.type;
	}
}




