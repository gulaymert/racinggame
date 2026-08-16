using UnityEngine;

public class car : MonoBehaviour
{
    public WheelCollider onSolCollider, onSagCollider;
    public WheelCollider arkaSolCollider, arkaSagCollider;
    public Transform onSolMesh, onSagMesh;
    public Transform arkaSolMesh, arkaSagMesh;

    public float motorGucu = 5000f; 
    public float geriVitesGucu = 2000f; 
    public float frenGucu = 8000f; 
    public float maxDonmeAcisi = 30f;

    public Transform agirlikMerkezi; 
    private Rigidbody rb;

    private float dikeyGirdi;
    private float yatayGirdi;
    private bool elFreni; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (agirlikMerkezi != null)
        {
            rb.centerOfMass = agirlikMerkezi.localPosition;
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ArabayiDuzelt();
        }

        
        elFreni = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        dikeyGirdi = Input.GetAxisRaw("Vertical"); 
        yatayGirdi = Input.GetAxis("Horizontal"); 

        
        float hiz = transform.InverseTransformDirection(rb.linearVelocity).z;

        
        if (elFreni)
        {
            FrenYap(frenGucu);
            MotorGucuVer(0f);
        }
        
        else
        {
            if (dikeyGirdi > 0) 
            {
                if (hiz < -0.5f) 
                {
                    FrenYap(frenGucu);
                    MotorGucuVer(0f);
                }
                else 
                {
                    FrenYap(0f);
                    MotorGucuVer(dikeyGirdi * motorGucu);
                }
            }
            else if (dikeyGirdi < 0) 
            {
                if (hiz > 0.5f) 
                {
                    FrenYap(frenGucu);
                    MotorGucuVer(0f);
                }
                else 
                {
                    FrenYap(0f);
                    MotorGucuVer(dikeyGirdi * geriVitesGucu);
                }
            }
            else 
            {
                
                FrenYap(3000f); 
                MotorGucuVer(0f);

                
                if (Mathf.Abs(hiz) < 0.2f)
                {
                    
                    rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
                }
            }
        }

        
        onSolCollider.steerAngle = yatayGirdi * maxDonmeAcisi;
        onSagCollider.steerAngle = yatayGirdi * maxDonmeAcisi;

        TekerlegiGuncelle(onSolCollider, onSolMesh);
        TekerlegiGuncelle(onSagCollider, onSagMesh);
        TekerlegiGuncelle(arkaSolCollider, arkaSolMesh);
        TekerlegiGuncelle(arkaSagCollider, arkaSagMesh);
    }

    void FrenYap(float guc)
    {
        onSolCollider.brakeTorque = guc;
        onSagCollider.brakeTorque = guc;
        arkaSolCollider.brakeTorque = guc;
        arkaSagCollider.brakeTorque = guc;
    }

    void MotorGucuVer(float guc)
    {
        arkaSolCollider.motorTorque = guc;
        arkaSagCollider.motorTorque = guc;
    }

    void ArabayiDuzelt()
    {
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void TekerlegiGuncelle(WheelCollider collider, Transform mesh)
    {
        Vector3 pozisyon;
        Quaternion rotasyon;
        collider.GetWorldPose(out pozisyon, out rotasyon);
        mesh.position = pozisyon;
        mesh.rotation = rotasyon;
    }
}