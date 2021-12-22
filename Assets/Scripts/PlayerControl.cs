using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public int Health;

    public void ChangeHealth(int Change)
    {
        Health -= Change;
        if (Health < 1)
        {
            Death();
        }
    }
    public void Death()
    {

    }
}
