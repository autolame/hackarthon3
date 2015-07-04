using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{
    public Vector3 localPosition;

    public Transform _levelParent;

    private Rigidbody _rigidbody;

    public float MovementSpeed = 10;
    public float RotationSpeed = 5;

    public Transform StartPosition;
    public float DeathZoneY = -10;

    // Use this for initialization
    void Awake()
    {
        if (_rigidbody == null)
            gameObject.AddComponent<Rigidbody>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    float speed;

    // Update is called once per frame
    void LateUpdate()
    {
        if(speed > 0 && Input.GetAxis("Vertical") < 0 || speed < 0 && Input.GetAxis("Vertical") > 0)
        {
            _rigidbody.velocity.Set(0, _rigidbody.velocity.y, 0);
        }
        if (Input.GetAxis("Vertical") != 0)
        {
                speed = Input.GetAxis("Vertical");
            //_rigidbody.AddRelativeForce(Vector3.forward * MovementSpeed * Input.GetAxis("Vertical"));
            //_rigidbody.AddForce(transform.forward * MovementSpeed * Input.GetAxis("Vertical"));
                _rigidbody.velocity = transform.forward * MovementSpeed * Input.GetAxis("Vertical");
            //transform.localPosition += transform.forward * Input.GetAxis("Vertical");
        }
        else
        {
            _rigidbody.velocity.Set(0, _rigidbody.velocity.y, 0);
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            //			_rigidbody.AddRelativeTorque (Vector3.up * RotationSpeed * Input.GetAxis ("Horizontal"));
            transform.RotateAround(transform.position, transform.up, Input.GetAxis("Horizontal") * RotationSpeed);
        }
        else
        {
            if (Input.GetAxis("Horizontal") == 0)
            {
                _rigidbody.angularVelocity = Vector3.zero;

                transform.RotateAround(transform.position, Vector3.zero, 0);
            }

            if (transform.localPosition.y < _levelParent.transform.position.y + DeathZoneY)
            {
                transform.position = StartPosition.position;
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;


            }

            _rigidbody.AddForce(-_levelParent.up);
            //transform.localPosition -= transform.up;

            _rigidbody.velocity.Set(Mathf.Clamp(_rigidbody.velocity.x, 0, 10), _rigidbody.velocity.y, Mathf.Clamp(_rigidbody.velocity.z, 0, 10));
            _rigidbody.angularVelocity = Vector3.zero;

            localPosition = transform.localPosition;
        }
    }
}
