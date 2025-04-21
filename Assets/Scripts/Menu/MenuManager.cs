using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Animation")]
    public Animator menuAnimator;
    public string menuAnimationName = "MenuAnimation";

    [Header("Buttons")]
    public Button[] buttons;
    public float buttonEnableDelay = 0.5f;

    [Header("Fade Transition")]
    public Image fadePanel; 
    public float fadeDuration = 1.0f;

    private bool menuAnimationFinished = false;

    void Start()
    {
        SetButtonsInteractable(false);
        if (menuAnimator != null)
        {
            menuAnimator.Play(menuAnimationName);
        }
    }

    void Update()
    {
        if (!menuAnimationFinished && menuAnimator != null)
        {
            if (menuAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                menuAnimationFinished = true;
                PauseAnimator(menuAnimator);
                StartCoroutine(EnableButtonsAfterDelay());
            }
        }
    }

    private void PauseAnimator(Animator animator)
    {
        animator.speed = 0;
    }

    private IEnumerator EnableButtonsAfterDelay()
    {
        yield return new WaitForSeconds(buttonEnableDelay);
        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool state)
    {
        foreach (Button button in buttons)
        {
            button.interactable = state;
        }
    }

    public void OnStartButtonClicked()
    {
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        SetButtonsInteractable(false);

        fadePanel.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color startColor = fadePanel.color;
        startColor.a = 0f;
        fadePanel.color = startColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene("A1_S1");
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}