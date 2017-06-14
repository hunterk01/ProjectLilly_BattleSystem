using UnityEngine;
using System.Collections;

public class SelectorMovement : MonoBehaviour
{
    public float rotationSpeed = 25f;
    public float movementSpeed = 5f;

	void Update ()
	{
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}
