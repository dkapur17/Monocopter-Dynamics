using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float smoothScale = 0.01f;
    private Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, target.position - offset, smoothScale);
        Vector3 desiredPosition = new Vector3(smoothedPosition.x, (target.position - offset).y, smoothedPosition.z);
        transform.position = desiredPosition;
    }
}
