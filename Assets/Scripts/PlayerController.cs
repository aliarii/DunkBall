using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody ballRb;
    public ParticleSystem groundParticle;
    public ParticleSystem pointParticle;
    public Transform basket;
    public float smoothSwipe;
    bool addForce;
    void Start()
    {
        ballRb = GetComponentInChildren<Rigidbody>();
        addForce = true;
    }

    void LateUpdate()
    {
        transform.LookAt(basket.position);
        if (SwipeManager.swipeRight)
        {
            ballRb.AddForce(transform.right * smoothSwipe, ForceMode.Impulse);
        }
        if (SwipeManager.swipeLeft)
        {
            ballRb.AddForce(-transform.right * smoothSwipe, ForceMode.Impulse);
        }
        if (SwipeManager.swipeUp)
        {
            ballRb.AddForce(transform.forward * 15f, ForceMode.Impulse);
        }

    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "Ground")
        {
            Instantiate(groundParticle, transform.position, groundParticle.transform.rotation);
            if (addForce)
            {
                ballRb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
            }
        }
        else if (other.transform.tag == "Border")
        {
            ballRb.AddForce(Vector3.back * 10f, ForceMode.Impulse);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Point")
        {
            Instantiate(pointParticle, transform.position, pointParticle.transform.rotation);
            addForce = false;
            gameObject.GetComponent<SphereCollider>().material.bounciness = 0.5f;
            GameManager.isGameOver = true;
        }
    }



}
