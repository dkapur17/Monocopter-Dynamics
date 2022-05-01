using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerSpin : MonoBehaviour
{
    public int RotSpeed;
    public MonocopterControl controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(-RotSpeed, 0f, 0f), Space.Self);
    }

    void FixedUpdate()
    {
        RotSpeed = controller.GetPropRotSpeed();
    }
}
