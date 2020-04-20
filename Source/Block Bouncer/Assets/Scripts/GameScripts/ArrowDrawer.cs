using UnityEngine;

public class ArrowDrawer : MonoBehaviour
{
    // Default values
    private Vector3 defaultLocalScale;
    private Vector3 defaultLocalPosition;

    private LineRenderer lineRenderer;

    // Line renderer position values
    private float destinationY = 0;
    private float lineYoffset = 0.15f;

    private void Awake()
    {
        defaultLocalScale = transform.localScale;
        defaultLocalPosition = transform.localPosition;

        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetupLineRenderer(Vector3 origin, Vector3 destination)
    {
        lineRenderer.SetPosition(0, new Vector3(origin.x, origin.y - lineYoffset, origin.z));
        lineRenderer.SetPosition(1, new Vector3(destination.x, destinationY + lineYoffset, destination.z));

        destinationY = destination.y;
    }

    // Scaling arrow depending on distance from  ball to player
    public void ChangeArrowLegth(float distance)
    {
        transform.localScale = new Vector3(distance * defaultLocalScale.x, defaultLocalScale.y,
           defaultLocalScale.z);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 
            defaultLocalPosition.z - distance / 2);
    }

    public void ChangeLineDestPosition(Vector3 destination)
    {
        lineRenderer.SetPosition(1, new Vector3(destination.x, destinationY + lineYoffset, destination.z));
    }
}
