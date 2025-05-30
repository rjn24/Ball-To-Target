using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SphereAgent : Agent
{
    public Transform target; // The cube's transform
    public float speed = 3f; // Movement speed
    private Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset agent position
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));

        // Reset target position
        target.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));

        // Reset velocity
        rb.velocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agent's position
        sensor.AddObservation(transform.localPosition);

        // Observe the target's position
        sensor.AddObservation(target.localPosition);

        // Observe the agent's velocity
        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 move = new Vector3(moveX, 0, moveZ) * speed;
        rb.AddForce(move, ForceMode.VelocityChange);

        // Reward the agent for getting closer to the target
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        AddReward(-0.01f * distanceToTarget);

        // Reward if the agent reaches the target
        if (distanceToTarget < 0.5f)
        {
            AddReward(1.0f);
            EndEpisode();
        }

        // Punish the agent for falling off
        if (transform.localPosition.y < 0)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}
