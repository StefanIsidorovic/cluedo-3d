using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControler : MonoSingleton<CameraControler>
{
    /// <summary>
    /// Name of main camera in editor. This name is used when cameras are fetched.
    /// </summary>
    private readonly string mainCameraName = "Main Camera";

    /// <summary>
    /// Name of top view camera in editor. This name is used when cameras are fetched.
    /// </summary>
    private readonly string topViewCameraName = "TopViewCamera";

    /// <summary>
    /// Initial rotation of main camera. Note that rotation of main camera is changing during the game play.
    /// </summary>
    public static readonly Vector3[] mainCameraStartingRotation = { 
                                                                     new Vector3(45, 180, 0), 
                                                                     new Vector3(45, -90, 0), 
                                                                     new Vector3(45, 0, 0), 
                                                                     new Vector3(45, 0, 0), 
                                                                     new Vector3(45, 90, 0), 
                                                                     new Vector3(45, 90, 0) 
                                                                  };
    /// <summary>
    /// Initial and final rotation of top view camera.
    /// </summary>
    public static readonly Vector3[] topViewCameraStartingRotation = {
                                                                        new Vector3(90, 180, 0), 
                                                                        new Vector3(90, 270, 0), 
                                                                        new Vector3(90, 0, 0), 
                                                                        new Vector3(90, 0, 0), 
                                                                        new Vector3(90, 90, 0), 
                                                                        new Vector3(90, 90, 0)
                                                                     };

    /// <summary>
    /// Main Camera
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// TopView Camera
    /// </summary>
    private Camera topViewCamera;

    /// <summary>
    /// Reference to player tracked by Main Camera and TopViewCamera
    /// </summary>
    private CharacterControl playerTracked;

    void Start()
    {
        // Fetch and initialize cameras
        foreach (var c in Camera.allCameras)
        {
            if (c.name == mainCameraName)
            {
                mainCamera = c;
            }
            else if (c.name == topViewCameraName)
            {
                topViewCamera = c;
            }
        }
        mainCamera.enabled = false;
        topViewCamera.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            mainCamera.enabled = !mainCamera.enabled;
            topViewCamera.enabled = !topViewCamera.enabled;
            if (playerTracked && IsTopViewCamera)
            {
                playerTracked.GetComponent<CharacterControl>().CameraSwitchedToTopViewRotationAdjustment();
            }
        }
    }

    public CharacterControl PlayerTracked { get { return playerTracked; } set { playerTracked = value; } }

    public bool IsTopViewCamera { get { return topViewCamera.enabled; } }
    public bool IsMainCamera { get { return mainCamera.enabled; } }
}
