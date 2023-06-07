using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AvoiderAgent : Agent
{
    private Transform tr;
    private Rigidbody rb;

    [SerializeField]
    private Enemy[] enemies;

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(tr.localPosition);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float h = Mathf.Clamp(actions.ContinuousActions[0], -1.0f, 1.0f);
        float v = Mathf.Clamp(actions.ContinuousActions[1], -1.0f, 1.0f);
        Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
        rb.AddForce(dir.normalized * 50.0f);

        SetReward(-0.001f);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        actionsOut.ContinuousActions.Array[0] = Input.GetAxis("Horizontal");
        actionsOut.ContinuousActions.Array[1] = Input.GetAxis("Vertical");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ENEMY"))
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }
    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        tr.localPosition = new Vector3(0f, 1f, 0f);

        foreach(var enemy in enemies)
        {
            enemy.ResetPos();
        }
    }
}

