using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
	public float waitTime;

    // Start is called before the first frame update
    void Start()
    {
		Game.Instance.ResetSaves();
		StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
	{
		yield return new WaitForSeconds(waitTime);

		SceneManager.LoadScene("main_menu");
	}
}
