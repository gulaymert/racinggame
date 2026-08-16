using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class finish : MonoBehaviour
{
    [Header("Arayüz (UI) Elemanları")]
    public GameObject sonucPaneli;
    
    
    public TextMeshProUGUI sonucYazisi; 

    private bool yarisBitti = false;

    void Start()
    {
        Time.timeScale = 1f; 
        if (sonucPaneli != null) sonucPaneli.SetActive(false);
    }

    void OnTriggerEnter(Collider digeri)
    {
        if (yarisBitti) return;

        if (digeri.CompareTag("Player"))
        {
            YarisiBitir("YOU WIN!", Color.green);
        }
        else if (digeri.CompareTag("Rakip"))
        {
            YarisiBitir("YOU LOSE!", Color.red);
        }
    }

    void YarisiBitir(string mesaj, Color yaziRengi)
    {
        yarisBitti = true;
        
        sonucPaneli.SetActive(true);
        sonucYazisi.text = mesaj;
        sonucYazisi.color = yaziRengi;
        
        Time.timeScale = 0f; 
    }

    public void YenidenBaslat()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}