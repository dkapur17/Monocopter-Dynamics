using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{

    public GameObject Monocopter;
    public float allowedDelta = 0.01f;
    public List<Vector3> Waypoints;

    [Header("Visualization")]
    public bool visualizeWaypoints = true;
    public bool visualizeTrajectory = true;

    private List<Vector3> trajectory = new List<Vector3>();

    private MonocopterControl controller;

    int currWaypointIdx;


    // Start is called before the first frame update
    void Start()
    {
        controller = Monocopter.GetComponent<MonocopterControl>();
        controller.Engage();
        currWaypointIdx = 0;
    }

    void FixedUpdate()
    {
        if (controller.IsEngaged())
        {
            if (currWaypointIdx < Waypoints.Count)
            {
                Vector3 currentPosition = controller.GetCurrentLocation();
                Vector3 targetPosition = Waypoints[currWaypointIdx];

                trajectory.Add(currentPosition);

                controller.SetReferenceLocation(targetPosition);
                if (Vector3.Distance(currentPosition, targetPosition) <= allowedDelta)
                    currWaypointIdx++;
            }
            else
                controller.Disengage();
        }

    }

    private void OnDrawGizmos()
    {
        if (visualizeWaypoints)
        {
            Gizmos.color = Color.red;

            foreach (Vector3 waypoint in Waypoints)
            {
                Gizmos.DrawSphere(waypoint, 0.1f);
            }
        }

        if (visualizeTrajectory)
        {
            Gizmos.color = Color.green;

            for (int i = 1; i < trajectory.Count; i++)
                Gizmos.DrawLine(trajectory[i - 1], trajectory[i]);
        }

    }
}
