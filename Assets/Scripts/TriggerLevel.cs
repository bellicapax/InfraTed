﻿using UnityEngine;
using System.Collections;

public class TriggerLevel : MonoBehaviour {

    public string levelToLoad;

    void OnTriggerEnter(Collider other)
    {
        print(collider.tag);
        if (other.tag == "Player")
            Application.LoadLevel(levelToLoad);
    }
}
