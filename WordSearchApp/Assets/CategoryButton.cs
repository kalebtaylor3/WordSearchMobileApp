using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{

    public WordSearchGenerator wordSearch;
    public string category;

    public int maxPuzzles;
    public int currentPuzzle;
    public TMP_Text prog;
    public Slider slide;

    public GameObject lockImage;

    public Button theButton;

    public string CateogryName;

    public void OnClick()
    {
        if (currentPuzzle < maxPuzzles)
        {
            System.Random random = new System.Random();
            int param1 = random.Next(6, 11);  // Random integer between 6 and 10

            // Calculate param2 and time based on param1
            int param2;
            int time;

            if (param1 == 6)
            {
                param2 = 4;
            }
            else if (param1 == 7)
            {
                param2 = 5;
            }
            else if (param1 == 8)
            {
                param2 = 6;
            }
            else if (param1 == 9)
            {
                param2 = 7;
            }
            else // param1 == 10
            {
                param2 = 8;
            }

            // Calculate time based on param2
            float minTime = 30f;
            float maxTime = 120f;
            float normalizedTime = Mathf.Clamp01((float)param2 / 8f); // Assuming 8 is the maximum value of param2.
            time = Mathf.RoundToInt(Mathf.Lerp(minTime, maxTime, normalizedTime));

            wordSearch.CreateNewPuzzle(param1, param2, category, this, time);
        }
    }

    private void OnEnable()
    {
        if (currentPuzzle == maxPuzzles)
            Debug.Log("cateogry complete");

        //it doesnt work because it will save then be reset

        currentPuzzle = PlayerPrefs.GetInt(CateogryName, 0);
        prog.text = currentPuzzle + "/" + maxPuzzles;
        slide.value = currentPuzzle;
        slide.maxValue = maxPuzzles;

    }

}
