﻿using UnityEngine;

public class Counter : MonoBehaviour 
{
    public int Count { get; private set; }

    public void Increment()
    {
        Count ++;
    }
}
