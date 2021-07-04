using UnityEngine;
using System.Collections;

public class ShoppingCartDrive : MonoBehaviour
{
    public float rotateSpeed = 30;
    public float maxAngle = 30;
    public float maxTorque = 300;
    public GameObject wheelShape;

    private TrailRenderer[] _wheelTrails;
    private WheelCollider[] _wheels;
    private bool _brake = true;
    private float _brakeForce = 5000;
    private Rigidbody _rigid;
    private float _angle
    {
        get { return maxAngle * Input.GetAxis("Horizontal"); }
    }
    private float _torque
    {
        get { return maxTorque * Input.GetAxis("Vertical"); }
    }
    private bool _isEmiting = false;
    
    private void Start()
    {
        _wheels = GetComponentsInChildren<WheelCollider>();
        _wheelTrails = GetComponentsInChildren<TrailRenderer>();
        _rigid = GetComponent<Rigidbody>();
        _rigid.centerOfMass = new Vector3(0, -0.9f, 0);

        GenerateWheel();
        Emitting(false);
    }

    private void Emitting(bool enableEmitting)
    {
        foreach (TrailRenderer trail in _wheelTrails)
        {
            if (!enableEmitting) { trail.emitting = false; return; }
            if (trail.transform.localPosition.z > 0)
            {
                bool isLeft = (_angle > 0);
                // Front trail
                if (trail.transform.localPosition.x < 0 && isLeft)
                {
                    // Left
                    trail.emitting = true;
                }
                else if (trail.transform.localPosition.x > 0 && !isLeft)
                {
                    // Right
                    trail.emitting = true;
                }
                else
                {
                    trail.emitting = false;
                }
            }
        }
    }

    private void GenerateWheel()
    {
        for (int i = 0; i < _wheels.Length; ++i)
        {
            var wheel = _wheels[i];
            if (wheelShape != null)
            {
                var ws = GameObject.Instantiate(wheelShape);
                ws.transform.parent = wheel.transform;

                if (wheel.transform.localPosition.x < 0f)
                {
                    ws.transform.localScale = new Vector3(ws.transform.localScale.x * -1f, ws.transform.localScale.y, ws.transform.localScale.z);
                }
            }
        }
    }

    private void Update()
    {
        AdjustBrake();
        Rotate();
        WheelMovement();
        if (_torque >= maxTorque) {
            Emitting(true);
            _isEmiting = true;
        } else if (_isEmiting) {
            Emitting(false);
            _isEmiting = false;
        }
    }

    private void AdjustBrake()
    {
        if (Input.GetButtonUp("Vertical"))
        {
            _brake = true;
        }
        else if (Input.GetButtonDown("Vertical"))
        {
            _brake = false;
        }
    }

    private void Rotate()
    {
        if (_torque < 10)
        {
            transform.Rotate(0.0f, Input.GetAxis("Horizontal") * rotateSpeed, 0.0f);
        }
    }

    private void WheelMovement()
    {
        foreach (WheelCollider wheel in _wheels)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                // Front wheels
                wheel.steerAngle = _angle;
                wheel.motorTorque = _torque * 0.1f;
            }

            if (wheel.transform.localPosition.z < 0)
            {
                // Back wheels
                wheel.motorTorque = _brake ? 0 : _torque;
            }

            wheel.brakeTorque = _brake ? _brakeForce : 0;
            if (wheelShape)
            {
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose(out p, out q);

                Transform shapeTransform = wheel.transform.GetChild(0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }
        }
    }
}
