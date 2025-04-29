using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinalScreenDecision : MonoBehaviour
{   
    public Animator transition;
    public float transitionTime = 1f;

    public void quit(){
        Debug.Log("Exiting Application...");
        Application.Quit();
    }

    public void goHome(){
        StartCoroutine(LoadNextScene(0));
    }

    public void goBack(){
        StartCoroutine(LoadNextScene(SceneManager.GetActiveScene().buildIndex - 1));
    }
    IEnumerator LoadNextScene(int sceneIndex)
    {

        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }
}