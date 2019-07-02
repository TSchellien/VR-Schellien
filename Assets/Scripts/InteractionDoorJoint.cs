using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

using Leap.Unity.Examples;

[RequireComponent(typeof(InteractionBehaviour))]
public class InteractionDoorJoint : MonoBehaviour
{

    private InteractionBehaviour _intObj;


    public int maxRotationZ;
    public int minRotationZ;

    void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();
    }

    void Update()
    {

        

        if (_intObj.isGrasped)
        {

            var sourceHand = Hands.Get(Chirality.Right) != null ? Hands.Get(Chirality.Right) : Hands.Get(Chirality.Left);

            if (sourceHand != null)
            {
                Vector3 rot = sourceHand.Rotation.ToQuaternion().eulerAngles;
                float newRot = -1 * rot.z + 360;
                
                if (this.minRotationZ <= newRot && newRot <= this.maxRotationZ)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, newRot);
                }
            }

        } else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnEnable()
    {
        _intObj = GetComponent<InteractionBehaviour>();
        // Prevent double subscription.


        //_intObj.manager.OnPostPhysicalUpdate -= applyJointConstraint;
        //_intObj.manager.OnPostPhysicalUpdate += applyJointConstraint;

        _intObj.OnGraspStay  = () => { };
        

        _intObj.OnGraspedMovement = applyJointConstraint;
        _intObj.manager.OnPostPhysicalUpdate = () => { };

        //_intObj.OnGraspEnd -= resetDoorJoint;
        //_intObj.OnGraspEnd += resetDoorJoint;


    }
    private void OnDisable()
    {
        //_intObj.OnGraspedMovement -= applyJointConstraint;

        //_intObj.OnGraspEnd -= resetDoorJoint;

    }

    public void resetDoorJoint() {
        Vector3 objRot = _intObj.rigidbody.rotation.eulerAngles;
        objRot.x = 0;
        objRot.y = 0;
        objRot.z = 0;
        _intObj.rigidbody.rotation = Quaternion.Euler(objRot.x, objRot.y, objRot.z);
        _intObj.rigidbody.angularVelocity = Vector3.zero;

    }



    public void applyJointConstraint(Vector3 presolvePos, Quaternion presolveRot, Vector3 solvedPos, Quaternion solvedRot, List<InteractionController> controllers)
    {

        // Constrain the position of the handle and determine the resulting rotation required to get there.
        Vector3 presolveToolToHandle = presolvePos - transform.position;
        Vector3 solvedToolToHandleDirection = (solvedPos - transform.position).normalized;
        Vector3 constrainedToolToHandle = Vector3.ProjectOnPlane(solvedToolToHandleDirection, (presolveRot * Vector3.up)).normalized * presolveToolToHandle.magnitude;
        Quaternion deltaRotation = Quaternion.FromToRotation(presolveToolToHandle, constrainedToolToHandle);

        // Notify the tool about the calculated rotation.
        //_tool.NotifyHandleRotation(deltaRotation);

        // Move the object back to its original position, to be moved correctly later on by the Transform Tool.
        //_intObj.rigidbody.position = presolvePos;

        Vector3  eulerRot = deltaRotation.eulerAngles;
        Debug.Log(eulerRot.x + " " + eulerRot.y + " " + eulerRot.z);
       Quaternion newSolvedRot = Quaternion.Euler(0, solvedRot.eulerAngles.z, 0);
        
        
        //_intObj.rigidbody.position = presolvePos;
        //_intObj.rigidbody.rotation = newSolvedRot;

        Debug.Log("apply joint contraints..");
  /*
        Vector3 objRot = _intObj.rigidbody.rotation.eulerAngles;
        if (objRot.z < this.minRotationZ)
        {
            objRot.z = this.minRotationZ;
            _intObj.rigidbody.rotation = Quaternion.Euler(0,0, objRot.z);
            //_intObj.rigidbody.velocity = objVel;
            Debug.Log("min constraint");
        }
        _intObj.rigidbody.angularVelocity = Vector3.zero;
        */

    }
}
