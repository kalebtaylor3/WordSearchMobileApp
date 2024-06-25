using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Xml;
using System.Drawing;
using Unity.VisualScripting;
using System;

public class WordSearchGenerator : MonoBehaviour
{
    public int gridSize = 6; // Set the grid size
    public int numberOfWords = 10;
    public GameObject letterPrefab;
    public GameObject displayWord;
    public Vector3 letterSpacing = new Vector3(1.0f, 1.0f, 0.0f); // Adjust letter spacing
    public string wordDatabaseFilePath;
    public string foodCategory = "Assets/FoodDataBase.txt";

    private char[,] grid;
    private List<string> words;

    public Transform wordGrid;
    public Transform wordCollection;
    public Camera mainCamera;

    [HideInInspector] public List<string> wordsToDisplay;

    public List<GameObject> wordsToFindGameobject;

    public GameObject wordsToFindUI;
    public GameObject mainMenu;
    public GameObject CategoryUI;
    public GameObject timer;

    public CategoryButton currentCat;


    public static event Action OnCategoryComplete;

    void Start()
    {
        //ClearPuzzle();
        //CreateNewPuzzle(gridSize, numberOfWords, foodCategory);
        mainMenu.SetActive(true);
        CategoryUI.SetActive(false);
    }

    public void CreateNewPuzzle(int size, int number, string cateogry, CategoryButton categoryProg, int time)
    {
        gridSize = size;
        numberOfWords = number;
        wordDatabaseFilePath = cateogry;
        wordsToFindUI.SetActive(true);
        mainMenu.SetActive(false);
        CategoryUI.SetActive(false);
        currentCat = categoryProg;

        LoadWordDatabase();
        InitializeGrid();
        PlaceWords();
        FillRandomLetters();
        DisplayGridWithUI();
        DisplayWordsToFind();
        AdjustCameraAndGridPosition();
        timer.SetActive(true);
        CountdownTimer.Instance.StartTimer(time);
    }

