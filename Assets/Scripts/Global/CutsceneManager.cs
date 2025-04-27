using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Add this line to fix the error

[RequireComponent(typeof(Animator))]
public class CutsceneManager : MonoBehaviour
{
    [Tooltip("Name of scene to load after cutscene")]
    public string nextSceneName = "A1_S1";

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(WaitForAnimation());
    }

    IEnumerator WaitForAnimation()
    {
        // Wait for animation to start
        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            yield return null;
        }

        // Wait for animation to complete
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        SceneManager.LoadScene(nextSceneName);
    }

    // Optional skip with any key
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}