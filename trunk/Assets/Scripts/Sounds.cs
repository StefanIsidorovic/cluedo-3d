using UnityEngine;
using System.Collections;

public class Sounds : MonoSingleton<Sounds> {

	private AudioClip[] sounds;
	private AudioSource source;
	private static Sounds tmp;

	// Use this for initialization

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
	void Start () {

		sounds = Resources.LoadAll<AudioClip> ("Sounds");
		if (sounds == null) {
			Debug.Log("No sounds Loaded");
			Debug.Break();
				}
	}

	public void PlayWrong(){
		source.Stop ();
		for (int i=0; i<sounds.Length; i++) {
				if(sounds[i].name=="Wrong")
				source.PlayOneShot(sounds[i],0.7f);
				}
	}

	public void PlayDice(){
		source.Stop ();
		for (int i=0; i<sounds.Length; i++) {
			if(sounds[i].name=="Dice")
				source.PlayOneShot(sounds[i],0.7f);
		}
	}
	public void PlayWin(){
		source.Stop ();
		for (int i=0; i<sounds.Length; i++) {
			if(sounds[i].name=="Win")
				source.PlayOneShot(sounds[i],0.7f);
		}
	}

	public void PlayLose(){
		source.Stop ();
		for (int i=0; i<sounds.Length; i++) {
			if(sounds[i].name=="Lose")
				source.PlayOneShot(sounds[i],0.7f);
		}
	}
	public void PlayFootstep(){
		source.Stop ();
		for (int i=0; i<sounds.Length; i++) {
			if(sounds[i].name=="Footstep")
				source.PlayOneShot(sounds[i],0.7f);
		}
	}
}
