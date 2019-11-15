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

	[HideInInspector]
	public MenuItem parent;
	[HideInInspector]
	public List<MenuItem> parentList;
	[HideInInspector]
	public int index;

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

	void Hide()
	{
		gameObject.SetActive(false);
	}

	public void InitAllSubItems(MenuItem parent, List<MenuItem> parentList)
	{
		int i = 0;
		foreach(MenuItem menuItem in subMenuItems)
		{
			TextMesh textMesh = menuItem.GetComponent<TextMesh>();
			textMesh.text = menuItem.label;

			menuItem.Hide();
			menuItem.InitAllSubItems(menuItem, subMenuItems);
			menuItem.parent = parent;
			menuItem.parentList = parentList;
			menuItem.index = i;
			i++;
		}
	}

	public void HideAllSubItems()
	{
		foreach(MenuItem menuItem in subMenuItems)
		{
			menuItem.Hide();
		}
	}

	void Show()
	{
		gameObject.SetActive(true);
	}

	public void ShowAllSubItems()
	{
		foreach(MenuItem menuItem in subMenuItems)
		{
			menuItem.Show();
		}
	}
}
