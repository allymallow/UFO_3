using _Project.Scripts;
using UnityEngine;


public class ConnectionPoint : MonoBehaviour
{
    public ConnectionPoint connectedPoint;
    public float connectionDistance = 0.5f;

    public bool IsConnected()
    {
        return connectedPoint != null;
    }

    public void TryConnect()
    {
        if (IsConnected()) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, connectionDistance);
        
        foreach (Collider2D hit in hits)
        {
            ConnectionPoint otherPoint = hit.GetComponent<ConnectionPoint>();

            if (otherPoint == null || otherPoint == this)
            {
                continue;
            }
            

            if (otherPoint.transform.parent == transform.parent)
            {
                continue;
            }

            if (otherPoint.IsConnected())
            {
                continue;
            }

            connectedPoint = otherPoint;
            otherPoint.connectedPoint = this;
            
            ConnectPieces(otherPoint);
            return;
        }
    }

    void ConnectPieces(ConnectionPoint otherPoint)
    {

        Transform thisRoot = DraggablePiece.GetGroupRoot(transform.parent);
        Transform otherRoot = DraggablePiece.GetGroupRoot(otherPoint.transform.parent);
        
        
        if (thisRoot == otherRoot) return; 

        Vector3 difference = otherPoint.transform.position - transform.position;
        thisRoot.position += difference;

        thisRoot.SetParent(otherRoot, true);
    }
}