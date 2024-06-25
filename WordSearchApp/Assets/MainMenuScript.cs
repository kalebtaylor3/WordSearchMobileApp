using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject categoriesMenu;
    public void Play()
    {
        mainMenu.SetActive(false);
        categoriesMenu.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
