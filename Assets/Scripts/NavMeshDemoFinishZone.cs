using UnityEngine;

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
            int safeCount = Mathf.Max(1, agentCount);
            float t = safeCount == 1 ? 0.5f : agentIndex / (float)(safeCount - 1);
            float z = Mathf.Lerp(-destinationSpread.y * 0.5f, destinationSpread.y * 0.5f, t);
            float x = safeCount <= 2 ? 0f : ((agentIndex % 2) == 0 ? -0.35f : 0.35f);

            Vector3 localPoint = new(x, 0f, z);
            return transform.TransformPoint(localPoint);
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
