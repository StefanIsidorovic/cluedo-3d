using UnityEngine;
//using CorruptedSmileStudio.JukeBox;
using System.Collections;



public class Jukebox : MonoBehaviour{


    // The audio source from where the sound will be playing.
	private AudioSource source;
	// AudioClip for all songs in directory Resourses/Songs
	private AudioClip[] songs;
	// The current song to be played;
    private int currentSong = 0;
 	// Ensures that only one jukebox instance will be available throughout a gameplay.
    private static Jukebox tmp;
	// The volume of the music.

	private float volume=1.0f;
    void Awake()
    {
        if (tmp == null)
        {
            tmp = this;
            DontDestroyOnLoad(gameObject);
            source = gameObject.GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
			songs=Resources.LoadAll<AudioClip>("Songs");
			if (songs == null) {
								Debug.Log ("songs are not loaded");
								Debug.Break ();
						}
       	 	Play ();
           
        
    }
  
  

   
    /// Plays the next song, if there is no next song plays the first song again.
    public void NextTrack()
    {
        Stop();
        if (songs!=null)
        {
           if (songs.Length > 1)
                {
                    System.Random ran = new System.Random();

                    int newSong = currentSong;
                    do
                    {
                        newSong = ran.Next(0, songs.Length);
                    } while (newSong == currentSong);
                    currentSong = newSong;
                }
            
          
            Play();
        }
    }
    /// Plays the previous song, if there is no previous song plays the last song in the list.
 
    public void PreviousTrack()
    {
        Stop();
        if (songs!=null)
        {
				if (songs.Length > 0)
				{
					if (songs.Length > 1)
                {
                    System.Random ran = new System.Random();

                    int newSong = currentSong;
                    while (newSong == currentSong)
                    {
                        newSong = ran.Next(0, songs.Length);
                    }
                    currentSong = newSong;
                }
            }
           
            Play();
        }
    }
    /// Stops playback.
    public void Stop()
    {
        source.Stop();
    }
    /// Starts playing the song at currentSong in the song list.\
    public void Play()
    {
			if (songs != null) {
								if (songs.Length > 0) {
										currentSong = Mathf.Clamp (currentSong, 0, songs.Length - 1);

										if (songs [currentSong] != null) {
												//source.volume = volume;
												source.clip = songs [currentSong];
												source.Play ();
												Invoke ("NextTrack", songs [currentSong].length);
               
										} else {
												Debug.LogError (string.Format ("Songs element {0} is missing an Audio Clip.", currentSong));
												NextTrack ();
										}
								}
						}
    }



  
}
