using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	public int lifeCount;
	public bool resetSavesOnStart;

	static public Game Instance { get; private set; }

	// Start is called before the first frame update
	void Awake()
    {
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;

		DontDestroyOnLoad(gameObject);

		QualitySettings.vSyncCount = 0;  // VSync must be disabled to allow targetFrameRate
		Application.targetFrameRate = 60;

		if(resetSavesOnStart)
			ResetSaves();
	}

	public void ResetSaves()
	{
		PlayerPrefs.DeleteAll();
	}
}
