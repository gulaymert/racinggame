using UnityEngine;
using UnityEngine.AI;
public class opponent : MonoBehaviour
{
    [Header("Rakip Hedefleri")]
    public Transform hedefNokta;
    public Transform oyuncu; 

    [Header("Rakip Hız Ayarları")]
    public float maksimumHiz = 45f;    
    public float hizlanma = 30f;       

    private NavMeshAgent agent;
    private Rigidbody rb;
    private Rigidbody oyuncuRb; 

    private bool saldiriyorMu = false;
    private float sonSaldiriZamani = 0f;
    private float saldiriSuresi = 0.8f;       
    private float saldiriBeklemeSuresi = 5f;  
    private float minimumOyuncuHizi = 8f;     

    private Vector3 oyuncuEskiHizi; 
    private float sonKazaZamani = 0f;

    void Start()
    {
        agent = (NavMeshAgent)GetComponent(typeof(NavMeshAgent));
        rb = (Rigidbody)GetComponent(typeof(Rigidbody));
        rb.isKinematic = true; 

        if (oyuncu == null)
        {
            oyuncu = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        if (oyuncu != null)
        {
            oyuncuRb = (Rigidbody)oyuncu.GetComponent(typeof(Rigidbody));
        }
    }

    void Update()
    {
        if (!agent.isActiveAndEnabled || oyuncu == null) return;

        agent.speed = maksimumHiz;
        agent.acceleration = hizlanma;

        if (oyuncuRb != null)
        {
            oyuncuEskiHizi = oyuncuRb.linearVelocity;
        }

        if (saldiriyorMu)
        {
            if (Time.time > sonSaldiriZamani + saldiriSuresi)
            {
                saldiriyorMu = false;
            }
            else
            {
                agent.SetDestination(oyuncu.position);
                return; 
            }
        }

        bool saldiriHazir = Time.time > (sonSaldiriZamani + saldiriSuresi + saldiriBeklemeSuresi);
        float oyuncuHizi = (oyuncuRb != null) ? oyuncuRb.linearVelocity.magnitude : 0f;
        
        Vector3 oyuncuyaDogru = oyuncu.position - transform.position;
        float mesafe = oyuncuyaDogru.magnitude;
        float aci = Vector3.Angle(transform.forward, oyuncuyaDogru);

        if (saldiriHazir && mesafe < 15f && aci > 30f && aci < 150f && oyuncuHizi > minimumOyuncuHizi)
        {
            saldiriyorMu = true;
            sonSaldiriZamani = Time.time;
        }
        else
        {
            if (hedefNokta != null)
            {
                agent.SetDestination(hedefNokta.position);
            }
        }
    }

    void OnTriggerEnter(Collider digeri)
    {
        if (Time.time < sonKazaZamani + 0.5f) return;

        if (digeri.CompareTag("Player"))
        {
            if (agent.isActiveAndEnabled)
            {
                agent.enabled = false;
                rb.isKinematic = false; 
                
                
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

            sonKazaZamani = Time.time;

            Vector3 oyuncuSuAnkiHizi = oyuncuEskiHizi;
            if (oyuncuRb != null) oyuncuSuAnkiHizi = oyuncuRb.linearVelocity;

            float carpmaSiddeti = (oyuncuSuAnkiHizi - agent.velocity).magnitude;
            
            if (carpmaSiddeti < 2f) 
            {
                Invoke("YarisaGeriDon", 0.5f);
                return;
            }

            Vector3 rakiptenOyuncuya = (oyuncu.position - transform.position).normalized;
            rakiptenOyuncuya.y = 0f; 
            rakiptenOyuncuya.Normalize();
            Vector3 oyuncudanRakibe = -rakiptenOyuncuya;

            float rakipSaldiriGucu = Vector3.Dot(transform.forward, rakiptenOyuncuya) * agent.velocity.magnitude;
            float oyuncuSaldiriGucu = Vector3.Dot(oyuncu.forward, oyuncudanRakibe) * (oyuncuSuAnkiHizi.magnitude + 0.1f);

            bool rakipSaldirdi = (rakipSaldiriGucu > oyuncuSaldiriGucu);
            carpmaSiddeti = Mathf.Clamp(carpmaSiddeti, 0f, 40f);

            if (rakipSaldirdi)
            {
                if (oyuncuRb != null)
                {
                    float hizKoruCarpani = Mathf.Clamp(1f - (carpmaSiddeti * 0.015f), 0.6f, 0.95f);
                    oyuncuRb.linearVelocity = oyuncuSuAnkiHizi * hizKoruCarpani;

                    float yanDarbeYonu = Vector3.Dot(oyuncu.right, oyuncudanRakibe) > 0 ? -1f : 1f; 
                    Vector3 savrulmaYonu = oyuncu.right * yanDarbeYonu;
                    savrulmaYonu.y = 0f;
                    
                    oyuncuRb.AddForce(savrulmaYonu * (carpmaSiddeti * 300f), ForceMode.Impulse);
                }
            }
            else
            {
                Vector3 savrulmaYonu = transform.forward;
                savrulmaYonu.y = 0f; 

                rb.AddForce(savrulmaYonu * (carpmaSiddeti * 450f), ForceMode.Impulse);
                
                float yanDarbeYonu = Vector3.Dot(transform.right, rakiptenOyuncuya) > 0 ? 1f : -1f;
                rb.AddTorque(Vector3.up * (yanDarbeYonu * carpmaSiddeti * 200f), ForceMode.Impulse);

                if (oyuncuRb != null)
                {
                    oyuncuRb.linearVelocity = oyuncuSuAnkiHizi; 
                }
            }

            Invoke("YarisaGeriDon", 1.5f);
        }
    }

    void YarisaGeriDon()
    {
        
        rb.constraints = RigidbodyConstraints.None;
        
        rb.isKinematic = true; 
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        
        agent.enabled = true;
        agent.Warp(transform.position); 
        agent.velocity = Vector3.zero;
    }
}