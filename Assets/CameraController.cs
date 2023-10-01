using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public float moveSmoothness;
    public float rotSmoothness;
    public Vector3 moveOffset;
    public Vector3 rotOffset;


    public Transform targetObject;


    void FixedUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {

            targetObject = targetObject.transform;
            Vector3 targetPos = new Vector3();
            targetPos = targetObject.TransformPoint(moveOffset);

            transform.position = Vector3.Lerp(transform.position, targetPos, moveSmoothness * Time.deltaTime);
 
    }

    void HandleRotation()
    {


            var direction = targetObject.position - transform.position;
            var rotation = new Quaternion();

            rotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotSmoothness * Time.deltaTime);
  
    }

}