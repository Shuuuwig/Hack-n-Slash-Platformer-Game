using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance = null;
    public static GameManager Instance
    {
        get
        {
            //Look for Game Manager instance
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();

                //If Game Manager instance could not be found, make a new one
                if (m_instance == null)
                {
                    GameObject newGameManager = new GameObject("GameManager");
                    newGameManager.AddComponent<GameManager>();
                }
                //Keep the Instance alive throughout the project
                DontDestroyOnLoad(m_instance.gameObject);
            }
            return m_instance;
        }
    }

    [SerializeField] private int mainMenuLevel = 0;
    [SerializeField] private int currentlevel = 0;
    [SerializeField] private GameObject pauseMenu;

    public int CurrentLevel { get { return currentlevel; } }

    private void Awake()
    {
        pauseMenu = GameObject.FindWithTag("PauseMenu");
        if (pauseMenu != null )
        {
            pauseMenu.SetActive(false);
        }
        
        
    }

    private void Update()
    {
        PauseGame();
        ResetScene();
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

    private void ResetScene()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            SceneManager.LoadScene(currentlevel);
        }
    }

    private void CheckLevel()
    {

    }

    public void StartGame()
    {
        currentlevel++;
        SceneManager.LoadScene(currentlevel);
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
