using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource Area1Bgm;
    public static AudioManager instance;
    private GameManager GameManager;

    private void Awake()
    {
        GameManager = GetComponent<GameManager>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Update()
    {
        if (GameManager.Instance.CurrentLevel >= 1)
        {
            if (Area1Bgm.isPlaying == false)
            {
                Area1Bgm.Play();
                Debug.Log("Playing BGM1");
            }
        }
        else
        {
            Area1Bgm.Stop();
        }
    }
}
