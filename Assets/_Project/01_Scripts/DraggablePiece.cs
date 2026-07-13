using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts
{
    public class DraggablePiece : MonoBehaviour
    {
        [SerializeField]
        private LayerMask draggableLayerMask = ~0;

        [SerializeField]
        private float rotationStepDegrees = 90f;

        private Vector3 offset;
        private bool dragging;
        private Transform draggedRoot;

        private ConnectionPoint[] connectionPoints;

        private static int globalSortingOrder = 0;

        void Start()
        {
            connectionPoints = GetComponentsInChildren<ConnectionPoint>();
        }

        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 mouseWorldPosition = GetMouseWorldPosition();

                RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity, draggableLayerMask);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    bool isRoot = transform.parent == null;

                    if (isRoot)
                    {
                        draggedRoot = GetGroupRoot(transform);
                    }
                    else
                    {
                        DetachFromGroup();
                        draggedRoot = transform;
                    }

                    offset = draggedRoot.position - mouseWorldPosition;
                    dragging = true;

                    BringGroupToFront(draggedRoot);
                }
            }

            if (Mouse.current.leftButton.isPressed && dragging)
            {
                draggedRoot.position = GetMouseWorldPosition() + offset;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && dragging)
            {
                dragging = false;

                foreach (ConnectionPoint connectionPoint in connectionPoints)
                {
                    connectionPoint.TryConnect();
                }
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Vector3 mouseWorldPosition = GetMouseWorldPosition();

                RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity, draggableLayerMask);

                bool canRotate = transform.parent == null;

                if (hit.collider != null && hit.collider.gameObject == gameObject && canRotate)
                {
                    transform.Rotate(0f, 0f, rotationStepDegrees);
                }
            }
        }

        void DetachFromGroup()
        {
            foreach (ConnectionPoint point in connectionPoints)
            {
                point.Detach();
            }

            transform.SetParent(null, true);
        }

        Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            worldPosition.z = transform.position.z;

            return worldPosition;
        }

        public static Transform GetGroupRoot(Transform piece)
        {
            Transform current = piece;

            while (current.parent != null && current.parent.GetComponent<DraggablePiece>() != null)
            {
                current = current.parent;
            }

            return current;
        }

        public static void BringGroupToFront(Transform root)
        {
            globalSortingOrder++;

            SpriteRenderer[] renderers = root.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.sortingOrder = globalSortingOrder;
            }
        }
    }
}