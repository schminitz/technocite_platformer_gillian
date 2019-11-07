using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
	public Player playerToSpawn;

    // Start is called before the first frame update
    void Start()
    {
		Instantiate(playerToSpawn, transform.position, Quaternion.identity);
    }

}
