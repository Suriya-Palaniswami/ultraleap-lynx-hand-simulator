using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Leap;
public class HandOverride : MonoBehaviour, IEquatable<Hand>
{
    public float GrabStrength = 1.0f;

    public bool Equals(Hand other)
    {
        this.GrabStrength = other.GrabStrength;
        if (other.GrabStrength == this.GrabStrength)
        {
            return true;
        }
        else
        {
            return false;
        }
         
    }

    
}
