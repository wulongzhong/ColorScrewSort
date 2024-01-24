using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutBev : MonoBehaviour
{
    public int color;
    public bool moving;
    public int currPosY;
    public bool bMasking;

    public void Init(int color)
    {
        this.color = color;

        switch(color)
        {
            case 0:
                break;

            default:

                break;
        }
    }

    public void MoveToNewStick(StickBev stickBev)
    {
    }

    public void MoveToUp()
    {

    }

    public void MoveToDown()
    {

    }

    public void OpenMask()
    {

    }
}
