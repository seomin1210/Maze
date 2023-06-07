using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody _rigid;

    [SerializeField]
    private GameObject _target;
    private Vector3 _firstPos;

    private void Awake()
    {
        TryGetComponent(out _rigid);

        _firstPos = transform.localPosition;
    }

    private void Update()
    {
        if (_target != null)
        {
            transform.LookAt(_target.transform);
        }
    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            _rigid.velocity = Vector3.zero;
            Vector3 dir = _target.transform.position - transform.position;
            dir.y = 0;
            _rigid.AddForce(dir.normalized * 50f);
        }
    }

    public void ResetPos()
    {
        transform.localPosition = _firstPos;
    }
}
