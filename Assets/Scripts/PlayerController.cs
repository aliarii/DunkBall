using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject groundObject;
    [SerializeField] ParticleSystem groundParticle;
    [SerializeField] ParticleSystem[] afterPointParticle;
    [SerializeField] Transform cameraFollowObject;
    [SerializeField] Transform basket;
    [SerializeField] Transform moveBall;
    [SerializeField] Vector3 followOfset;
    [SerializeField] float groundForce;
    [SerializeField] float smoothSwipe;
    [SerializeField] float initialAngle;
    Vector2 mouseStartPos;
    Vector2 mouseEndPos;
    Vector3 ballStartPos;
    Vector2 mousePosDiff;
    Rigidbody ballRb;
    Collider groundCollider;
    bool addForce;
    bool canShoot;
    bool canJump;

    void Start()
    {
        canShoot = false;
        canJump = true;
        addForce = true;
        mousePosDiff = mouseEndPos = mouseStartPos = Vector2.zero;
        ballStartPos = Vector3.zero;
        ballRb = GetComponentInChildren<Rigidbody>();
        cameraFollowObject.position = new Vector3(moveBall.position.x + followOfset.x, cameraFollowObject.position.y, moveBall.position.z + followOfset.z);
        groundCollider = groundObject.gameObject.GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPos = Input.mousePosition;
            ballStartPos = transform.position;
            if (groundCollider.bounds.size.z / 2.5 > Mathf.Abs(transform.position.z))
            {
                canShoot = true;
            }
            else
            {
                canShoot = false;
            }
        }
        if (Input.GetMouseButton(0))
        {
            mouseEndPos = Input.mousePosition;
            mousePosDiff = mouseEndPos - mouseStartPos;
            if (mousePosDiff.x < -(Screen.width / 20f))
            {
                ChangePosition(-moveBall.right, smoothSwipe);
                ApplyForce(-moveBall.right);

            }
            if (mousePosDiff.x > (Screen.width / 20f))
            {
                ChangePosition(moveBall.right, smoothSwipe);
                ApplyForce(moveBall.right);
            }
            if (mousePosDiff.y > (Screen.height / 20f))
            {
                ChangePosition(cameraFollowObject.forward, (smoothSwipe / 2));
                ApplyForce(cameraFollowObject.forward);
            }
            if (mousePosDiff.y < -(Screen.height / 20f))
            {
                ChangePosition(-cameraFollowObject.forward, smoothSwipe);
                ApplyForce(-cameraFollowObject.forward);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            float findDiff;
            findDiff = transform.position.z - ballStartPos.z;
            if (mousePosDiff.y > (Screen.height / 5f) && canJump && canShoot)
            {
                canJump = false;
                ShootBall();
            }
            else if (findDiff < (groundCollider.bounds.size.z / 5f) && mousePosDiff.y > (Screen.height / 5f) && canJump)
            {
                canJump = false;
                transform.rotation = Quaternion.AngleAxis(45, Vector3.left);
                ballRb.AddForce(transform.forward * 5f, ForceMode.Impulse);
            }
        }
    }
    private void LateUpdate()
    {
        cameraFollowObject.position = new Vector3(moveBall.position.x + followOfset.x, cameraFollowObject.position.y, moveBall.position.z + followOfset.z);
        moveBall.LookAt(basket.position);
        cameraFollowObject.LookAt(basket.position);
    }
    void ApplyForce(Vector3 getForce)
    {
        ballRb.AddForce(getForce * Time.deltaTime, ForceMode.Impulse);
    }
    void ChangePosition(Vector3 newPos, float smoothNess)
    {
        transform.Translate(newPos * smoothNess * Time.deltaTime, Space.World);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Ground")
        {
            canJump = true;
            groundParticle.transform.position = moveBall.position;
            groundParticle.gameObject.SetActive(true);
            groundParticle.Play();
            if (addForce)
            {
                ballRb.velocity = ballRb.velocity / 2f;
                ballRb.AddForce(Vector3.up * groundForce, ForceMode.VelocityChange);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Point")
        {
            foreach (ParticleSystem particle in afterPointParticle)
            {
                particle.gameObject.SetActive(true);
            }
            addForce = false;
            gameObject.GetComponent<SphereCollider>().material.bounciness = 0.5f;
            GameManager.isGameOver = true;
        }
    }
    void ShootBall()
    {
        if (Mathf.Abs(basket.transform.position.x - transform.position.x) < 1f && Mathf.Abs(basket.transform.position.z - transform.position.z) < 0.7f)
        {
            ballRb.AddForce(Vector3.back * ballRb.mass * 2f, ForceMode.VelocityChange);
        }
        else
        {
            Vector3 p = basket.position;
            float gravity = Physics.gravity.magnitude;
            // Selected angle in radians
            float angle = initialAngle * Mathf.Deg2Rad;
            // Positions of this object and the target on the same plane
            Vector3 planarTarget = new Vector3(p.x, 0, p.z);
            Vector3 planarPostion = new Vector3(moveBall.position.x, 0, moveBall.position.z);
            // Planar distance between objects
            float distance = Vector3.Distance(planarTarget, planarPostion);
            // Distance along the y axis between objects
            float yOffset = moveBall.position.y - p.y;
            float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
            Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
            // Rotate our velocity to match the direction between the two objects
            float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (p.x > moveBall.position.x ? 1 : -1);
            Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;
            // Fire!
            ballRb.velocity = finalVelocity;
            // Alternative way:
            //ballRb.AddForce(finalVelocity * ballRb.mass, ForceMode.VelocityChange);
        }
    }
}
