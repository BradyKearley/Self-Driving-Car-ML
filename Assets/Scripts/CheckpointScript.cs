using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [SerializeField]
    public TrackCheckpoints trackCheckpoints;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.TryGetComponent<Player>(out Player player))
        {
            // Call the instance method on the assigned TrackCheckpoints instance
            trackCheckpoints.CarThroughCheckpoint(this,other.transform);
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }
}

