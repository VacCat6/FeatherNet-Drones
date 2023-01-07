using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbControl : MonoBehaviour
{
    public float m_Thrust;
    public float runAwayRange;
    public float runAwaySpeed;
    public float pers = 5000;
    public bool debugThrust = false;

    public GameObject markerOne;
    public GameObject markerTwo;

    private Vector3 home;
    private Rigidbody m_Rigidbody;
    private Vector3 thrustDirec;
    private Vector3 thrustCalc;
    private Vector3 randomVec;
    private List<GameObject> nearbyDrones = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        // random direction
        Vector3 mk1Vec = markerOne.transform.position - transform.position;
        Vector3 mk2Vec = markerTwo.transform.position - transform.position;

        var rX = UnityEngine.Random.Range(mk1Vec.x, mk2Vec.x);
        var rY = UnityEngine.Random.Range(mk1Vec.y, mk2Vec.y);
        var rZ = UnityEngine.Random.Range(mk1Vec.z, mk2Vec.z);
        randomVec = new Vector3(rX, rY, rZ);


    }

    // Update is called once per frame
    void Update()
    {
        UpdateNearbyDrones();
        UpdateThrust();
        m_Rigidbody.AddForce(thrustDirec * m_Thrust);

    }


    private void UpdateThrust()
    {

        thrustCalc = randomVec;

        foreach (GameObject drone in nearbyDrones)
        {
            // if near drone move away
            Vector3 droneVector = VectorTo(drone);
            Vector3 runAwayVec = runAwaySpeed * droneVector;

            // final direction to move
            thrustCalc -= runAwayVec;
            pers -= 1;
            if (pers < 0)
            {
                randomVec *= -1;
            }


            if (debugThrust)
            {
                //Debug.DrawRay(transform.position, runAwayVec / 10, Color.red);
            }
        }

        if (debugThrust)
        {
            //Debug.DrawRay(transform.position, randomVec / 10, Color.green);
            Debug.DrawRay(transform.position, thrustCalc / 10, Color.blue);
        }

        thrustDirec = thrustCalc.normalized;
        transform.rotation = Quaternion.LookRotation(thrustDirec);



    }

    private Vector3 VectorTo(GameObject gameObject)
    {
        Vector3 objVector = gameObject.transform.position - transform.position;

        //Debug.Log(objVector);

        return objVector;
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
                if (droneVector.magnitude < runAwayRange)
                {
                    nearbyDrones.Add(drone);
                }
            }
        }


    }



}