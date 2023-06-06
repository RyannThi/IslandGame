using Cinemachine;
using UnityEngine;

public class MouseSensitivity : MonoBehaviour
{
    public CinemachineFreeLook cm;

    void Update()
    {
        cm.m_XAxis.m_MaxSpeed = (PlayerPrefs.GetInt("MOUSE_SENSITIVITY", 50) * 0.1f) + 2;
    }
}
