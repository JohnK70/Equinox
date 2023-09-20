using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabScript : MonoBehaviour
{
    public GameObject player;
    public Transform posHold;
    //public GameObject pickUpText;
    private GameObject lookObject;
    private GameObject grabbedObject;
    private Rigidbody grabRigid;
    private bool currLook = false;

    void Update() {
        
        // Checks to see if object is in grabbable range.
        if (grabbedObject == null) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f)) {
                if (hit.transform.gameObject.tag == "Grabbable") {
                    lookObject = hit.transform.gameObject;
                    currLook = true;
                   // pickUpText.SetActive(true);
                }
            } else {
                lookObject = null;
                currLook = false;
               // pickUpText.SetActive(false);
            }
        }
        
        
        // If looking at object and key pressed is E then pick up the object.
        if (currLook && grabbedObject == null && Input.GetKeyDown(KeyCode.E)) {
            grabObject(lookObject);
        }

        if (grabbedObject != null) {
            moveObject();
            if (Input.GetKeyDown(KeyCode.Q)) {
                dropObject();
            }
        }

    }

    public void grabObject(GameObject grabObj) {
        grabbedObject = grabObj;
        grabRigid = grabbedObject.GetComponent<Rigidbody>();
        grabRigid.isKinematic = true;
        grabRigid.transform.parent = posHold.transform;
        grabbedObject.transform.rotation = posHold.rotation;
        Physics.IgnoreCollision(grabbedObject.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        //pickUpText.SetActive(false);
    }

    // Drops Object
    // I was trying to get it to drop directly in front of the camera, but I was busy.
    // Can finish later, unless someone else wants to take care of it.
    void dropObject() {
        Physics.IgnoreCollision(grabbedObject.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        grabRigid.isKinematic = false;
        grabbedObject.transform.parent = null;
        grabbedObject = null;
    }

    void moveObject() {
        grabbedObject.transform.position = posHold.transform.position;
    }

}
