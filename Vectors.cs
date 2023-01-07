using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vectors : MonoBehaviour
{
    
    public GameObject birb;
    public float m_Thrust;
    public float settleDistance;
    public float detectionRange;
    public float shapeResponsive;
    public float protectionRadius = 50;
    public float birdNoFlyRadius = 20;
    public float homeTendancy = 5;
    public bool debugNet = true;
    public bool debugThrust = false;
    public bool debugTarget = false;

    private Vector3 thrustDirection;
    private Vector3 drone2Home;
    private Rigidbody m_Rigidbody;
    private List<GameObject> nearbyDrones = new List<GameObject>();
    private Vector3 home;

    // Start is called before the first frame update
    void Start()
    {
        home = transform.position;
        
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        PickNearestBirb();
        Vector3 birbVec = VectorTo(birb);
        //Debug.Log(birbVec);
        //Debug.DrawRay(transform.position, birbVec, Color.red);

        birbVec.y = 0;
        transform.rotation = Quaternion.LookRotation(birbVec);

        UpdateNearbyDrones();
        UpdateThrust();
        m_Rigidbody.AddForce(thrustDirection * m_Thrust);
    }

    private Vector3 VectorTo(GameObject gameObject)
    {
        Vector3 objVector = gameObject.transform.position - transform.position; 

        //Debug.Log(objVector);

        return objVector;
    }

    private Vector3 VectorTarget(GameObject gameObject)
    {
        drone2Home = home - transform.position;

        if (debugTarget)
        {
            Debug.DrawRay(transform.position, drone2Home, Color.blue);
        }

        Vector3 home2birb = home - gameObject.transform.position;
        Vector3 drone2birb;

        if (home2birb.magnitude > protectionRadius)
        {
            if (debugTarget)
            {
                Debug.DrawRay(transform.position, drone2Home, Color.red);
            }
            return 1000 * drone2Home;
        }
        else
        {
            drone2birb = VectorTo(birb);

            if (drone2birb.magnitude < birdNoFlyRadius)
            {
                drone2birb *= -10;
            }
            else
            {
                if (debugTarget)
                {
                    Debug.DrawRay(transform.position, drone2birb, Color.red);
                }
            }
            
            
            return drone2birb;
        }

    }

    private void UpdateThrust()
    {
        Vector3 thrustCalc = VectorTarget(birb) + drone2Home/homeTendancy;

        Vector3 birbDirection = thrustCalc.normalized; // NEEDS WORK< SHOULD ALLOW SLIP PERP TO BIRD VECTOR
        //thrustCalc.y *= 3;

        Debug.Log(birbDirection);

        //Debug.Log("before drone");
        //Debug.Log(thrustCalc);

        foreach (GameObject drone in nearbyDrones)
        {
            Vector3 droneVector = VectorTo(drone);

            if (debugNet)
            {
                Debug.DrawRay(transform.position, droneVector, Color.green);
            }
            //Debug.Log(Mathf.Sign(droneVector.z) / Mathf.Pow(droneVector.z, (float)2));

            thrustCalc.x += shapeResponsive * droneVector.x / nearbyDrones.Count - Mathf.Abs(birbDirection.z) * settleDistance * Mathf.Sign(droneVector.x) / Mathf.Pow(droneVector.x, (float)2);
            thrustCalc.y += shapeResponsive * droneVector.y / nearbyDrones.Count - settleDistance * Mathf.Sign(droneVector.y) / Mathf.Pow(droneVector.y, (float)2);
            thrustCalc.z += shapeResponsive * droneVector.z / nearbyDrones.Count - Mathf.Abs(birbDirection.x) * settleDistance * Mathf.Sign(droneVector.z) / Mathf.Pow(droneVector.z, (float)2);
            //thrustCalc += new Vector3(xAttraction, yAttraction, zAttraction);
        }

        //Debug.Log("with drone");
        //Debug.Log(thrustCalc);

        if (transform.position.y < 15)
        {
            thrustCalc.y += 100000;
        }

        thrustDirection = thrustCalc.normalized;

        if (debugThrust)
        {
            Debug.DrawRay(transform.position, thrustCalc / 10, Color.blue);
        }
    }

    private void UpdateNearbyDrones()
    {
        nearbyDrones.Clear();

        foreach (GameObject drone in GameObject.FindGameObjectsWithTag("Drone"))
        {
            Vector3 droneVector = VectorTo(drone);
            //Debug.DrawRay(transform.position, droneVector, Color.red);

            //Debug.Log(drone.name);

            if (!drone.Equals(this.gameObject))
            {
                if (droneVector.magnitude < detectionRange)
                {
                    nearbyDrones.Add(drone);
                }
            }
        }

        //Debug.Log("number of drones");
        //Debug.Log(nearbyDrones.Count);
    }

    private void PickNearestBirb()
    {

        foreach (GameObject birbTest in GameObject.FindGameObjectsWithTag("Birb"))
        {
            Vector3 home2TestBirb = home - birbTest.transform.position;
            Vector3 home2CurrentBirb = home - birb.transform.position;

            if(home2TestBirb.magnitude < home2CurrentBirb.magnitude)
            {
                birb = birbTest;
            }

        }

        //Debug.Log("number of drones");
        //Debug.Log(nearbyDrones.Count);
    }
}
