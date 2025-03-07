using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviourPun
{
    public float moveSpeed = 10f;

    void Start()
    {
        if (!photonView.IsMine)
        {
            // Disable the camera and this script for other players
            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
            enabled = false;
        }
    }

    void Update()
    {
        // Movement controls (e.g., WASD or arrow keys)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, v, 0f);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }
}
