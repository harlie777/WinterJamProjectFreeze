using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//https://docs.unity3d.com/ScriptReference/AudioSource.html
public enum MusicState {
    TITLE_SCREEN,
    MAP,
    GAME_PLAY,
    GAMEOVER,
    SILENCE,
}

public class MusicManager : MonoBehaviour {
    public static MusicManager instance;
    [SerializeField]
    private MusicState musicState;
    [SerializeField]
    public List<MusicPool> musicPools;
    public MusicPool currentMusicPool;
    [Range(0,1)]
    public float musicMasterVolume = 0.5f;
    private const float defaultFadeTime = 10f;
    public float intialMenuFadeInTime = 20f;
    public bool firstTrackInPoolPlayed = false;

    //We use 2 audio sources so that we can fade between tracks if needed
    public AudioSource audioSource;
    //public AudioSource audioSourceB;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = musicMasterVolume;
        

        //Starting music state
        SwitchMusicState(musicState, intialMenuFadeInTime, 1f);
    }


    void Update() {
        if(!audioSource.isPlaying && firstTrackInPoolPlayed) {
            currentMusicPool = GetMusicPoolByState(musicState);
            AudioClip nextTrack = GetRandomTrackFromPool(currentMusicPool);
            Debug.Log(nextTrack);
            float fadeInTime = 1f;
            float fadeOutTime = 1f;

            if (nextTrack != null) {
              PlayMusicWithFade(nextTrack, fadeInTime, fadeOutTime);
            }
        }
    }
    
    /// <summary>
    /// Switch the music to play the track(s) associtated with that state
    /// </summary>
    /// <param name="musicState"></param>
    public void SwitchMusicState(MusicState musicState) {
        firstTrackInPoolPlayed = false;
        this.musicState = musicState;
        currentMusicPool = GetMusicPoolByState(musicState);
        AudioClip nextTrack = GetRandomTrackFromPool(currentMusicPool);
        Debug.Log(nextTrack);
        float fadeInTime = 0.5f;
        float fadeOutTime = 0.5f;

        if (nextTrack != null) {
            PlayMusicWithFade(nextTrack, fadeInTime, fadeOutTime);
        }
        firstTrackInPoolPlayed = true;

    }

    public void SwitchMusicTrackToAudioClip(AudioClip nextTrack, bool Looping = true) {

        this.musicState = MusicState.GAME_PLAY;
        float fadeInTime = 0.5f;
        float fadeOutTime = 0.5f;


        if (nextTrack != null)
        {
            PlayMusicWithFade(nextTrack, fadeInTime, fadeOutTime, true);
        }
        


    }

    public void SwitchMusicState(MusicState musicState, float fadeInTime, float fadeOutTime)
    {
        firstTrackInPoolPlayed = false;

        this.musicState = musicState;
        currentMusicPool = GetMusicPoolByState(musicState);
        AudioClip nextTrack = GetRandomTrackFromPool(currentMusicPool);
        Debug.Log(nextTrack);

        if (nextTrack != null)
        {
            PlayMusicWithFade(nextTrack, fadeInTime, fadeOutTime);
        }
        firstTrackInPoolPlayed = true;

    }

    private MusicPool GetMusicPoolByState(MusicState state) {
        return musicPools.Find(pool => pool.state == state);
    }

    // public void PlayRandomTrack(AudioClip[] tracks) {
    //     int random = Random.Range(0, tracks.Length);
        
    // }

    private AudioClip GetRandomTrackFromPool(MusicPool pool) {
        if (pool.clips.Length == 0) {
            return null;
        }

        //If we have heard all the tracks
        if (pool.getTracksHeard().Count >= pool.clips.Length) {
            pool.getTracksHeard().Clear();
        }

        int randomIndex;
        randomIndex = Random.Range(0, pool.clips.Length);

        while (pool.getTracksHeard().Contains(randomIndex)) {
            randomIndex = Random.Range(0, pool.clips.Length);
        }

        pool.getTracksHeard().Add(randomIndex);

        return pool.clips[randomIndex];
    }
    // private 
    
    private void PlayMusicWithFade(AudioClip newTrack, float fadeInTime, float fadeOutTime, bool looping = false) {
        if (audioSource.isPlaying) {
            StartCoroutine(HandleFadeOutIn(audioSource, newTrack, fadeInTime, fadeOutTime, looping));
        } else {
            StartCoroutine(FadeMusicIn(audioSource, newTrack, fadeInTime, looping));
        }
        //} else {
        //    StartCoroutine(FadeMusicOut(audioSourceB, fadeOutTime));
        //    StartCoroutine(FadeMusicIn(audioSourceA, newTrack, fadeInTime));
        //}
    }


    private IEnumerator HandleFadeOutIn(AudioSource source, AudioClip newTrack, float fadeInTime, float fadeOutTime, bool looping = false) {
        yield return StartCoroutine(FadeMusicOut(source, fadeOutTime));
        yield return StartCoroutine(FadeMusicIn(source, newTrack, fadeInTime, looping));
    }
    private IEnumerator FadeMusicIn(AudioSource source, AudioClip newMusic, float fadeTime = defaultFadeTime, bool looping = false) {
        
        float startVolume = 0f;
        source.clip = newMusic;
        source.volume = startVolume;
        source.loop = looping; // Sets looping to false unless specificed.
        source.Play();
        
        while (source.volume < musicMasterVolume)
        {
            source.volume += Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }

        source.volume = musicMasterVolume;
    }

    private IEnumerator FadeMusicOut(AudioSource source, float fadeTime = defaultFadeTime) {
        //float startVolume = source.volume;
        float currentVolume = source.volume; 
        while (currentVolume > 0) {
            currentVolume -= Time.unscaledDeltaTime / fadeTime;
            source.volume = currentVolume;
            yield return null;
        }
        source.volume = 0f;
        Debug.Log("Completed Fading out");
        source.Stop();
    }

}


[System.Serializable]
public class MusicPool {
    public MusicState state;
    public AudioClip[] clips;
    private List<int> tracksHeard = new List<int>();

    public List<int> getTracksHeard() {
        return tracksHeard;
    } 
}




// public void SwitchMusic(MusicState musicState, float fadeSpeed, float fadeTime) {
    //     this.musicState = musicState;
    //     //PlayMusic(musicState, fadeSpeed, fadeTime);
    // }

    // public void PlayMusic(AudioClip newClip) {
    //     if(audioSource.isPlaying) {
    //         audioSource.Stop();
    //     }

    //     audioSource.clip = newClip;
    // }

    // public void PlayMusic

    // private void PlayMusic(MusicState musicState, float fadeInTime, float fadeOutTime) {
    //     AudioClip nextTrack;
    //     //Get the music clip
    //     switch(musicState) {
    //         case MusicState.TITLE_SCREEN:

    //             nextTrack = mainMenuMusic;
    //             break;
    //         case MusicState.LEVEL1:
    //             nextTrack = mainMenuMusic;
    //             break;
    //         case MusicState.BASECAMP1:
    //         case MusicState.LEVEL2:
    //         case MusicState.BASECAMP2:
    //         case MusicState.LEVEL3:
    //         case MusicState.BASECAMP3:
    //         case MusicState.LEVEL4:
    //         case MusicState.BASECAMP4:
    //         case MusicState.LEVEL5:
            
    //         case MusicState.GAMEOVER:
    //         case MusicState.FINAL:
                


    //         case MusicState.SILENCE:
    //             FadeMusicOut(audioSourceA, fadeOutTime);
    //             FadeMusicOut(audioSourceA, fadeOutTime);
    //             return;
        
        
    //     }
    // }