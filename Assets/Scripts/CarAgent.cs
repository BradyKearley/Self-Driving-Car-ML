using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using UnityEngine.InputSystem.XR;
using Unity.MLAgents.Actuators;
using System;

public class CarAgent : Agent
{
    public PrometeoCarController carController;  // Reference to the CarController
    public Transform carTransform;
    public float maxSpeed = 200f;        // Max speed of the car
    private float throttle;
    private float reverse;
    private float steering;
    private bool handbrake;

    [SerializeField] private TrackCheckpoints trackCheckpoints;

    private void Start()
    {
        trackCheckpoints.OnPlayerCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpoints.CarCheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            AddReward(1f);
        }
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, TrackCheckpoints.CarCheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            AddReward(-1f);
        }
    }
    // Called when the agent is reset
    public override void OnEpisodeBegin()
    {
        // Reset the car's position, speed, and other variables as needed
        carTransform.localPosition = Vector3.zero;
        carTransform.localRotation = Quaternion.identity;
        carController.localVelocityX = 0;
        carController.localVelocityZ = 0;
        carController.StopCar();
        trackCheckpoints.ResetCarTrackIndex(transform);
        
    }

   
    private void OnTriggerEnter(Collider other)
    { 
       if(other.tag == "Wall")
        {
            AddReward(-10f);
            EndEpisode();
        }
    }



    // Collect observations from the environment (car's state)
    public override void CollectObservations(VectorSensor sensor)
    {
        // Add observations such as car's velocity, position, etc.
        sensor.AddObservation(carController.carSpeed);  // Car's speed
        sensor.AddObservation(carController.localVelocityX);  // Local X velocity
        sensor.AddObservation(carController.localVelocityZ);  // Local Z velocity
        sensor.AddObservation(carController.steeringAngle);  // Steering angle
    }

    // Perform actions based on the agent's decisions
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        AddReward(-1f / 500000);
        // Actions are given as an array of floats from the model
        // The model will output throttle, reverse, steering, and handbrake actions
        throttle = Mathf.Clamp(vectorAction.ContinuousActions[0], 0f, 1f);  // Throttle (0 to 1)
        reverse = Mathf.Clamp(vectorAction.ContinuousActions[1], 0f, 1f);   // Reverse (0 to 1)
        steering = Mathf.Clamp(vectorAction.ContinuousActions[2], -1f, 1f);  // Steering (-1 to 1)
        handbrake = vectorAction.ContinuousActions[3] > 0f;                 // Handbrake (0 or 1)
        //AddReward(1f / 500000);
        // Apply the actions to the car controller
        if (throttle > 0f)
        {
            carController.GoForward();
        }
        else if (reverse > 0f)
        {
            carController.GoReverse();
        }

        // Apply steering
        if (steering < -0.35)
        {
            carController.TurnLeft();
        }
        else if (steering > 0.35)
        {
            carController.TurnRight();
        }
        else
        {
            carController.ResetSteeringAngle();
        }

        // Apply handbrake
        if (handbrake)
        {
            carController.Handbrake();
        }
        else
        {
            carController.RecoverTraction();
        }

        // Reward based on car's speed (e.g., encourage the agent to go fast)
        AddReward(carController.carSpeed / maxSpeed);
    }

    // Called when the agent is trained or when manual input is given
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        // Manual input for testing purposes (use keyboard to control the car)
        // Throttle (Continuous)
        float throttleInput = Input.GetKey(KeyCode.W) ? 1f : 0f;

        // Reverse (Continuous)
        float reverseInput = Input.GetKey(KeyCode.S) ? 1f : 0f;

        // Steering (Continuous)
        float steeringInput = Input.GetAxis("Horizontal");  // Left/Right steering

        // Handbrake (Discrete)
        int handbrakeInput = Input.GetKey(KeyCode.Space) ? 1 : 0;

        // Assign the inputs to actionsOut
        continuousActions[0] = throttleInput;
        continuousActions[1] = reverseInput;
        continuousActions[2] = steeringInput;
        continuousActions[3] = handbrakeInput;
    }
}
