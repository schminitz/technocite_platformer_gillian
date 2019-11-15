using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuController : MonoBehaviour
{
	public List<MenuItem> menuItems;
	public Color activeColor;
	public Color inactiveColor;

	MenuItem activeMenuItem;
	int activeIndex;

    // Start is called before the first frame update
    void Start()
    {
		InitMenu();
    }

    // Update is called once per frame
    void Update()
    {
		MenuNavigation();
    }

	void MenuNavigation()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow) ||
			Input.GetKeyDown(KeyCode.S))
		{
			NavigateUpDown(goDown: true);
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow) ||
			    Input.GetKeyDown(KeyCode.Z))
		{
			NavigateUpDown(goDown: false);
		}
		else if(Input.GetKeyDown(KeyCode.Return))
		{
			CallAction();
		}
		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			GoToParent();
		}
	}

	void CallAction()
	{
		switch(activeMenuItem.action)
		{
			case MenuAction.GoDeeper:
				ActionGoDeeper();
				break;
			case MenuAction.Credits:
				ActionCredits();
				break;
			case MenuAction.Quit:
				ActionQuit();
				break;
			case MenuAction.NewGame:
				ActionNewGame();
				break;
			case MenuAction.Continue:
				ActionContinue();
				break;
			case MenuAction.Resolution:
				ActionResolution();
				break;
			case MenuAction.Fullscreen:
				ActionFullscreen();
				break;
			default:
				break;
		}
	}

	void GoToParent()
	{
	}

	#region Actions
	void ActionGoDeeper()
	{
		activeMenuItem.ShowAllSubItems();
		// Change the actual list of menu items
		menuItems = activeMenuItem.subMenuItems;

		InitMenu();
	}

	void ActionCredits()
	{
		SceneManager.LoadScene("credits");
	}

	void ActionQuit()
	{
		Debug.Log("Quitting game");
		Application.Quit();
	}

	void ActionNewGame()
	{
	}

	void ActionContinue()
	{
	}

	void ActionResolution()
	{
	}

	void ActionFullscreen()
	{
	}

	#endregion

	void NavigateUpDown(bool goDown)
	{
		if(goDown)
			activeIndex++;
		else
			activeIndex--;

		activeIndex = (int)Mathf.Repeat(activeIndex, menuItems.Count);

		activeMenuItem.SetInactive(inactiveColor);
		activeMenuItem = menuItems[activeIndex];
		activeMenuItem.SetActive(activeColor);
	}

	void InitMenu()
	{
		activeIndex = 0;
		activeMenuItem = menuItems[0];

		int i = 0;
		foreach (MenuItem menuItem in menuItems)
		{
			TextMesh textMesh = menuItem.GetComponent<TextMesh>();
			textMesh.text = menuItem.label;
			menuItem.SetInactive(inactiveColor);
			menuItem.InitAllSubItems();
			i++;
		}

		activeMenuItem.SetActive(activeColor);
	}
}
