﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI.Extensions;
using UnityEngine;

public class Test : MonoBehaviour 
{
    public int              Count { get; private set; }
    public string[]         List;
    public List<TestObject> ObjectList;

    private void Start()
    {
        GetComponent<ListView>().Data = ObjectList.Cast<object>();
    }

    public void Increment()
    {
        Count ++;
    }
}
