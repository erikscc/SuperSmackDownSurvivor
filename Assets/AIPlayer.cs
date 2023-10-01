using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPlayer : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject checkpointOne;
    private Rigidbody agentRB;
    public bool isOnGround = false;

    public LayerMask groundLayer;

    private int checkPointIndex;
    private GameObject _checkPoints;

    [SerializeField]
    private bool isJumping;

    public Action OnCheckPointReached;

    private void OnEnable()
    {
        OnCheckPointReached += CheckPointReached;
    }
    private void OnDisable()
    {
        OnCheckPointReached -= CheckPointReached;
    }

    private void CheckPointReached()
    {
        checkPointIndex++;

        if (checkPointIndex>=_checkPoints.transform.childCount-1)
            checkPointIndex=_checkPoints.transform.childCount-1;

        if (agent.isActiveAndEnabled)
            agent.SetDestination(_checkPoints.transform.GetChild(checkPointIndex).position);
    }

    private void Start()
    {
        checkPointIndex = 0;
        // Initialize the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        agentRB=GetComponent<Rigidbody>();
        // Find the checkpoint object with the "CheckPointOne" tag
        _checkPoints = GameObject.FindGameObjectWithTag("CheckPoints");

        if (_checkPoints == null)
        {
            Debug.LogError("CheckpointOne not found in the scene.");
        }
        else
        {
            // Set the initial target to the checkpointOne object

            agent.SetDestination(_checkPoints.transform.GetChild(checkPointIndex).position);
        }
    }

    private void Update()
    {
        if (!agent.enabled)
            return;

        // Check if the AI agent has reached the current target checkpoint
        if (agent.remainingDistance <= agent.stoppingDistance && agent.isActiveAndEnabled)
        {
            // If so, set the next target to the checkpointOne object again
            agent.SetDestination(_checkPoints.transform.GetChild(checkPointIndex).position);
        } 
    }

    public void StartJump(float distance)
    {
        isJumping = true;
        agent.enabled = false;
        agentRB.isKinematic = false;
        Vector3 jumpDirection = _checkPoints.transform.GetChild(checkPointIndex).position - agent.transform.position;
        agentRB.velocity = jumpDirection.normalized * distance+ new Vector3(0f, 4f, 0f);
        StartCoroutine(StartJumpPhase());
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Ground"))
        {
            isJumping = false;
        }
        if (other.CompareTag("CheckPoint"))
        {
            CheckPointReached();
        }
    }
    private void OnTriggerExit(Collider other)
    {
            isOnGround = false;
    }

    public IEnumerator StartJumpPhase()
    {
        yield return new WaitForSeconds(0.1f);

        if (isJumping)
        {
            StartCoroutine(StartJumpPhase());
        } else
        {
            agent.enabled = true;
            agentRB.isKinematic = true;

        }
    }


}


