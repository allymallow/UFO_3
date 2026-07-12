using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts
{
    public class DraggablePiece : MonoBehaviour
    {
        [SerializeField] private LayerMask draggableLayerMask = ~0; 

        private Vector3 offset; 
        private bool isDragging;
        private Transform draggedObject;

        private ConnectionPoint[] connectionPoints;

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
       
                    draggedObject = GetGroupRoot(transform);
                    offset = draggedObject.position - mouseWorldPosition;
                    isDragging = true;
                }
            }

            if (Mouse.current.leftButton.isPressed && isDragging)
            {
                draggedObject.position = GetMouseWorldPosition() + offset;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
            {
                isDragging = false;

                foreach (ConnectionPoint connectionPoint in connectionPoints)
                {
                    connectionPoint.TryConnect();
                }
            }
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
    }
}