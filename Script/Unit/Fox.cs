using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : Unit
{
    void Awake()
    {
        InitializeUnit();
    	HitPoint = 50;
        AttackNeed = 50;
        InitialSpeed = 200;
        AttackRange = 2;
    }
}
