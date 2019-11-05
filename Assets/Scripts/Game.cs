using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
		Debug.Log("Start game");
		QualitySettings.vSyncCount = 0;  // VSync must be disabled to allow targetFrameRate
		Application.targetFrameRate = 60;
	}
}
