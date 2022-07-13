using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour, IPunObservable
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    public float m_CurrentHealth;  
    public bool m_Dead;
 
    private int iseTimer, fireTimer, shieldTimer;
    public bool nameShell, fireOn, iseOn, shieldOn, bufOn;
    public GameObject isePartl, firePartl, medPartl, shieldPartl;
    Bufs bufs;
    ShellExplosion shellExplosion;
   
    public GameManager manager;
    public TankMovement tankMovement;
    public TankShooting tankShooting;
    
    private void Awake()
    {
       
        
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
        bufOn = false;

        bufs = Resources.Load<Bufs>("Cube");
      
        shellExplosion = Resources.Load<ShellExplosion>("Shell");
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Rigidbody targetRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        tankMovement = targetRigidbody.GetComponent<TankMovement>();
        tankShooting = targetRigidbody.GetComponent<TankShooting>();
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    private void FixedUpdate()
    {
        bufs = Resources.Load<Bufs>("Cube");

       
         if(bufs.medOn == true)
        {
           
            if (m_CurrentHealth < m_StartingHealth)
            {
                medPartl.gameObject.SetActive(true);
                m_CurrentHealth += 25f;
                SetHealthUI();
            }

            bufs.medOn = false;
            medPartl.gameObject.SetActive(false);
        }

    }

   public void FireDamage() // If triggered with fire bullet
    {
        m_CurrentHealth -= 0.5f;
       
        SetHealthUI();
        fireTimer++;
        Invoke("FireDamage", 0.2f);
        if (fireTimer >= 24)
        {
            firePartl.gameObject.SetActive(false);
            fireOn = false;
            CancelInvoke("FireDamage");
            fireTimer = 0;
            
        }
        
    }
   public void IseDamage() // If triggered with ise bullet
    {
        
        tankMovement.IseOnDamage();
        iseTimer++;
        Invoke("IseDamage", 1f);
        if (iseTimer >= 5)
        {
            isePartl.gameObject.SetActive(false);
            iseOn = false;
            tankMovement.IseOffDamage();
            CancelInvoke("IseDamage");
            iseTimer = 0;
        }
    }

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        m_CurrentHealth -= amount;
        if(fireOn == true)
        {
           
            bufOn = false;
        }
        else if(iseOn == true)
        {
           
            bufOn = false;
        }

        nameShell = true;
        SetHealthUI();

        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "Med" && m_CurrentHealth < m_StartingHealth)
        {
            m_CurrentHealth += 25f;
            SetHealthUI();
        }
       
        else if (hit.tag == "Shield")
        {
            shieldOn = true;
            shieldPartl.gameObject.SetActive(true);
            ShieldTriggered();
        }
        else if (hit.tag == "Spike")
        {
            TakeDamage(40);
        }
        else if (hit.tag == "ArtBoom")
        {
            TakeDamage(60);
        }
        else if (hit.tag == "Gunx2")
        {
            TakeDamage(100);
        }


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "FireShell")
        {
            Debug.Log("ogoniok");
            firePartl.gameObject.SetActive(true);
            FireDamage();
            fireOn = true;
            bufOn = true;
        }
        else if (collision.collider.tag == "IseShell")
        {
            Debug.Log("ledok");
            isePartl.gameObject.SetActive(true);
            IseDamage();
            iseOn = true;
            bufOn = true;
        }
    }
    void ShieldTriggered()
    {
       
            shellExplosion.m_MaxDamage = 0f;
            shieldTimer++;
            Invoke("ShieldTriggered", 0.5f);
            if(shieldTimer >= 5)
            {
                shieldTimer = 0;
                CancelInvoke("ShieldTriggered");
                shieldPartl.gameObject.SetActive(false);
            }
       
    }
    public void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);

    }


    public void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        
        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();


        if (PhotonNetwork.NickName == manager.m_GameWinnerText.text && manager.m_MyLives >= 0)
        {

            manager.m_GoldPlace.SetActive(true);
            manager.moneyScript.EarnGold();
        }
        this.gameObject.SetActive(false);
       

    }
    public void GameEnd()
    {
        

        

        if (manager.m_MyLives <= 0)
        {

            manager.endLvl.SetActive(true);
            
            
            if(manager.m_FirstScoreInt <= 0)
            {
                manager.m_GameWinnerText.text = manager.m_SecondPlayerName;
                
            }
            else if(manager.m_SecondScoreInt <= 0)
            {
                manager.m_GameWinnerText.text = manager.m_FirstPlayerName;
            }

            tankMovement.m_FixedJoystickObject.SetActive(false);
            tankShooting.m_ShotButton.SetActive(false);

        }
        
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_CurrentHealth);
            stream.SendNext(firePartl.activeSelf);
            stream.SendNext(isePartl.activeSelf);
            stream.SendNext(medPartl.activeSelf);
            stream.SendNext(shieldPartl.activeSelf);
            stream.SendNext(this.gameObject.activeSelf);
            stream.SendNext(manager.endLvl.activeSelf);
            stream.SendNext(manager.m_GameWinnerText.text);
            stream.SendNext(tankMovement.m_FixedJoystickObject.activeSelf);
            stream.SendNext(tankShooting.m_ShotButton.activeSelf);
            
        }
        else if (stream.IsReading)
        {
            m_CurrentHealth = (float)stream.ReceiveNext();
            SetHealthUI();
            firePartl.SetActive((bool)stream.ReceiveNext());
            isePartl.SetActive((bool)stream.ReceiveNext());
            medPartl.SetActive((bool)stream.ReceiveNext());
            shieldPartl.SetActive((bool)stream.ReceiveNext());
            this.gameObject.SetActive((bool)stream.ReceiveNext());
            manager.endLvl.SetActive((bool)stream.ReceiveNext());
            manager.m_GameWinnerText.text = (string)stream.ReceiveNext();
            tankMovement.m_FixedJoystickObject.SetActive((bool)stream.ReceiveNext());
            tankShooting.m_ShotButton.SetActive((bool)stream.ReceiveNext());
            
        }
    }
}