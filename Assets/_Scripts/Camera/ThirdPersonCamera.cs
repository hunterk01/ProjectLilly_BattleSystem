using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{

    public GameObject target;
    // GameObject cameraFocus;

    public float damping = 1;
    public Vector3 offset;
    float horizontalSpeed = 2000.0f;
    float verticalSpeed = 2000.0f;
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float h = horizontalSpeed * Input.GetAxis("Mouse X");
            transform.Translate(0, h, 0);
        }
    }

    void LateUpdate()
    {
        float currentAngle = transform.eulerAngles.y;
        float desiredAngle = target.transform.eulerAngles.y;
        float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);

        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        transform.position = target.transform.position - (rotation * offset);

        transform.LookAt(target.transform);

        if (Input.GetMouseButton(1))
        {
            float h = horizontalSpeed * Input.GetAxis("Mouse X");
            float v = horizontalSpeed * Input.GetAxis("Mouse Y");
            Vector3 cam = new Vector3(v, h, 0);
            transform.position = target.transform.position - (rotation * offset);
        }

    }
}