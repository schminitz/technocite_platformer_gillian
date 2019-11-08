using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Flag : MonoBehaviour
{
	public string sceneToLoad;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Player player = collision.GetComponent<Player>();

		if (player != null)
		{
			LoadNextScene(player);
		}
	}

	Coroutine loadNextSceneCoroutine;

	void LoadNextScene(Player player)
	{
		if(loadNextSceneCoroutine == null)
			loadNextSceneCoroutine = StartCoroutine(LoadNextSceneCoroutine(player));
	}

	IEnumerator LoadNextSceneCoroutine(Player player)
	{
		player.Freeze();
		GetComponent<Animator>().Play("FlagReach");

		yield return new WaitForSeconds(GetComponent<AnimationTimes>().GetTime("FlagReach"));

		SceneManager.LoadScene(sceneToLoad);
	}
}
