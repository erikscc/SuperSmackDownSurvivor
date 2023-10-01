using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIJumpArea : MonoBehaviour
{
    public float distance;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            other.GetComponent<AIPlayer>().StartJump(distance);
        }
    }

   
}
