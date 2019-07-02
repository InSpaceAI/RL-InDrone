using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DJAgent : Agent
{

	public GameObject ball;
	Vector3 ballInitPos;
	Rigidbody rbBall;
	Vector3 ballPrePos;
	Vector3 ballCurPos;

	Vector3 droneInitPos;
	Quaternion droneInitRot;
	Rigidbody rbDrone;

	public GameObject pro1;
	public GameObject pro2;
	public GameObject pro3;
	public GameObject pro4;

	Vector3 pro1InitPos;
	Vector3 pro2InitPos;
	Vector3 pro3InitPos;
	Vector3 pro4InitPos;

	Quaternion pro1InitRot;
	Quaternion pro2InitRot;
	Quaternion pro3InitRot;
	Quaternion pro4InitRot;

	Rigidbody rb1;
	Rigidbody rb2;
	Rigidbody rb3;
	Rigidbody rb4;

	public override void InitializeAgent()
	{
		ballInitPos = ball.transform.position;
		rbBall = ball.GetComponent<Rigidbody>();

		droneInitPos = gameObject.transform.position;
		droneInitRot = gameObject.transform.rotation;
		rbDrone = gameObject.GetComponent<Rigidbody>();

		pro1InitPos = pro1.transform.position;
		pro2InitPos = pro2.transform.position;
		pro3InitPos = pro3.transform.position;
		pro4InitPos = pro4.transform.position;

		pro1InitRot = pro1.transform.rotation;
		pro2InitRot = pro2.transform.rotation;
		pro3InitRot = pro3.transform.rotation;
		pro4InitRot = pro4.transform.rotation;

		rb1 = pro1.GetComponent<Rigidbody>();
		rb2 = pro2.GetComponent<Rigidbody>();
		rb3 = pro3.GetComponent<Rigidbody>();
		rb4 = pro4.GetComponent<Rigidbody>();

	}

	public override void CollectObservations()
	{
		AddVectorObs(ball.transform.position - gameObject.transform.position);
		AddVectorObs(gameObject.transform.position - droneInitPos);
		AddVectorObs(gameObject.transform.up);
		AddVectorObs(gameObject.transform.forward);
		AddVectorObs(rbDrone.velocity);
		AddVectorObs(rbDrone.angularVelocity);
		AddVectorObs((ballCurPos - ballPrePos).normalized);
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

		rb1.AddForce(pro1.transform.up * (act0 * 10));
		rb2.AddForce(pro2.transform.up * (act1 * 10));
		rb3.AddForce(pro3.transform.up * (act2 * 10));
		rb4.AddForce(pro4.transform.up * (act3 * 10));

		if (ball.transform.position.y < gameObject.transform.position.y ||
			gameObject.transform.position.y < 0.2f ||
			(gameObject.transform.position - droneInitPos).magnitude > 3f)
		{
			SetReward(-1);
			Done();
			//Debug.Log("Failed.");
		}
		else
		{
			ballPrePos = ballCurPos;
			ballCurPos = ball.transform.position;

			var reward = (gameObject.transform.up.y / 10f ) ;
			reward += (1f/(10f +(gameObject.transform.position - droneInitPos).magnitude));
			reward += (1f/(10f +(ball.transform.position - ballInitPos).magnitude));

			SetReward(reward);
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

		pro1.transform.position = pro1InitPos;
		pro2.transform.position = pro2InitPos;
		pro3.transform.position = pro3InitPos;
		pro4.transform.position = pro4InitPos;

		pro1.transform.rotation = pro1InitRot;
		pro2.transform.rotation = pro2InitRot;
		pro3.transform.rotation = pro3InitRot;
		pro4.transform.rotation = pro4InitRot;

		ball.transform.position = ballInitPos;
		rbBall.velocity = Vector3.zero;
		rbBall.angularVelocity = Vector3.zero;

		ballPrePos = ball.transform.position + ball.transform.up;
		ballCurPos = ball.transform.position;
	}
}
