using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private int mainMenuLevel = 0;
    [SerializeField] private int currentlevel;

    private void Update()
    {
        
    }

    private void CheckLevel()
    {

    }

    private void ToNextLevel()
    {
        m_instance.currentlevel++;
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
