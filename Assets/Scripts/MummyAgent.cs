using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MummyAgent : Agent
{
    private Transform tr;
    private Rigidbody rb;
    public Transform targetTr;
    public Renderer floorRd;

    private Material originMt;
    public Material badMt;
    public Material goodMt;

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        originMt = floorRd.material;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetTr.localPosition);
        sensor.AddObservation(tr.localPosition);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float h = Mathf.Clamp(actions.ContinuousActions[0], -1.0f, 1.0f);
        float v = Mathf.Clamp(actions.ContinuousActions[1], -1.0f, 1.0f);
        Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
        rb.AddForce(dir.normalized * 100.0f);

        SetReward(-0.001f);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        actionsOut.ContinuousActions.Array[0] = Input.GetAxis("Horizontal");
        actionsOut.ContinuousActions.Array[1] = Input.GetAxis("Vertical");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("DEADZONE"))
        {
            floorRd.material = badMt;
            SetReward(-1.0f);
            EndEpisode();
        }

        if (collision.collider.CompareTag("TARGET"))
        {
            floorRd.material = goodMt;
            SetReward(1.0f);
            EndEpisode();
        }
    }
    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        tr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.05f, Random.Range(-4.0f, 4.0f));
        targetTr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.55f, Random.Range(-4.0f, 4.0f));
        StartCoroutine(RevertMaterial());
    }

    IEnumerator RevertMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originMt;
    }
}

