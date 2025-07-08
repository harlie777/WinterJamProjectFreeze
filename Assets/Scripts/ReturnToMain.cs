using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Application.LoadLevel(0);
    }

}
