using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Required for NavMesh

namespace MimicSpace
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Movement : MonoBehaviour
    {
        [Header("Navigation")]
        [Tooltip("The object this Mimic should chase (e.g., the Player)")]
        public Transform target;

        [Header("Controls")]
        [Tooltip("Body Height from ground")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;

        // FIXED: Making these strictly private stops clones from overwriting each other's legs!
        private Mimic myMimic;
        private NavMeshAgent agent;

        // FIXED: Using Awake ensures component links are locked instantly on spawn
        private void Awake()
        {
            myMimic = GetComponent<Mimic>();
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            if (agent != null)
            {
                // Apply the hover height directly to the NavMeshAgent
                agent.baseOffset = height;
            }

            // FIXED: If the spawner didn't give this clone a target, find the player automatically!
            if (target == null)
            {
                // Tries finding by Tag first
                GameObject playerObj = GameObject.FindWithTag("Player");

                // If you don't use tags, tries finding by exact object name "Player"
                if (playerObj == null) playerObj = GameObject.Find("Player");

                if (playerObj != null)
                {
                    target = playerObj.transform;
                }
            }
        }

        void Update()
        {
            // Safety check to ensure components exist for this specific instance
            if (agent == null || myMimic == null) return;

            // 1. ONLY tell the agent to move if it is active and successfully snapped to the NavMesh
            if (target != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.SetDestination(target.position);
            }

            // 2. Get the velocity from the NavMeshAgent and flatten it (x, 0, z)
            Vector3 flattenedVelocity = new Vector3(agent.velocity.x, 0, agent.velocity.z);

            // 3. Pass the velocity strictly to THIS specific procedural animator instance
            myMimic.velocity = flattenedVelocity;
        }
    }
}