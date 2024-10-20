using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManger : MonoBehaviour
{
    private static GameManger m_instance = null;
    public static GameManger Instance
    {
        get
        {
            //Look for Game Manager instance
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManger>();

                //If Game Manager instance could not be found, make a new one
                if (m_instance == null)
                {
                    GameObject newGameManager = new GameObject("GameManager");
                    newGameManager.AddComponent<GameManger>();
                }
                //Keep the Instance alive throughout the project
                DontDestroyOnLoad(m_instance.gameObject);
            }
            return m_instance;
        }
    }

    [SerializeField] private int mainMenuLevel;
    [SerializeField] private int currentlevel;
    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        pauseMenu = GameObject.FindWithTag("PauseMenu");
        pauseMenu.SetActive(false);
    }

    private void Update()
    {

        PauseGame();
    }

    private void PauseGame()
    {
        //if (m_instance.currentlevel == 0)
        //    return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
            }
            else if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
            }
        }
    }

    private void CheckLevel()
    {

    }

    public void ToNextLevel()
    {
        m_instance.currentlevel++;
        SceneManager.LoadScene(m_instance.currentlevel);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
