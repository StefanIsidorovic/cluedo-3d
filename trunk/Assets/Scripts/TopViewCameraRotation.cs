using UnityEngine;
using System.Collections;

public class TopViewCameraRotation : MonoSingleton<TopViewCameraRotation>
{
    public Quaternion target;
    
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, target, Constants.TOP_CAMERA_ROTATION_SPEED);
    }
}
