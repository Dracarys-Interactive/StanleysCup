using UnityEngine;

namespace DracarysInteractive.StanleysCup
{
    public class Bounded : MonoBehaviour
    {
        public RectTransform bounds;
        public Vector3 tolerance = Vector3.zero;

        void Update()
        {
            if (bounds && !(bounds.rect.Contains(transform.position + tolerance) ||
                bounds.rect.Contains(transform.position - tolerance)))
            {
                Destroy(gameObject);
            }
        }
    }
}

