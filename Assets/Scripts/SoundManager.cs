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

	private void Update()
	{
	}

	/// <summary>
	/// Prépare un dictionnaire contenant tous mes sound effects
	/// {"player_die": playerDieClip,
	///  "player_jump": playerJumpClip,
	///  ...}
	/// </summary>
	void GenerateSoundEffectsDict()
	{
		foreach(AudioClipStruct audioClip in soundEffects)
		{
			soundEffectsDict.Add(audioClip.name, audioClip.clip);
		}
	}

	public void PlaySoundEffect(string clipName)
	{
		// PlayOneShot permet de lancer plusieurs sons sur le meme AudioSource sans conflit
		aSourceSFX.PlayOneShot(soundEffectsDict[clipName]);
	}

	/// <summary>
	/// Réduit le pitch de la musique de 100% à 0% pendant une certaine durée
	/// </summary>
	/// <param name="duration"></param>
	public void LowerMusicPitch(float duration)
	{
		StartCoroutine(LowerMusicPitchCoroutine(duration));
	}

	IEnumerator LowerMusicPitchCoroutine(float duration)
	{
		float time = 0;
		while (time <= duration)
		{
			// MusicPitch 1 > 0
			float musicPitch = Mathf.Lerp(1, 0, time / duration);

			aSourceSFX.outputAudioMixerGroup.audioMixer.SetFloat("MusicPitch", musicPitch);

			time += Time.deltaTime;
			yield return null;
		}

		aSourceSFX.outputAudioMixerGroup.audioMixer.SetFloat("MusicPitch", 0f);
	}

	public void SetMusicPitch(float musicPitch)
	{
		aSourceSFX.outputAudioMixerGroup.audioMixer.SetFloat("MusicPitch", musicPitch);
	}

}
