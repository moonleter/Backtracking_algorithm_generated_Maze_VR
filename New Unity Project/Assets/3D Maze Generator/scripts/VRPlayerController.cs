using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class VRPlayerController : MonoBehaviour
{
    public float speed = 1.0f;
    [SerializeField] private XRController leftController;
    [SerializeField] private XRController rightController;

    private Rigidbody rigidbody;
    private Transform xrRigTransform;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        xrRigTransform = GetComponentInChildren<XRRig>().transform;

        leftController = GameObject.Find("LeftHand Controller").GetComponent<XRController>();
        rightController = GameObject.Find("RightHand Controller").GetComponent<XRController>();
    }

    void Update()
    {
        // Get input from left and right controllers
        InputDevice leftInputDevice = leftController.inputDevice;
        InputDevice rightInputDevice = rightController.inputDevice;
        leftInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftInput);
        rightInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightInput);

        // Calculate movement direction
        Vector3 forward = xrRigTransform.TransformDirection(Vector3.forward);
        Vector3 right = xrRigTransform.TransformDirection(Vector3.right);
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        Vector3 direction = (forward * leftInput.y + right * leftInput.x) + (forward * rightInput.y + right * rightInput.x);

        // Move the character
        rigidbody.MovePosition(rigidbody.position + direction * speed * Time.deltaTime);
    }
}
