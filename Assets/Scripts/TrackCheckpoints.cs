using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler<CarCheckpointEventArgs> OnPlayerCorrectCheckpoint;
    public event EventHandler<CarCheckpointEventArgs> OnCarWrongCheckpoint;

    private List<CheckpointScript> checkpointSingleList;
    [SerializeField]
    private List<Transform> CarTransformList;
    private List<int> nextCheckPointIndexList; // Track each car's next checkpoint index

    private void Awake()
    {
        Transform checkpointsTransform = transform;

        checkpointSingleList = new List<CheckpointScript>();

        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointScript checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointScript>();
            if (checkpointSingle == null)
            {

                continue;
            }

            checkpointSingle.SetTrackCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);

        }

        nextCheckPointIndexList = new List<int>();
        foreach (Transform carTransform in CarTransformList)
        {
            nextCheckPointIndexList.Add(0);

        }
    }
    public void ResetCarTrackIndex(Transform carTransform)
    {
        int carIndex = CarTransformList.IndexOf(carTransform);

        if (carIndex != -1)
        {
            nextCheckPointIndexList[carIndex] = 0;
        } 
    }
    public void CarThroughCheckpoint(CheckpointScript checkpoint, Transform carTransform)
    {
        // Get the index of the car in the list
        int carIndex = CarTransformList.IndexOf(carTransform);

        // Get the next expected checkpoint index for this car
        int nextCheckpointIndex = nextCheckPointIndexList[carIndex];

        // Check if the checkpoint index matches the expected next checkpoint
        if (checkpointSingleList.IndexOf(checkpoint) == nextCheckpointIndex)
        {
            // Update to the next checkpoint index, looping around when reaching the end
            nextCheckPointIndexList[carIndex] = (nextCheckpointIndex + 1) % checkpointSingleList.Count;

            // Fire the correct checkpoint event
            CarCheckpointEventArgs args = new CarCheckpointEventArgs
            {
                carTransform = carTransform,
                checkpoint = checkpoint
            };
            OnPlayerCorrectCheckpoint?.Invoke(this, args);
        }
        else
        {
            // Fire the wrong checkpoint event
            CarCheckpointEventArgs args = new CarCheckpointEventArgs
            {
                carTransform = carTransform,
                checkpoint = checkpoint
            };
            OnCarWrongCheckpoint?.Invoke(this, args);
        }
    }

    // Define the event arguments class inside TrackCheckpoints
    public class CarCheckpointEventArgs : EventArgs
    {
        public Transform carTransform { get; set; }
        public CheckpointScript checkpoint { get; set; }
    }
}
