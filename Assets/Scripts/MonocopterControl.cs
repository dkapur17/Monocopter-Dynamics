using UnityEngine;

public class MonocopterControl : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Gains")]
    public Gains xPosGains;
    public Gains yPosGains;
    public Gains zPosGains;
    private PIDController xPosController;
    private PIDController yPosController;
    private PIDController zPosController;

    [Header("Reference State")]
    [SerializeField] private bool engaged = false;

    [SerializeField] private float referenceXPos;
    [SerializeField] private float referenceYPos;
    [SerializeField] private float referenceZPos;

    [Header("Numeric Limits")]
    public int maxPropSpeed = 120;


    [Header("Geometry")]
    public Transform COR;
    public float propellerOffset = 0.552f;
    public float wingSurfaceArea = 0.069f;

    [Header("Current State")]
    public int propRotSpeed = 0;
    private float wingSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = float.MaxValue;

        xPosController = new PIDController(xPosGains);
        yPosController = new PIDController(yPosGains);
        zPosController = new PIDController(zPosGains);
    }

    void FixedUpdate()
    {
        if (engaged)
            AddMotorForces();
        else
            propRotSpeed = 0;

    }

    public void Engage()
    {
        engaged = true;
    }

    public void Disengage()
    {
        engaged = false;
    }

    public bool IsEngaged()
    {
        return engaged;
    }

    public void SetReferenceLocation(Vector3 referenceLocation)
    {
        referenceXPos = referenceLocation.x;
        referenceYPos = referenceLocation.y;
        referenceZPos = referenceLocation.z;
    }

    public Vector3 GetCurrentLocation()
    {
        return transform.position;
    }

    void AddMotorForces()
    {

        // Controller Logic
        float xPosError = GetXPosError();
        float xPosCommand = xPosController.GetCommand(xPosError, Time.fixedDeltaTime);

        float yPosError = GetYPosError();
        int yPosCommand = Mathf.RoundToInt(yPosController.GetCommand(yPosError, Time.fixedDeltaTime));

        float zPosError = GetZPosError();
        float zPosCommand = zPosController.GetCommand(zPosError, Time.fixedDeltaTime);


        // Computing Desired Propeller Speed
        propRotSpeed = yPosCommand;
        propRotSpeed = Mathf.Clamp(propRotSpeed, 0, maxPropSpeed);


        // Adding torque to body of drone due to Propeller Thrust rxF
        float propellerThrust = GetThrustFromRotSpeed();
        float torque = -propellerOffset * propellerThrust;
        rb.AddTorque(COR.transform.up * propellerOffset * torque);

        float aoa = 0;
        float heading = transform.eulerAngles.y;


        // Go Left
        if (xPosError < 0)
        {
            if (45f < heading && heading < 135f)
            {
                aoa = -xPosCommand;
                rb.AddForce(transform.forward * xPosCommand);

            }
            else if (225f < heading && heading < 315f)
            {
                aoa = xPosCommand;
                rb.AddForce(-transform.forward * xPosCommand);

            }

        }
        // Go Right
        else if (xPosError > 0)
        {
            if (45 < heading && heading < 135f)
            {
                aoa = xPosCommand;
                rb.AddForce(transform.forward * xPosCommand);
            }
            else if (225f < heading && heading < 315f)
            {
                aoa = -xPosCommand;
                rb.AddForce(-transform.forward * xPosCommand);
            }

        }

        // Go Back
        if (zPosError < 0)
        {
            if ((0f < heading && heading < 45f) || (315f < heading && heading < 360f))
            {
                aoa = -zPosCommand;
                rb.AddForce(transform.forward * zPosCommand);
            }
            else if (135f < heading && heading < 225f)
            {
                aoa = zPosCommand;
                rb.AddForce(-transform.forward * zPosCommand);
            }
        }
        // Go Forward
        else if (zPosError > 0)
        {
            if ((0f < heading && heading < 45f) || (315f < heading && heading < 360f))
            {
                aoa = zPosCommand;
                rb.AddForce(transform.forward * zPosCommand);
            }
            else if (135f < heading && heading < 225f)
            {
                aoa = -zPosCommand;
                rb.AddForce(-transform.forward * zPosCommand);
            }
        }


        aoa = Mathf.Clamp(aoa, -10f, 10f);



        // Adding Lift due to windspeed on the Wing
        wingSpeed = Mathf.Abs(rb.angularVelocity.y * propellerOffset);
        float lift = ComputeLift(aoa);

        rb.AddForce(transform.up * lift);
    }

    float GetXPosError()
    {
        return referenceXPos - transform.position.x;
    }


    float GetYPosError()
    {
        return referenceYPos - transform.position.y;
    }

    float GetZPosError()
    {
        return referenceZPos - transform.position.z;
    }


    float ComputeLift(float aoa)
    {
        float cl = 0.0826f * aoa + 0.1f;
        float rho = 1.225f;
        float v = wingSpeed;
        float s = wingSurfaceArea;

        return cl * 0.5f * rho * Mathf.Pow(v, 2) * s;
    }

    float GetThrustFromRotSpeed()
    {
        float ct = 1f;
        float rho = 1.225f;
        float n = propRotSpeed;
        float D = 0.187f;

        return ct * rho * Mathf.Pow(propRotSpeed, 2) * Mathf.Pow(D, 4);
    }

    public int GetPropRotSpeed()
    {
        return propRotSpeed;
    }
}
