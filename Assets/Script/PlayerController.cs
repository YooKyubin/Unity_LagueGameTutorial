using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidbody;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public void LookAt(Vector3 point)
    {
        Vector3 heightCorrectedPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(heightCorrectedPoint);
    }

    // 정기적이로 짧게 반복적으로 실행되어야 함
    // 다른 오브젝트 밑으로 끼지 않도록
    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.deltaTime);
    }
}
