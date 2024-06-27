using UnityEngine;

public class CarFollowCamera : MonoBehaviour
{
    public Transform car;
    public float distance = 6.4f;
    public float height = 1.4f;
    public float rotationDamping = 3.0f;
    public float heightDamping = 2.0f;
    public float zoomRatio = 0.5f;
    public float defaultFOV = 60f;

    private Vector3 rotationVector;

    void LateUpdate()
    {
        // Calculate desired angle and height
        float wantedAngle = rotationVector.y;
        float wantedHeight = car.position.y + height;

        // Smoothly interpolate current angle and height
        float myAngle = Mathf.LerpAngle(transform.eulerAngles.y, wantedAngle, rotationDamping * Time.deltaTime);
        float myHeight = Mathf.Lerp(transform.position.y, wantedHeight, heightDamping * Time.deltaTime);

        // Set camera rotation
        Quaternion currentRotation = Quaternion.Euler(0, myAngle, 0);
        transform.position = car.position - currentRotation * Vector3.forward * distance;

        // Adjust camera height
        Vector3 tempPosition = transform.position;
        tempPosition.y = myHeight;
        transform.position = tempPosition;

        // Look at the car
        transform.LookAt(car);
    }

    void FixedUpdate()
    {
        // Calculate local velocity and adjust rotation vector
        Vector3 localVelocity = car.InverseTransformDirection(car.GetComponent<Rigidbody>().velocity);
        float targetAngle = (localVelocity.z < -0.1f) ? car.eulerAngles.y + 180 : car.eulerAngles.y;
        rotationVector.y = Mathf.LerpAngle(rotationVector.y, targetAngle, rotationDamping * Time.deltaTime);

        // Adjust field of view based on car speed
        float speed = car.GetComponent<Rigidbody>().velocity.magnitude;
        GetComponent<Camera>().fieldOfView = defaultFOV + speed * zoomRatio * Time.deltaTime;
    }
}
