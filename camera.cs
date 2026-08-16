using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform hedef; 
    public Vector3 offset = new Vector3(0, 3, -7); 
    public float takipHizi = 10f;

    void FixedUpdate()
    {
        if (hedef == null) return;

        
        Vector3 hedefPozisyon = hedef.position + hedef.TransformDirection(offset);
        
        
        transform.position = Vector3.Lerp(transform.position, hedefPozisyon, takipHizi * Time.deltaTime);
        
        
        transform.LookAt(hedef);
    }
}
