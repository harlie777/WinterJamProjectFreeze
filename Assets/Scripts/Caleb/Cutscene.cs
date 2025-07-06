using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public CinemachineVirtualCamera gameCam;
    public CinemachineVirtualCamera cutsceneCam;
    public Camera cam;
    public bool cutsceneCanStart;
    public Animator animator;
    bool cutSceneStarted;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.C))
        {
            gameCam.gameObject.SetActive(false);
            cutsceneCam.gameObject.SetActive(true);
            
        }

        cutsceneCanStart = cutsceneCam.transform.position == cam.transform.position;
        if (cutsceneCanStart && !cutSceneStarted)
        {
            cutSceneStarted = true;
            StartCoroutine(CutScene());
        }
    }


    IEnumerator CutScene()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Attack", true);
        //Wait for time to stab
        yield return new WaitForSeconds(1.01f);
        Debug.Log("Stabbed");
    }
}
