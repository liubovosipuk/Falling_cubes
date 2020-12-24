using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float speed = 5f;
    private Transform _rotator; 

    private void Start()
    {
        _rotator = GetComponent<Transform>();
    }

    private void Update()
    {
        _rotator.Rotate(0, speed * Time.deltaTime, 0); // it can help rotate our object with certain speed 
        //if specify the rotation speed in Y, then the rotation will be very fast
    }
}
    
    

    
