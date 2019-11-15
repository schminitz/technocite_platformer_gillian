using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	}

	void NavigateUpDown(bool goDown)
	{
		if (goDown)
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
		foreach (MenuItem menuItem in menuItems)
		{
			TextMesh textMesh = menuItem.GetComponent<TextMesh>();
			textMesh.text = menuItem.label;
			menuItem.SetInactive(inactiveColor);
		}

		activeIndex = 0;
		activeMenuItem = menuItems[0];
		activeMenuItem.SetActive(activeColor);
	}
}
