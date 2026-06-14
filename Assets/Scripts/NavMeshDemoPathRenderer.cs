using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDiplomaDemo
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class NavMeshDemoPathRenderer : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float yOffset = 0.18f;

        private LineRenderer line;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.useWorldSpace = true;
        }

        private void LateUpdate()
        {
            if (agent == null || agent.path == null || agent.path.corners.Length == 0)
            {
                line.positionCount = 0;
                return;
            }

            Vector3[] corners = agent.path.corners;
            line.positionCount = corners.Length;
            for (int i = 0; i < corners.Length; i++)
            {
                line.SetPosition(i, corners[i] + Vector3.up * yOffset);
            }
        }

        public void Bind(NavMeshAgent nextAgent)
        {
            agent = nextAgent;
        }
    }
}
