using System;

	public class PieceModel
	{
		private PieceColor color;
		private PieceType type;

		public PieceModel (PieceType type, PieceColor color)
		{
			this.type = type;
			this.color = color;
		}
		// GETTERS

		public PieceColor GetColor () {
			return this.color;
		}

		public PieceType GetPieceType () {
			return this.type;
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


