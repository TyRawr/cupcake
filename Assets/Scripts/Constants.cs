using UnityEngine;
using System.Collections;

public static class Constants {

    public enum SwipeDirection
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    };

    //Point values
    public static readonly int NORMAL_SHAPE_VALUE = 10;

    // Size of images
    public static readonly int SIZE_WIDTH = 50;
    public static readonly int SIZE_HEIGHT = 50;

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

    //Piece Animations
    public static readonly string ANIMATE = "piece_animate";
    public static readonly string ANIMATE_UP = "piece_animate_up";
    public static readonly string ANIMATE_RIGHT = "piece_animate_right";
    public static readonly string ANIMATE_DOWN = "piece_animate_down";
    public static readonly string ANIMATE_LEFT = "piece_animate_left";
}
