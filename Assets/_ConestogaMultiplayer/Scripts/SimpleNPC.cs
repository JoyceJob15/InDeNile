using UnityEngine;
using UnityEngine.AI;

public class SimpleNPC : MonoBehaviour
{
    [SerializeField] private Transform destination;

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = destination.position;
    }

    void Update()
    {
        
    }
}
