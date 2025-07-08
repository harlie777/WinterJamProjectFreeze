using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMain : MonoBehaviour
{
    public float timeToTransistion = 5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(timeToTransistion);
        SceneManager.LoadScene(0);
        MusicManager.instance.SwitchMusicState(MusicState.TITLE_SCREEN);
    }

}
