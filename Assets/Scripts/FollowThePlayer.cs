using UnityEngine;
using System.Collections;

//WILL BE MODIFIED

public class FollowThePlayer : MonoBehaviour
{

    // The target we are following
    public Transform target;

    void Update()
    {
        if (!target)
            return;

        transform.position = new Vector3(target.position.x, 2, target.position.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Constants.MAIN_CAMERA_ROTATION_SPEED);
    }
}
