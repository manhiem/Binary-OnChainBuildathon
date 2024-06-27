using UnityEngine;
using Cinemachine;

public class CameraSetup : MonoBehaviour
{
    public Transform carTransform;

    void Start()
    {
        CinemachineFreeLook freeLookCam = GetComponent<CinemachineFreeLook>();
        freeLookCam.Follow = carTransform;
        freeLookCam.LookAt = carTransform;

        freeLookCam.m_Orbits[0].m_Height = 5;
        freeLookCam.m_Orbits[0].m_Radius = 2;

        freeLookCam.m_Orbits[1].m_Height = 2;
        freeLookCam.m_Orbits[1].m_Radius = 3;

        freeLookCam.m_Orbits[2].m_Height = 0;
        freeLookCam.m_Orbits[2].m_Radius = 5;

        freeLookCam.m_Lens.FieldOfView = 150;
        freeLookCam.m_Lens.NearClipPlane = 0.1f;
        freeLookCam.m_Lens.FarClipPlane = 1000f;

        freeLookCam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;

        freeLookCam.m_YAxis.Value = 1;
        freeLookCam.m_XAxis.m_MaxSpeed = 300;
        freeLookCam.m_YAxis.m_MaxSpeed = 2;

        freeLookCam.m_SplineCurvature = 0.2f;

        //freeLookCam.m_XAxis.m_InvertAxis.InputValue = true; // depending on your preference
        //freeLookCam.m_YAxis.m_InvertAxis = false; // depending on your preference

        //freeLookCam.m_XAxis.m_Recentering = 0.1f;
        //freeLookCam.m_YAxis.m_Recentering.m_DeadZoneHeight = 0.1f;
        //freeLookCam.m_XAxis.m_Recentering.m_SoftZoneWidth = 0.2f;
        //freeLookCam.m_YAxis.m_Recentering.m_SoftZoneHeight = 0.2f;

        // Adjust follow offset and damping in transposers for each rig
        SetupRig(freeLookCam.GetRig(0).transform);
        SetupRig(freeLookCam.GetRig(1).transform);
        SetupRig(freeLookCam.GetRig(2).transform);
    }

    void SetupRig(Transform rig)
    {
        CinemachineVirtualCamera virtualCam = rig.GetComponent<CinemachineVirtualCamera>();
        if (virtualCam != null)
        {
            CinemachineTransposer transposer = virtualCam.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                // Adjust the follow offset and damping to smooth out the camera movement
                transposer.m_FollowOffset = new Vector3(0, 3, -10); // Example offset, adjust as needed
                transposer.m_XDamping = 0.5f;
                transposer.m_YDamping = 0.5f;
                transposer.m_ZDamping = 0.5f;
            }
        }
    }
}
