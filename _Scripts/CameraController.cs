using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    private PlayerController playerController;
    [SerializeField] float xBoundaryMin;
    [SerializeField] float xBoundaryMax;
    [SerializeField] float yBoundaryMin;
    [SerializeField] float yBoundaryMax;
    [SerializeField] float yOffset = 0;
    private Vector3 pos;
    private Vector3 shakeOffset;
    [SerializeField] float shakeRadius;
    public bool isShaking;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        pos = transform.position;
    }

    // Follow the players movement as long as they are alive
    void Update()
    {
        if (playerController.IsDead)
        {
            return;
        }

        // Generate a shaking offset if the camera is shaking
        if (isShaking)
        {
            shakeOffset = Random.insideUnitCircle * shakeRadius;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        pos = new Vector3 (player.position.x, player.position.y + yOffset, -10);
        pos.x = Mathf.Clamp(pos.x, xBoundaryMin, xBoundaryMax);
        pos.y = Mathf.Clamp(pos.y, yBoundaryMin, yBoundaryMax);
        transform.position = pos + shakeOffset;
    }
}
