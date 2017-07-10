using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public GameObject player01;
    public GameObject player02;

    public GameObject mainMenu;
    public GameObject aboutMenu;

    public GameObject dialogObjects;
    public GameObject fallingObjects;

    void Start()
    {
        mainMenu.SetActive(true);
        aboutMenu.SetActive(false);

        dialogObjects.SetActive(false);
        fallingObjects.SetActive(false);
    }

    public void OnStartPressed()
    {
        StartCoroutine(StartDialog());
    }

    public void OnAboutPressed()
    {
        mainMenu.SetActive(false);
        aboutMenu.SetActive(true);
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }

    public void OnBackPressed()
    {
        mainMenu.SetActive(true);
        aboutMenu.SetActive(false);
    }

    private IEnumerator StartDialog()
    {
        player01.GetComponent<Animator>().SetBool("Start", true);
        yield return new WaitForSeconds(0.2f);
        player02.GetComponent<Animator>().SetBool("Start", true);

        yield return new WaitForSeconds(player01.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length / 2f);

        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);

        GetComponent<AudioSource>().Stop();

        dialogObjects.SetActive(true);
        yield return new WaitForSeconds(dialogObjects.GetComponent<AudioSource>().clip.length);

        fallingObjects.SetActive(true);
        yield return new WaitForSeconds(fallingObjects.GetComponent<AudioSource>().clip.length);

        SceneManager.LoadScene("Level 1");
    }
}
