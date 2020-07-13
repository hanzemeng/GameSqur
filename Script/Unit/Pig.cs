using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : Unit
{
    void Awake()
    {
        InitializeUnit();
    	HitPoint = 150;
        AttackNeed = 150;
        LineOfSight = 1;
    }
}
