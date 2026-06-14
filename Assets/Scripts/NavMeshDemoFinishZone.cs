using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDiplomaDemo
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class NavMeshDemoFinishZone : MonoBehaviour
    {
        [SerializeField] private Vector2 destinationSpread = new(2.8f, 4.8f);

        private BoxCollider zoneCollider;

        private void Awake()
        {
            zoneCollider = GetComponent<BoxCollider>();
            zoneCollider.isTrigger = true;
        }

        public Vector3 GetDestinationPoint(int agentIndex, int agentCount)
        {
            if (zoneCollider == null)
            {
                zoneCollider = GetComponent<BoxCollider>();
            }

            int safeCount = Mathf.Max(1, agentCount);
            float t = safeCount == 1 ? 0.5f : agentIndex / (float)(safeCount - 1);
            Bounds bounds = zoneCollider.bounds;
            float xPadding = Mathf.Min(0.55f, bounds.extents.x * 0.35f);
            float zPadding = Mathf.Min(0.75f, bounds.extents.z * 0.35f);
            float zMin = bounds.min.z + zPadding;
            float zMax = bounds.max.z - zPadding;
            float xCenter = bounds.center.x;
            float xOffset = safeCount <= 2 ? 0f : ((agentIndex % 2) == 0 ? -xPadding : xPadding);

            float z = Mathf.Lerp(zMin, zMax, t);
            Vector3 worldPoint = new(xCenter + xOffset, bounds.center.y, z);

            if (NavMesh.SamplePosition(worldPoint, out NavMeshHit hit, 2.5f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return bounds.ClosestPoint(worldPoint);
        }

        public bool Contains(Vector3 worldPosition)
        {
            if (zoneCollider == null)
            {
                zoneCollider = GetComponent<BoxCollider>();
            }

            Vector3 local = transform.InverseTransformPoint(worldPosition) - zoneCollider.center;
            Vector3 halfSize = zoneCollider.size * 0.5f;

            return Mathf.Abs(local.x) <= halfSize.x
                && Mathf.Abs(local.y) <= halfSize.y
                && Mathf.Abs(local.z) <= halfSize.z;
        }
    }
}
