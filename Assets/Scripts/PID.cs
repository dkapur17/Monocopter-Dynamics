using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDController
{
    float runningError = 0f;
    float prevError = 0f;

    private Gains gains;

    public PIDController(Gains gains)
    {
        this.gains = gains;
    }

    public float GetCommand(float error, float timeStep)
    {
        runningError += error * timeStep;
        float errorSlope = (error - prevError) / timeStep;
        prevError = error;

        float propPath = this.gains.Kp * error;
        float intPath = this.gains.Ki * runningError;
        float derPath = this.gains.Kd * errorSlope;


        float command = propPath + intPath + derPath;
        return command;
    }
}

[System.Serializable]
public class Gains
{
    public float Kp;
    public float Ki;
    public float Kd;

    public Gains(float Kp, float Ki, float Kd)
    {
        this.Kp = Kp;
        this.Ki = Ki;
        this.Kd = Kd;
    }
}
