using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Constants {

    public enum SwipeDirection
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    };

    public static SwipeDirection GetOppositeDirection(SwipeDirection a)
    {
        if(a == SwipeDirection.UP)
            return SwipeDirection.DOWN;
        if (a == SwipeDirection.RIGHT)
            return SwipeDirection.LEFT;
        if (a == SwipeDirection.DOWN)
            return SwipeDirection.UP;
        if (a == SwipeDirection.LEFT)
            return SwipeDirection.RIGHT;
        return SwipeDirection.DOWN;
    }

    public static Vector2 GetNextRowCol(SwipeDirection swipeDirection , int row , int col)
    {
        Vector2 retVect = new Vector2(row, col);
        if (swipeDirection == SwipeDirection.UP)
        {
            retVect.x -= 1;
        }
        else if (swipeDirection == SwipeDirection.RIGHT)
        {
            retVect.y += 1;
        }
        else if (swipeDirection == SwipeDirection.DOWN)
        {
            retVect.x += 1;
        }
        else {
            retVect.y -= 1;
        }
        return retVect;
    }

    public class ShapeGridLocation
    {
        public int ROW;
        public int COL;

        public ShapeGridLocation(int _row, int _col)
        {
            this.ROW = _row;
            this.COL = _col;
        }
        public void SetRow(int _row)
        {
            this.ROW = _row;
        }
        public void SetCol(int _col)
        {
            this.COL = _col;
        }
        public void SetRowCol(int _row, int _col)
        {
            this.ROW = _row;
            this.COL = _col;
        }

    }

    //Animation times
    public static readonly float DEFAULT_SWAP_ANIMATION_DURATION = .3f;

    //Point values
    public static readonly int NORMAL_SHAPE_VALUE = 10;

    // Size of images
	public static readonly int CELL_PADDING_FULL = 4;
	public static readonly int MAX_NUMBER_OF_GRID_ITEMS = 7;
    public static readonly int MIN_SIZE = 30;
    public static readonly float MAX_SIZE = 75f;

    //Events
    public static readonly string SHAPES_CREATED = "shapes_created";
    public static readonly string SWIPE_RIGHT_EVENT = "swipe_right_event";
    public static readonly string SWIPE_LEFT_EVENT = "swipe_left_event";
    public static readonly string SWIPE_UP_EVENT = "swipe_up_event";
    public static readonly string SWIPE_DOWN_EVENT = "swipe_down_event";
    public static readonly string SWIPE_MOVED_EVENT = "swipe_moved_event";
    public static readonly string SWIPE_ENDED_EVENT = "swipe_ended_event";
    public static readonly string SWIPE_BEGAN_EVENT = "swipe_began_event";

    public static readonly string LEVEL_LOAD_BEGIN_EVENT = "level_began_loading";
    public static readonly string LEVEL_LOAD_END_EVENT = "level_ended_loading";
    public static readonly string LEVEL_START_EVENT = "level_start_event";
    public static readonly string LEVEL_END_EVENT = "level_end_event";

    public static readonly string SWAP_RESULT_FAILURE = "swipe_result_failure";
    public static readonly string SWAP_RESULT_SUCCESS = "swipe_result_success";

    public static readonly string ABILITY1 = "ability1";
    public static readonly string UNSUBSCRIBE_ALL_PIECES_FROM_ABILITY_1 = "unsub_pieces_from_ability_1";

    //Device Rotation Event
    public static readonly string ON_RECT_TRANSFORM_DIMENSIONS_CHANGE = "OnRectTransformDimensionsChange";

    // Modal UI Game Object Names
    public static readonly string UI_Message_Modal = "Message_UI";
    public static readonly string UI_Level = "Level_UI";
    public static readonly string UI_Store_Modal = "Store_UI";
    public static readonly string UI_Settings_Modal = "Settings_UI";
    public static readonly string UI_Board_Modal = "Board_UI";

    //Product IDs
    public static readonly string PRODUCT_GOLD_10 = "gold_10";
    public static readonly string PRODUCT_GOLD_50 = "gold_50";
    public static readonly string PRODUCT_GOLD_110 = "gold_110";
    public static readonly string PRODUCT_GOLD_250 = "gold_250";
    public static readonly string PRODUCT_GOLD_750 = "gold_750";

    //Settings
    public static readonly string SETTING_ENABLE_SOUNDS = "enable_sounds";
    public static readonly string SETTING_ENABLE_MUSIC = "enable_music";
    public static readonly string SETTING_ENABLE_PUSH_NOTIFICATION = "enable_notifications";

	//Game Events
	public static readonly string GAME_OVER = "game_over";

    //Match Sound File Names
    public static readonly string MATCH_NORMAL = "match_normal";
    public static readonly string MATCH_ALL = "match_all";
    public static readonly string MATCH_ROW = "match_row_or_col";
    public static readonly string MATCH_COL = "match_row_or_col";
    public static readonly string MATCH_BOMB = "match_bomb";

    //Piece ID Mapping
    public static Dictionary<string, PieceColor> PieceIDMapping = new Dictionary<string, PieceColor>() {
		{ "b", PieceColor.BLUE },
		{ "g", PieceColor.GREEN },
		{ "o", PieceColor.ORANGE },
		{ "i", PieceColor.PINK },
		{ "p", PieceColor.PURPLE },
        { "f", PieceColor.FROSTING }
    };

	public enum PieceColor 
	{
		PINK,
		ORANGE,
		GREEN,
		PURPLE,
		BLUE,
        YELLOW,
        FROSTING,
		NULL
	}

	public enum PieceType 
	{
		NORMAL,
		STRIPED_ROW,
        STRIPED_COL,
        BOMB,
        FROSTING,
		NULL
	}	
    
}
