﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 1;
    private float timer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotate();
    }

    void rotate()
    {
        timer += Time.deltaTime;
       
        transform.eulerAngles = new Vector3(0.0f, timer*speed, 0.0f);
    }

}
