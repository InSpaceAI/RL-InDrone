using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DOAgent : Agent
{

    public GameObject goal;
	public GameObject obstacle;
    Vector3 goalInitPos;
    Quaternion InitRot;

    Vector3 droneInitPos;
    Quaternion droneInitRot;
    Rigidbody rbDrone;

    public GameObject moter1;
    public GameObject moter2;
    public GameObject moter3;
    public GameObject moter4;

    Rigidbody rb1;
    Rigidbody rb2;
    Rigidbody rb3;
    Rigidbody rb4;

    float preDist;
    float curDist;

    public override void InitializeAgent()
    {
        goalInitPos = goal.transform.position;

        droneInitPos = gameObject.transform.position;
        droneInitRot = gameObject.transform.rotation;
        rbDrone = gameObject.GetComponent<Rigidbody>();

        rb1 = moter1.GetComponent<Rigidbody>();
        rb2 = moter2.GetComponent<Rigidbody>();
        rb3 = moter3.GetComponent<Rigidbody>();
        rb4 = moter4.GetComponent<Rigidbody>();
    }

    public override void CollectObservations()
    {
        AddVectorObs(goal.transform.position - gameObject.transform.position);
        AddVectorObs(obstacle.transform.position - gameObject.transform.position);
        AddVectorObs(gameObject.transform.up);
		AddVectorObs(gameObject.transform.forward);
        AddVectorObs(rbDrone.velocity);
        AddVectorObs(rbDrone.angularVelocity);
    }

    float act0;
    float act1;
    float act2;
    float act3;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
		act0 = Mathf.Clamp(vectorAction[0], 0f, 1f);
		act1 = Mathf.Clamp(vectorAction[1], 0f, 1f);
		act2 = Mathf.Clamp(vectorAction[2], 0f, 1f);
		act3 = Mathf.Clamp(vectorAction[3], 0f, 1f);

		rb1.AddForce(moter1.transform.up * (act0 * 10));
		rb2.AddForce(moter2.transform.up * (act1 * 10));
		rb3.AddForce(moter3.transform.up * (act2 * 10));
		rb4.AddForce(moter4.transform.up * (act3 * 10));

		if ((goal.transform.position - gameObject.transform.position).magnitude < 0.5f)
        {
            SetReward(10);
            Done();
            //Debug.Log("Success.");
        }
		else if (Mathf.Abs(gameObject.transform.position.x - droneInitPos.x) > 6f ||
			Mathf.Abs(gameObject.transform.position.z - droneInitPos.z) > 6f ||
			gameObject.transform.position.y - droneInitPos.y > 6f ||
			gameObject.transform.position.y < 0.1f ||
			gameObject.transform.up.y < 0.5f ||
			(obstacle.transform.position - gameObject.transform.position).magnitude < 0.3f)
		{
			SetReward(-10);
			Done();
			//Debug.Log("Failed.");
		}
		else
		{
            curDist = (goal.transform.position - gameObject.transform.position).magnitude;
            var reward = (preDist - curDist);
            SetReward(reward);
            preDist = curDist;
        }

    }

    public override void AgentReset()
    {
        gameObject.transform.position = droneInitPos;
        gameObject.transform.rotation = droneInitRot;
        rbDrone.velocity = Vector3.zero;
        rbDrone.angularVelocity = Vector3.zero;

        rb1.velocity = Vector3.zero;
        rb2.velocity = Vector3.zero;
        rb3.velocity = Vector3.zero;
        rb4.velocity = Vector3.zero;
        rb1.angularVelocity = Vector3.zero;
        rb2.angularVelocity = Vector3.zero;
        rb3.angularVelocity = Vector3.zero;
        rb4.angularVelocity = Vector3.zero;

		var theta1 = Random.Range(-Mathf.PI, Mathf.PI);
		var theta2 = Random.Range(-Mathf.PI, Mathf.PI);
		var radius = Random.Range(1f, 5f);

		goal.transform.position = droneInitPos + new Vector3(radius * Mathf.Sin(theta1) * Mathf.Cos(theta2),
															radius * Mathf.Sin(theta1) * Mathf.Sin(theta2),
															radius * Mathf.Cos(theta1));

		obstacle.transform.position = droneInitPos + (goal.transform.position - droneInitPos) * Random.Range(0.3f, 0.8f)
										+ new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f)); //Noise term

		preDist = (goal.transform.position - gameObject.transform.position).magnitude;
    }
}
