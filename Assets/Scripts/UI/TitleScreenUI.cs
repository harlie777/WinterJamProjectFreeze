using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using SmallHedge.SoundManager;

public class TitleScreenUI : MonoBehaviour
{   
    

    public float fadeTime = 1f;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;

    public RectTransform leftToRight;
    public RectTransform rightToLeft;


    public CanvasGroup creditsCanvasGroup;
    public RectTransform creditsPanel;


    // [Header("Particle Effect")]
    // public GameObject clickParticlePrefab;

    public void Start()
    {
        // CreditsHideInstantly();
        PanelFadeIn();
        
    }

    public void PanelFadeIn()
    {
        canvasGroup.alpha = 0f;
        Vector2 targetPosMainPanel = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(targetPosMainPanel.x, -1000f);
        // rectTransform.transform.localPosition = new Vector3(0f, -1000f, 0f);
        rectTransform.DOAnchorPos(targetPosMainPanel, fadeTime, false).SetEase(Ease.OutElastic);
        canvasGroup.DOFade(1, fadeTime);

        // Animate LeftToRight: from left -> original position
        if (leftToRight != null)
        {
            Vector2 targetPos = leftToRight.anchoredPosition; // Save original position
            leftToRight.anchoredPosition = new Vector2(-2000f, targetPos.y); // Move off-screen
            leftToRight.DOAnchorPos(targetPos, fadeTime).SetEase(Ease.InOutSine);
        }

        // Animate RightToLeft: from right -> original position
        if (rightToLeft != null)
        {
            Vector2 targetPos = rightToLeft.anchoredPosition; // Save original position
            rightToLeft.anchoredPosition = new Vector2(2000f, targetPos.y); // Move off-screen
            rightToLeft.DOAnchorPos(targetPos, fadeTime).SetEase(Ease.InOutSine);
        }
    }

    // public void PanelFadeOut()
    // {
    //     canvasGroup.alpha = 1f;
    //     // Vector2 targetPosMainPanel = rectTransform.anchoredPosition;

    //     Vector2 targetPosMainPanel = rectTransform.anchoredPosition;
    //     rectTransform.anchoredPosition = new Vector2(targetPosMainPanel.x, -1000f);

    //     // rectTransform.transform.localPosition = targetPosMainPanel;
    //     rectTransform.DOAnchorPos(new Vector2(targetPosMainPanel.x, -1000f), fadeTime, false).SetEase(Ease.InOutQuint);
    //     canvasGroup.DOFade(1, fadeTime);
    // }

    // private void CreditsHideInstantly()
    // {
    //     creditsCanvasGroup.alpha = 0f;
    //     creditsCanvasGroup.interactable = false;
    //     creditsCanvasGroup.blocksRaycasts = false;
    //     // popupTransform.anchoredPosition = new Vector2(0, -1000f);
    // }


    // public void CreditsPanel()
    // {   
    //     creditsCanvasGroup.interactable = true;
    //     creditsCanvasGroup.blocksRaycasts = true;
    //     creditsCanvasGroup.alpha = 0f;
    //     Vector2 targetPosMainPanel = creditsPanel.anchoredPosition;
    //     creditsPanel.anchoredPosition = new Vector2(targetPosMainPanel.x, -1000f);
    //     // rectTransform.transform.localPosition = new Vector3(0f, -1000f, 0f);
    //     creditsPanel.DOAnchorPos(targetPosMainPanel, fadeTime, false).SetEase(Ease.OutElastic);
    //     creditsCanvasGroup.DOFade(1, fadeTime);
    // }

    public void StartGame()
    {
        MusicManager.instance.SwitchMusicState(MusicState.GAME_PLAY);
        SoundManager.PlaySound(SoundType.BUTTON);

        // //Create Feather Particles 
        // if (clickParticlePrefab != null)
        // {
        //     Vector3 worldPos = transform.position;
        //     GameObject particle = Instantiate(clickParticlePrefab, worldPos, Quaternion.identity, transform);
        //     Destroy(particle, 2f); // Optional: auto-destroy after 2 seconds
        // }


        canvasGroup.alpha = 1f;

        Vector2 targetPosMainPanel = rectTransform.anchoredPosition;
        // rectTransform.anchoredPosition = new Vector2(targetPosMainPanel.x, -1000f);
        // rectTransform.transform.localPosition = new Vector3(0f, 0f, 0f);

        // Create the fade and move tweens
        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.Join(canvasGroup.DOFade(0, fadeTime));
        fadeOutSequence.Join(rectTransform.DOAnchorPos(new Vector2(targetPosMainPanel.x, -1000f), fadeTime).SetEase(Ease.InOutQuint));

        // When both complete, load the scene
        fadeOutSequence.OnComplete(() =>
        {
            SceneManager.LoadScene("GameLevel");
        });
    }

    
    public void QuitGame()
    {
        Debug.Log("Quit pressed"); 
        Application.Quit();
        
    }
}
