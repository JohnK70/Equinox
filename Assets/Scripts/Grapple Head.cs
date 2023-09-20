using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrappleHead : MonoBehaviour
{
    public bool insideSomething;
    public float SPEED;
    private PlayerGrapple player;
    private Rigidbody rigidBody;
    public bool retracting;
    private LineRenderer grapplingHookLine;
    private GameObject grabbedObj;
    private Rigidbody grabRig;
    private GrabScript grab;

    void Awake()
    {
        player = FindObjectOfType<PlayerGrapple>();
        grab = FindObjectOfType<GrabScript>();
        rigidBody = GetComponent<Rigidbody>();
        grapplingHookLine = transform.GetChild(0).GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            grapplingHookLine.SetPosition(0, player.transform.position);
            grapplingHookLine.SetPosition(1, transform.position);
            grapplingHookLine.startWidth = .25f;
            grapplingHookLine.endWidth = .25f;
        }
        else
        {
            grapplingHookLine.gameObject.SetActive(false);
        }
        if (player.MAXDISTANCE < (player.transform.position - transform.position).magnitude)
        {
            StopGrappling();
        }
    }
    public void StartMovement(Vector3 startPosition, Vector3 direction)
    {
        if (retracting)
        {
            return;
        }
        if (gameObject.activeSelf)
        {
            StopGrappling();
            return;
        }
        gameObject.SetActive(true);
        transform.position = startPosition + direction.normalized * 1f;
        rigidBody.AddForce(direction * SPEED);
    }

    // In this method you see the ramblings of a madman trying to figure out why I was being flung out into space.
    // This code is bad. I will refactor, I was just trying to get it to work ish.
    // Also slightly broken since I'm not sure why the grapple zooms so fast on a grappable item.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerGrapple>() == null)
        {
            
            if (!insideSomething && collision.gameObject.CompareTag("Grabbable")) { // Code for if the object was grabbable
                grabbedObj = collision.gameObject;
                grabRig = grabbedObj.GetComponent<Rigidbody>();
                grabRig.isKinematic = false;
                //grabRig.transform.parent = rigidBody.transform;
                // grabRig.transform.parent = rigidBody.transform;

                rigidBody.isKinematic = true; //Disables Physics on this object
                GetComponent<Collider>().enabled = false;
                transform.parent = collision.transform;
                insideSomething = true;
                player.StartGrappling(collision.collider);
            }
           
            
            
            if (!insideSomething)
            {
                rigidBody.isKinematic = true; //Disables Physics on this object
                GetComponent<Collider>().enabled = false;
                transform.parent = collision.transform;
                insideSomething = true;
                player.StartGrappling(collision.collider);
            }
        }
    }

    public void StopGrappling()
    {
        insideSomething = false;
        player.StopGrappling();
        transform.parent = null;
        rigidBody.velocity = Vector3.zero;
        StartCoroutine(Retract());
    }

    public IEnumerator Retract()
    {
        retracting = true;
        while ((transform.position - player.transform.position).magnitude > 1f)
        {
            transform.position -= (transform.position - player.transform.position).normalized * (25 / SPEED);
            yield return new WaitForSeconds(.02f);
        }
        if (grabbedObj != null) { // Only when object is grabbable
            grab.grabObject(grabbedObj);
            grabRig = null;
            grabbedObj = null;
        }
        rigidBody.isKinematic = false; //enables physics
        GetComponent<Collider>().enabled = true;
        gameObject.SetActive(false);
        retracting = false;
    }
}
