using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance = null;
    public static GameManager Instance
    {
        get
        {
            //Look for Game Manager instance
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<GameManager>();

                //If Game Manager instance could not be found, make a new one
                if (m_Instance == null)
                {
                    GameObject newGameManager = new GameObject("GameManager");
                    m_Instance = newGameManager.AddComponent<GameManager>();
                }
                //Keep the Instance alive throughout the project
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    [SerializeField] private int mainMenuLevel = 0;
    [SerializeField] private int currentlevel = 0;
    [SerializeField] private Vector2 playerSavedPosition;

    private GameObject pauseMenu;
    private GameObject playerCharacter;

    public int CurrentLevel { get { return currentlevel; } }

    private void Awake()
    {
        pauseMenu = GameObject.FindWithTag("PauseMenu");
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        //playerCharacter = GameObject.FindWithTag("Player");
        //if (playerCharacter != null)
        //{
        //    playerCharacter.transform.position = m_Instance.playerSavedPosition;
        //}

        Debug.Log("Awoken");
    }

    private void Update()
    {
        //PauseGame();
        //ResetScene();
    }

    private void PauseGame()
    {
        if (currentlevel == 0 || m_Instance.currentlevel == 0 )
            return;

        if (pauseMenu == null)
        {
            pauseMenu = GameObject.FindWithTag("PauseMenu");
        }

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
            SceneManager.LoadScene(m_Instance.currentlevel);
        }
    }

    private void CheckLevel()
    {

    }

    public void ToNextLevel(int nextSection)
    {
        if (playerCharacter == null)
        {
            playerCharacter = GameObject.FindWithTag("Player");
        }

        m_Instance.currentlevel = nextSection;
        SceneManager.LoadScene(m_Instance.currentlevel);
    }
}
