using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	[Header("Audio sources")]
	public AudioSource aSourceSFX;
	public AudioSource aSourceMusic;

	[Header("List of sound effect clips")]
	public List<AudioClipStruct> soundEffects = new List<AudioClipStruct>();

	Dictionary<string, AudioClip> soundEffectsDict = new Dictionary<string, AudioClip>();

	static public SoundManager Instance { get; private set; }

	[System.Serializable]
	public struct AudioClipStruct
	{
		public string name;
		public AudioClip clip;
	}

	void Awake()
	{
		if(Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		GenerateSoundEffectsDict();

		//soundEffects.Find(s => s.name == "");
	}

	void GenerateSoundEffectsDict()
	{
		foreach (AudioClipStruct audioClip in soundEffects)
		{
			soundEffectsDict.Add(audioClip.name, audioClip.clip);
		}
	}

	public void PlaySoundEffect(string clipName)
	{
		aSourceSFX.clip = soundEffectsDict[clipName];
		aSourceSFX.Play();
	}
}
