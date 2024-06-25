using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public delegate void EnableSquareSelection();
    public static event EnableSquareSelection OnEnableSquareSelection;

    public static void EnableSquareSelectionMethod()
    {
        if (OnEnableSquareSelection != null)
            OnEnableSquareSelection();
    }


    public delegate void DisableSquareSelection();
    public static event DisableSquareSelection OnDisableSquareSelection;

    public static void DisableSquareSelectionMethod()
    {
        if (OnDisableSquareSelection != null)
            OnDisableSquareSelection();
    }

    public delegate void SelectSquare(GameObject Letter);
    public static event SelectSquare OnSelectSquare;

    public static void SelectSquareMethod(GameObject Letter)
    {
        if (OnSelectSquare != null)
            OnSelectSquare(Letter);
    }

    public delegate void CheckSquare();
    public static event CheckSquare OnCheckSquare;

    public static void CheckSquareMethod()
    {
        if (OnCheckSquare != null)
            OnCheckSquare();
    }

    public delegate void ClearSelection();
    public static event ClearSelection OnClearSelection;

    public static void ClearSelectionMethod()
    {
        if (OnClearSelection != null)
            OnClearSelection();
    }
}
