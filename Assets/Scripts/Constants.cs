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
    public static readonly float DEFAULT_SWAP_ANIMATION_DURATION = .7f;

    //Point values
    public static readonly int NORMAL_SHAPE_VALUE = 10;

    // Size of images
    public static readonly int SIZE_MARGIN = 30;

    //Events
    public static readonly string SHAPES_CREATED = "shapes_created";
    public static readonly string SWIPE_RIGHT_EVENT = "swipe_right_event";
    public static readonly string SWIPE_LEFT_EVENT = "swipe_left_event";
    public static readonly string SWIPE_UP_EVENT = "swipe_up_event";
    public static readonly string SWIPE_DOWN_EVENT = "swipe_down_event";
    public static readonly string SWIPE_MOVED_EVENT = "swipe_moved_event";
    public static readonly string SWIPE_ENDED_EVENT = "swipe_ended_event";
    public static readonly string SWIPE_BEGAN_EVENT = "swipe_began_event";

    public static readonly string LEVEL_BEGAN_LOADING_EVENT = "level_began_loading";
    public static readonly string LEVEL_ENDED_LOADING_EVENT = "level_ended_loading";
    public static readonly string LEVEL_START_EVENT = "level_start_event";
    public static readonly string LEVEL_END_EVENT = "level_end_event";

    public static readonly string CHECK_MATCH = "check_match";

    //Piece Animations
    public static readonly string ANIMATE = "piece_animate";
    public static readonly string ANIMATE_UP = "piece_animate_up";
    public static readonly string ANIMATE_RIGHT = "piece_animate_right";
    public static readonly string ANIMATE_DOWN = "piece_animate_down";
    public static readonly string ANIMATE_LEFT = "piece_animate_left";
    public static readonly string ANIMATE_END = "piece_animate_end";

    // Modal UI Game Object Names
    public static readonly string UI_Message_Modal = "Message_UI";
    public static readonly string UI_Level = "Level_UI";
    public static readonly string UI_Store_Modal = "Store_UI";
    public static readonly string UI_Settings_Modal = "Settings_UI";

    //Product IDs
    public static readonly string PRODUCT_GOLD_10 = "gold_10";
    public static readonly string PRODUCT_GOLD_50 = "gold_50";
    public static readonly string PRODUCT_GOLD_110 = "gold_110";
    public static readonly string PRODUCT_GOLD_250 = "gold_250";
    public static readonly string PRODUCT_GOLD_750 = "gold_750";

    //Piece ID Mapping
    public static Dictionary<string, string> pieceIDMapping = new Dictionary<string, string>() {
        { "b", "blue" },
        { "g", "green" },
        { "o", "orange" },
        { "r", "red" },
        { "y", "yellow" }
    };

    
}
