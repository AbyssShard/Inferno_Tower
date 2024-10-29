using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttack : CardBase, CardAction
{
    public void Action()
    {
        endAction = true;
        Debug.Log("ACtionMade");
    }
}
