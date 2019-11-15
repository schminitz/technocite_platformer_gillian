using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuAction
{
	GoDeeper,
	Credits,
	Quit,
	NewGame,
	Continue,
	Resolution,
	Fullscreen
}

public class MenuItem : MonoBehaviour
{
	public string label;
	public MenuAction action;
	public List<MenuItem> subMenuItems;

	TextMesh textMesh;

	// Start is called before the first frame update
	void Awake()
    {
		textMesh = GetComponent<TextMesh>();
    }

	public void SetActive(Color color)
	{
		textMesh.color = color;
	}

	public void SetInactive(Color color)
	{
		textMesh.color = color;
	}
}
