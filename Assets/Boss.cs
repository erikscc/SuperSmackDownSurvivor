using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameObject playerTarget;
    // Start is called before the first frame update
    void Start()
    {
        playerTarget=GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(playerTarget.transform);

    }
}
