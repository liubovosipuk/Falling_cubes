using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform camTransform;
    
    private float shakeDur = 1f, shakeAmount = 0.04f, decreaseFactor = 1.5f;
    /* shakeDur - camera shake time
     * shakeAmout - camera shake force
     * decreaseFactor - how fast the camera shake time will decrease
     */

    private Vector3 originPos; // camera start position



    private void Start()
    {
        camTransform = GetComponent<Transform>();
        originPos = camTransform.localPosition;

    }


    private void Update()
    {
        if(shakeDur > 0)
        {
            camTransform.localPosition = originPos + Random.insideUnitSphere * shakeAmount;
            shakeDur -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDur = 0;
            camTransform.localPosition = originPos;
        }
    }

}
