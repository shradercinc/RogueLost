using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform roomTransform;
    private void Update()
    {

        roomTransform = RoomManager.instance.GetCurrentRoom().transform;
        transform.position = new Vector3(roomTransform.position.x, 10, roomTransform.position.z);
    }
    
    public Material effectMat;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, effectMat);
    }
}
