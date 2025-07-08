using Cinemachine;
using SmallHedge.SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public static Cutscene instance;    
    public CinemachineVirtualCamera gameCam;
    private bool cutsceneCanStart;
    bool cutSceneStarted;

    private CinemachineVirtualCamera cutsceneCam;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (cutsceneCam != null)
            cutsceneCanStart = cutsceneCam.transform.position == Camera.main.transform.position;


        if (cutsceneCanStart && !cutSceneStarted)
        {
            cutSceneStarted = true;
            StartCoroutine(CutScene());
        }

    }

    public void StartCutscene(CinemachineVirtualCamera cutSceneCamera, Animator playerAnimator)
    {
        cutsceneCam = cutSceneCamera;
        animator = playerAnimator;

        gameCam.gameObject.SetActive(false);
        cutsceneCam.gameObject.SetActive(true);

        Debug.Log("Out of moves! You died.");
        MusicManager.instance.SwitchMusicState(MusicState.GAMEOVER);
    }


    IEnumerator CutScene()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Attack", true);
        //Wait for time to stab
        yield return new WaitForSeconds(1.01f);
        Debug.Log("Stabbed");
        SoundManager.PlaySound(SoundType.PLAYER_DEATH);
        yield return new WaitForSeconds(2.5f);
        Application.LoadLevel(0);
    }
}
