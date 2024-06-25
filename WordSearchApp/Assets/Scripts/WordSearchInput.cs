using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class WordSearchInput : MonoBehaviour
{
    public LayerMask letterLayer; // Set this layer to the layer of your letter prefab.
    public Color highlightColor; // Set the color for highlighting selected letters.
    public Color correctWordColor; // Set the color for correctly found words.
    public Color incorrectWordColor; // Set the color for incorrectly found words.

    private List<GameObject> selectedLetters = new List<GameObject>();
    private List<string> foundWords = new List<string>();
    public GraphicRaycaster raycaster;

    public WordSearchGenerator wordSearch;

    private bool isDragging = false; // Flag to track drag state

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                HandleTouchStart(touch);
                isDragging = true; // Set the flag to indicate dragging.
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                HandleTouchDrag(touch);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false; // Reset the dragging flag.
                HandleTouchEnd();
            }
        }
    }

    void HandleTouchStart(Touch touch)
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem not found. Ensure that you have an EventSystem in your scene.");
            return;
        }

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = touch.position;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            GameObject letterObj = results[0].gameObject;

            if (!selectedLetters.Contains(letterObj))
            {
                selectedLetters.Add(letterObj);
                // Animate the letter's scale for highlighting.
                AnimateLetterSelection(letterObj, true);
            }
        }
    }

    void AnimateLetterSelection(GameObject letterObj, bool select)
    {
        // Use DOTween to animate the scale.
        float scaleFactor = select ? 0.55f : 0.4f;
        Image background = letterObj.GetComponentInChildren<Image>();

        if (background != null)
        {
            Sequence scaleAnimation = DOTween.Sequence();
            scaleAnimation.Append(letterObj.transform.DOScale(Vector3.one * scaleFactor, 0.2f));
        }
        else
        {
            Debug.LogWarning("No Image component found in children of " + letterObj.name);
        }
    }

    void HandleTouchDrag(Touch touch)
    {
        // Handle the drag by continuously selecting letters.
        HandleTouchStart(touch);
    }

    void HandleTouchEnd()
    {
        string selectedWord = GetSelectedWord();
        if (!string.IsNullOrEmpty(selectedWord))
        {
            foundWords.Add(selectedWord);
            
            Debug.Log("Found Word: " + selectedWord);

            if (wordSearch.wordsToDisplay.Contains(selectedWord))
            {
                // Animate the discovered word with color change.
                AnimateWordDiscovery(selectedLetters);
            }
            else
            {
                // Animate the selected letters with incorrect animation.
                AnimateIncorrectWord(selectedLetters);
            }

            ClearSelectedLetters(true);
            wordSearch.UpdateWordsToFind(selectedWord);
        }
        else
        {
            // Reset the letter state to normal when input is released and it's not a correct word.
            ResetLetterState(selectedLetters);

            selectedLetters.Clear();
        }
    }

    void AnimateWordDiscovery(List<GameObject> wordLetters)
    {
        Sequence wordAnimation = DOTween.Sequence();

        // Animate each letter to be green in unison with the animation of each letter.
        foreach (GameObject letterObj in wordLetters)
        {
            Image background = letterObj.GetComponentInChildren<Image>();
            if (background != null)
            {
                wordAnimation.Append(letterObj.transform.DOScale(Vector3.one * 0.6f, 0.1f));
                wordAnimation.Join(background.DOColor(correctWordColor, 0.1f));
                wordAnimation.Append(letterObj.transform.DOScale(Vector3.one * 0.4f, 0.1f));
            }
        }

        // You can also fade out the letters if needed.
        wordAnimation.Join(wordLetters[0].GetComponent<Image>().DOFade(0f, 0.3f));

        // Remove the letters once the animation is complete.
        wordAnimation.OnComplete(() =>
        {
            ResetLetterState(wordLetters);
        });

        foreach (GameObject letterObj in selectedLetters)
        {
            letterObj.GetComponent<NonDrawingGraphic>().raycastTarget = false;
        }
    }


    void AnimateIncorrectWord(List<GameObject> wordLetters)
    {
        Sequence wordAnimation = DOTween.Sequence();

        foreach (GameObject letterObj in wordLetters)
        {
            Image background = letterObj.GetComponentInChildren<Image>();
            if (background != null)
            {
                float initialScale = .4f;
                float strobeScale = .55f;

                wordAnimation.Append(letterObj.transform.DOScale(Vector3.one * strobeScale, 0.025f));
                wordAnimation.Join(background.DOColor(incorrectWordColor, 0.025f));
                wordAnimation.Append(letterObj.transform.DOScale(Vector3.one * initialScale, 0.025f));
                wordAnimation.Append(background.DOColor(highlightColor, 0.025f));
            }
        }

        // Reset the letter state to normal when the animation is complete.
        wordAnimation.OnComplete(() =>
        {
            ResetLetterState(wordLetters);
        });
    }


    string GetSelectedWord()
    {
        // Combine the selected letters to check if they form any word in wordsToDisplay.
        string selectedLettersString = "";
        foreach (GameObject letterObj in selectedLetters)
        {
            TMP_Text textMesh = letterObj.GetComponentInChildren<TMP_Text>();
            selectedLettersString += textMesh.text;
        }

        return selectedLettersString;
    }

    void ClearSelectedLetters(bool found)
    {
        foreach (GameObject letterObj in selectedLetters)
        {
            AnimateLetterSelection(letterObj, false);
        }

        selectedLetters.Clear();
    }

    void ResetLetterState(List<GameObject> letters)
    {
        foreach (GameObject letterObj in letters)
        {
            AnimateLetterSelection(letterObj, false);
        }
    }
}
