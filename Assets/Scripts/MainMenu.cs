using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		Invoke("LoadNextScene", 3f);
    }

	void LoadNextScene()
	{
		SceneManager.LoadScene(PlayerPrefs.GetString("level", "stage01"));
	}
}