    public void ClearPuzzle()
    {
        if (words.Count > 0)
        {
            words.Clear();
            for (int i = 0; i < wordGrid.childCount; i++)
            {
                Transform child = wordGrid.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        if (wordsToFindGameobject.Count > 0)
        {
            wordsToFindGameobject.Clear();
            for (int i = 0; i < wordCollection.childCount; i++)
            {
                Transform child = wordCollection.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        if (wordsToDisplay.Count > 0)
            wordsToDisplay.Clear();

        timer.SetActive(false);
        CountdownTimer.Instance.StopTimer();
    }

    void LoadWordDatabase()
    {
        words = new List<string>();
        wordsToDisplay = new List<string>();

        // Attempt to load the text asset from the "Resources" folder
        TextAsset textAsset = Resources.Load<TextAsset>(wordDatabaseFilePath); // Replace with your text file name

        if (textAsset != null)
        {
            // Split lines on both '\n' and '\r' to remove extra spaces
            string[] wordLines = textAsset.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Trim each line to remove leading and trailing whitespace
            for (int i = 0; i < wordLines.Length; i++)
            {
                wordLines[i] = wordLines[i].Trim();
            }

            words.AddRange(wordLines);
        }
        else
        {
            Debug.LogError("Failed to load the word database text file from Resources.");
        }
    }

    void InitializeGrid()
    {
        grid = new char[gridSize, gridSize];
    }

    void PlaceWords()
    {
        for (int i = 0; i < numberOfWords; i++)
        {
            bool placed = false;
            while (!placed)
            {
                int startX = UnityEngine.Random.Range(0, gridSize);
                int startY = UnityEngine.Random.Range(0, gridSize);
                int direction = UnityEngine.Random.Range(0, 8);

                string selectedWord = GetRandomWord();

                if (CanPlaceWord(selectedWord, startX, startY, direction))
                {
                    PlaceWord(selectedWord, startX, startY, direction);
                    Debug.Log("Selected word: " + selectedWord);
                    wordsToDisplay.Add(selectedWord);

                    placed = true;
                }
            }
        }
    }

    bool CanPlaceWord(string word, int startX, int startY, int direction)
    {
        int wordLength = word.Length;
        int x = startX;
        int y = startY;

        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] dy = { 1, 1, 0, -1, -1, -1, 0, 1 };

        for (int i = 0; i < wordLength; i++)
        {
            if (x < 0 || x >= gridSize || y < 0 || y >= gridSize || grid[x, y] != '\0')
                return false;
            x += dx[direction];
            y += dy[direction];
        }

        return true;
    }

    void PlaceWord(string word, int startX, int startY, int direction)
    {
        int x = startX;
        int y = startY;

        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] dy = { 1, 1, 0, -1, -1, -1, 0, 1 };

        for (int i = 0; i < word.Length; i++)
        {
            grid[x, y] = word[i];
            x += dx[direction];
            y += dy[direction];
        }
    }

    void FillRandomLetters()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j] == '\0')
                {
                    grid[i, j] = GetRandomLetter();
                }
            }
        }
    }

    void DisplayGridWithUI()
    {
        // Calculate the width and height of each cell based on the screen dimensions and grid size
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float cellSizeX = screenWidth / gridSize;
        float cellSizeY = screenHeight / gridSize;

        // Adjust letter spacing based on the cell size
        Vector3 adjustedLetterSpacing = new Vector3(cellSizeX, cellSizeY, 0.0f);

        // Update the letter spacing for the grid
        letterSpacing = adjustedLetterSpacing;

        GridLayoutGroup gridLayout = wordGrid.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSize;
        gridLayout.cellSize = new Vector2(cellSizeX, cellSizeY);

        // Calculate camera size to fit the grid
        float gridWidth = gridSize * cellSizeX;
        float gridHeight = gridSize * cellSizeY;
        float aspectRatio = screenWidth / screenHeight;
        float requiredOrthoSize = Mathf.Max(gridWidth / 2, gridHeight / 2) / aspectRatio;

        // Set the camera's orthographic size
        mainCamera.orthographicSize = requiredOrthoSize;

        // Reposition the grid to fit within the screen
        Vector3 gridPosition = new Vector3(screenWidth / 2, screenHeight / 2, 0);
        wordGrid.transform.position = gridPosition;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject letter = Instantiate(letterPrefab, wordGrid);
                TMP_Text textMesh = letter.GetComponentInChildren<TMP_Text>();
                textMesh.text = grid[i, j].ToString();
            }
        }
    }

    void AdjustCameraAndGridPosition()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate the desired cell size based on screen dimensions and grid size
        float cellSizeX;
        if (gridSize <=8)
            cellSizeX = screenWidth / gridSize;
        else
            cellSizeX = screenWidth / (gridSize * 2);
        float cellSizeY = screenHeight / gridSize;

        // Calculate the grid layout spacing based on the desired cell size with minimum values
        float spacingX;


            if (gridSize <= 8)
            spacingX = Mathf.Max(150.0f, cellSizeX / 5); // Minimum of 150 units
        else
            spacingX = Mathf.Max(200, cellSizeX / 5); // Minimum of 150 units
        float spacingY = Mathf.Max(-60, cellSizeY / 20); // Minimum of 50 units

        // Update the grid layout spacing
        GridLayoutGroup gridLayout = wordGrid.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSize;
        gridLayout.cellSize = new Vector2(cellSizeX, cellSizeY);
        gridLayout.spacing = new Vector2(spacingX, spacingY);

        // Calculate the camera's orthographic size to fit the entire grid
        float gridWidth = gridSize * (cellSizeX + spacingX);
        float gridHeight = gridSize * (cellSizeY + spacingY);
        float aspectRatio = screenWidth / screenHeight;

        // Adjust the camera orthographic size (1.5x higher)
        float requiredOrthoSize = Mathf.Max(gridWidth / 2, gridHeight / 2) / aspectRatio * 1.5f;

        // Add some padding to the orthographic size
        float padding = 1.0f; // Adjust as needed
        mainCamera.orthographicSize = requiredOrthoSize + padding;

        // Calculate a position that centers the grid on the screen
        Vector3 gridPosition = new Vector3(screenWidth / 2, screenHeight / 2, 0);
        wordGrid.transform.position = gridPosition;
    }

    void DisplayWordsToFind()
    {
        for (int j = 0; j < wordsToDisplay.Count; j++)
        {
            GameObject word = Instantiate(displayWord, wordCollection);
            TMP_Text textMesh = word.GetComponent<TMP_Text>();
            textMesh.text = wordsToDisplay[j];
            wordsToFindGameobject.Add(word);
        }
    }


    char GetRandomLetter()
    {
        return (char)('A' + UnityEngine.Random.Range(0, 26));
    }

    string GetRandomWord()
    {
        if (words.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, words.Count);
            string randomWord = words[randomIndex];
            words.RemoveAt(randomIndex);
            return randomWord;
        }
        else
        {
            Debug.LogError("No words available in the database. Word count: " + words.Count);
            return string.Empty;
        }
    }

    public void UpdateWordsToFind(string foundWord)
    {
        for (int i =0; i < wordsToDisplay.Count; i++)
        {
            if (wordsToDisplay[i] == foundWord)
                wordsToDisplay.Remove(foundWord);

            for(int j = 0; j < wordsToFindGameobject.Count; j++)
            {
                if (wordsToFindGameobject[j].GetComponent<TMP_Text>().text == foundWord)
                {
                    string text = wordsToFindGameobject[j].GetComponent<TMP_Text>().text;
                    text = "<s>" + text + "</s>";
                    wordsToFindGameobject[j].GetComponent<TMP_Text>().text = text;

                    if (wordsToDisplay.Count <= 0)
                    {
                        Debug.Log("Puzzle Complete!");
                        CountdownTimer.Instance.StopTimer();
                        ClearPuzzle();
                        currentCat.currentPuzzle += 1;
                        PlayerPrefs.SetInt(currentCat.CateogryName, currentCat.currentPuzzle);
                        PlayerPrefs.Save();
                        //load category menu again

                        if (currentCat.currentPuzzle == currentCat.maxPuzzles)
                        {
                            OnCategoryComplete?.Invoke();
                        }

                    }
                }
            }
        }
    }

}