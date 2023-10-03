using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform player;
    Coroutine cameraShake;
    Vector3 offset;

    private void Start()
    {
        player = GameObject.Find("Player").transform;

    }

    private void LateUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y - 2, -10) + offset;
    }

    public void StartCameraShake(float length = 1, float intensity = 1)
    {
        if (cameraShake != null) { StopCoroutine(cameraShake); }
        cameraShake = StartCoroutine(CameraShake(length, intensity));
    }

    IEnumerator CameraShake(float length = 1, float intensity = 1)
    {
        Debug.Log("here");
        float timer = 0;
        while (timer <= length)
        {
            timer += Time.deltaTime;

            offset = new Vector3(UnityEngine.Random.Range(-1f * intensity, 1f * intensity), UnityEngine.Random.Range(-1f * intensity, 1f * intensity), 0);
            yield return null;
        }
        offset = Vector3.zero;
    }
}
