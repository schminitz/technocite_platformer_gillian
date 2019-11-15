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

	public List<AvailableResolution> availableResolutions;
	int resolutionIndex = 0;

	[System.Serializable]
	public struct AvailableResolution
	{
		public string label;
		public int width;
		public int height;
	}

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
		else if (Input.GetKeyDown(KeyCode.LeftArrow) ||
			     Input.GetKeyDown(KeyCode.Q))
		{
			CallSwitch(goRight: false);
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow) ||
				 Input.GetKeyDown(KeyCode.D))
		{
			CallSwitch(goRight: true);
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

	void CallSwitch(bool goRight)
	{
		switch (activeMenuItem.action)
		{
			case MenuAction.Resolution:
				SwitchResolution(goRight);
				break;
		}
	}

	void GoToParent()
	{
		if(activeMenuItem.parent == null)
			return;

		menuItems = activeMenuItem.parentList;
		activeMenuItem = activeMenuItem.parent;

		activeMenuItem.HideAllSubItems();
		activeIndex = activeMenuItem.index;
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
		SceneManager.LoadScene("stage01");
	}

	void ActionContinue()
	{
		if (PlayerPrefs.GetString("level", "") == "")
		{
			return;
		}

		SceneManager.LoadScene(PlayerPrefs.GetString("level", "stage01"));
	}

	void ActionResolution()
	{
		AvailableResolution res = availableResolutions[resolutionIndex];
		Screen.SetResolution(
			res.width,
			res.height,
			Screen.fullScreen
			);
	}

	void ActionFullscreen()
	{
		Screen.SetResolution(
			Screen.width,
			Screen.height,
			!Screen.fullScreen
			);
	}

	#endregion

	#region Switch
	void SwitchResolution(bool goRight)
	{
		if(goRight)
			resolutionIndex++;
		else
			resolutionIndex--;

		resolutionIndex = (int)Mathf.Repeat(resolutionIndex, availableResolutions.Count);

		activeMenuItem.RefreshLabel(availableResolutions[resolutionIndex].label);
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
			if (menuItem.action == MenuAction.Resolution)
				menuItem.RefreshLabel(availableResolutions[resolutionIndex].label);
			else
				menuItem.RefreshLabel();

			menuItem.SetInactive(inactiveColor);
			menuItem.InitAllSubItems(menuItem, menuItems);
			menuItem.index = i;
			i++;
		}

		activeMenuItem.SetActive(activeColor);
	}
}
