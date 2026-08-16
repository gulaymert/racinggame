using UnityEngine;

public class carbalance : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = (Rigidbody)GetComponent(typeof(Rigidbody));
        
        
        rb.centerOfMass = new Vector3(0, -0.6f, 0); 
    }
}