using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MummyRayAgent : Agent
{
    private new Transform transform;
    private Rigidbody rigid;

    public float moveSpeed = 1.5f;
    public float turnSpeed = 200f;

    private StageManager stageManager;

    private Renderer floorRd;
    public Material goodMAT, badMAT;
    private Material originMAT;

    public override void Initialize()
    {
        MaxStep = 5000;

        transform = GetComponent<Transform>();
        rigid = GetComponent<Rigidbody>();
        stageManager = transform.parent.GetComponent<StageManager>();
        floorRd = transform.parent.Find("Floor").GetComponent<Renderer>();
        originMAT = floorRd.material;
    }

    IEnumerator RevertMaterial(Material changeMAT)
    {
        floorRd.material = changeMAT;
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originMAT;
    }

    public override void OnEpisodeBegin()
    {
        stageManager.SetStageObject();
        // 물리엔진 초기화
        rigid.velocity = rigid.angularVelocity = Vector3.zero;
        // 에이전트의 위치 변경
        transform.localPosition = new Vector3(Random.Range(-22f, 22f), 0.05f, Random.Range(-22f, 22f));
        // 에이전트의 회전 변경
        transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;
        Debug.Log($"[0] : {action[0]}, [1] : {action[1]}");

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        switch (action[0])
        {
            case 1: dir = transform.forward; break;
            case 2: dir = -transform.forward; break;
        }

        switch (action[1])
        {
            case 1: rot = -transform.up; break;
            case 2: rot = transform.up; break;
        }

        transform.Rotate(rot, Time.fixedDeltaTime * turnSpeed);
        rigid.AddForce(dir * moveSpeed, ForceMode.VelocityChange);

        AddReward(-1 / (float)MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        actionsOut.Clear();

        // Branch 0 : 이동로직에 쓸 키 매핑
        // Branch 0 size : 3 (정지/non/0, 전진/W/1, 후진/S/2)
        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 2;
        }

        // Branch 1 : 회전로직에 쓸 키 매핑
        // Branch 1 size : 3 (정지/non/0, 좌회전/A/1, 우회전/D/2)
        if (Input.GetKey(KeyCode.A))
        {
            action[1] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2;
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("GOOD_ITEM"))
        {
            rigid.velocity = rigid.angularVelocity = Vector3.zero;
            Destroy(col.gameObject);
            AddReward(1f);

            StartCoroutine(RevertMaterial(goodMAT));
        }
        if (col.collider.CompareTag("BAD_ITEM"))
        {
            AddReward(-1f);
            EndEpisode();

            StartCoroutine(RevertMaterial(badMAT));
        }
        if (col.collider.CompareTag("WALL"))
        {
            AddReward(-0.1f);
        }
    }
}
