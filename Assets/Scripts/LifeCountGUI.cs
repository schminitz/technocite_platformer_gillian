using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCountGUI : MonoBehaviour
{
	TextMesh textMesh;

    // Start is called before the first frame update
    void Start()
    {
		textMesh = GetComponent<TextMesh>();
		RefreshLifeCount();
    }

	// Update is called once per frame
	public void RefreshLifeCount()
	{
		textMesh.text = Game.Instance.lifeCount.ToString();
	}
}
