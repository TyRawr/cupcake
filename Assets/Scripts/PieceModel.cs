using System;

namespace AssemblyCSharp
{
	public class PieceModel
	{
		private PieceColor color;
		private PieceType type;

		public PieceModel (PieceType type, PieceColor color)
		{
			this.type = type;
			this.color = color;
		}
	}

	public enum PieceColor 
	{
		PINK,
		ORANGE,
		GREEN,
		PURPLE,
		YELLOW
	}

	public enum PieceType 
	{
		NORMAL,
		STRIPED,
		DOTTED,
		CANDLE,
	}	

	// GETTERS

	public PieceColor GetColor () {
		return this.color;
	}
	public PieceType GetType () {
		return this.type;
	}
}

