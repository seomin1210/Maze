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

    public void ResetPos()
    {
        transform.localPosition = _firstPos;
    }
}
