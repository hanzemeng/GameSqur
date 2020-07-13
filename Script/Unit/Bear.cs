using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : Unit
{
    void Awake()
    {
        InitializeUnit();
    	HitPoint = 300;
        AttackNeed = 300;
        InitialSpeed = 150;
    }
}
