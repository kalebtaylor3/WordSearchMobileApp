using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryManager : MonoBehaviour
{
    public List<CategoryButton> categories;
    public int currentLevel = 0;

    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0); // 0 is the default value if "CurrentLevel" is not found

        for(int i = 0; i < currentLevel; i++)
        {
            categories[currentLevel].lockImage.SetActive(false);
            categories[currentLevel].theButton.interactable = true;
        }
    }

    private void OnEnable()
    {
        WordSearchGenerator.OnCategoryComplete += UnlockCategory;
    }

    void UnlockCategory()
    {
        currentLevel += 1;
        categories[currentLevel].lockImage.SetActive(false);
        categories[currentLevel].theButton.interactable = true;

        // Save the currentLevel to PlayerPrefs
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
    }
}
