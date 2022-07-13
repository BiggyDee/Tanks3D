using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviourPun, IPunObservable
{
    #region PUBLIC
    public int m_PlayerNumber = 1;       
    public GameObject m_Shell;            
    public Transform m_FireTransform;    
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;
    public GameObject m_ShotButton;
    public bool shotgunOn, minigunOn, fireOn, iseOn, gunX2On, reflectMinigun, cantShoot;
    public GameObject gameObj, shotgunPartl, minigunPartl, gunX2Partl;
    #endregion

    #region PRIVATE
    int minigunBullets, canShootTimer;
    
    Bufs bufs;

    ShellExplosion shellExplosion;
    TankHealth tankHealth;
    
    #endregion


    private void Awake()
    {
        m_ShotButton.SetActive(photonView.IsMine);
        shellExplosion = Resources.Load<ShellExplosion>("Shell");
        fireOn = false;
        iseOn = false;
        shellExplosion.gameObject.tag = "Shell";

    }

    public void OnShotButtonClick()
    {
       
        Fire();
    }
   
    void MinigunFire()
    {
        GameObject shellObj = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position, m_FireTransform.rotation) as GameObject;
        Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
        shellRigidbody.velocity = m_FireTransform.forward * ShellExplosion.m_Speed;
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        minigunBullets++;
        
        Invoke("MinigunFire",0.3f);
        if (minigunBullets >= 6)
        {
            minigunOn = false;
            CancelInvoke("MinigunFire");
            minigunBullets = 0;
            minigunPartl.gameObject.SetActive(false);
            reflectMinigun = false;
            CanShoot();
            tankHealth.bufOn = false;
        }
    }
    void CanShoot()
    {
        cantShoot = true;
        canShootTimer++;
        Invoke("CanShoot", 0.5f);
        if (canShootTimer >= 4)
        {
            cantShoot = false;
            CancelInvoke("CanShoot");
        }
        
    }
    private void OnTriggerEnter(Collider hit)
    {
        Transform targetRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        tankHealth = targetRigidbody.GetComponent<TankHealth>();
        if (hit.tag == "Minigun" && tankHealth.bufOn == false)
        {
            minigunOn = true;
            reflectMinigun = true;
            tankHealth.bufOn = true;
        }
        else if( hit.tag == "Shotgun" && tankHealth.bufOn == false)
        {
            shotgunOn = true;
            tankHealth.bufOn = true;
        }
        else if(hit.tag == "Gunx2" && tankHealth.bufOn == false)
        {
            gunX2On = true;
            tankHealth.bufOn = true;
        }
        else if (hit.tag == "Fire" && tankHealth.bufOn == false)
        {
            fireOn = true;
            tankHealth.bufOn = true;
            shellExplosion.gameObject.tag = "FireShell";
        }
        else if (hit.tag == "Ise" && tankHealth.bufOn == false)
        {
            iseOn = true;
            tankHealth.bufOn = true;
            shellExplosion.gameObject.tag = "IseShell";
        }
       
    }

    private void Fire() // Choosing the type of shooting depending on the buff
    {
        Transform targetRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        tankHealth = targetRigidbody.GetComponent<TankHealth>();
        if (!photonView.IsMine)
            return;
        if(shotgunOn == false && minigunOn == false && cantShoot == false && iseOn == false && fireOn == false)
        {
            GameObject shellObj = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position, m_FireTransform.rotation) as GameObject;
            Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
            shellRigidbody.velocity = m_FireTransform.forward * ShellExplosion.m_Speed;
            shellExplosion.m_MaxDamage = 100f;
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
        }
        else if(shotgunOn == true )
        {
            shotgunPartl.gameObject.SetActive(true);
            GameObject shellObj = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position, m_FireTransform.rotation) as GameObject;
            Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
            shellRigidbody.velocity = m_FireTransform.forward * ShellExplosion.m_Speed;
            GameObject shellObj2 = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position = new Vector3(m_FireTransform.position.x + 0.6f, m_FireTransform.position.y, m_FireTransform.position.z + 0.2f), m_FireTransform.rotation) as GameObject;
            Rigidbody shellRigidbody2 = shellObj2.GetComponent<Rigidbody>();
            shellRigidbody2.velocity = m_FireTransform.forward * ShellExplosion.m_Speed;
            GameObject shellObj3 = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position = new Vector3(m_FireTransform.position.x - 0.6f, m_FireTransform.position.y, m_FireTransform.position.z + 0.2f), m_FireTransform.rotation) as GameObject;
            Rigidbody shellRigidbody3 = shellObj3.GetComponent<Rigidbody>();
            shellRigidbody3.velocity = m_FireTransform.forward * ShellExplosion.m_Speed;
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
            shotgunPartl.gameObject.SetActive(false);
            shotgunOn = false;
            tankHealth.bufOn = false;
        }
        else if(gunX2On == true)
        {
            gunX2Partl.gameObject.SetActive(true);
            GameObject shellObj = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position, m_FireTransform.rotation) as GameObject;
            Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
            shellRigidbody.velocity = m_FireTransform.forward * ShellExplosion.m_Speed;
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
            shellExplosion.m_MaxDamage = 100f;
            gunX2On = false;
            tankHealth.bufOn = false;
            gunX2Partl.gameObject.SetActive(false);
        }
        else if(minigunOn == true)
        {

            MinigunFire();
            minigunPartl.gameObject.SetActive(true);
            
        }
        else if (iseOn == true)
        {
            
            GameObject shellObj = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position, m_FireTransform.rotation) as GameObject;
            Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
            shellRigidbody.velocity = m_FireTransform.forward * ShellExplosion.m_Speed;
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
          
        
            tankHealth.bufOn = false;
            shellExplosion.gameObject.tag = "Shell";
            iseOn = false;
        }
        else if (fireOn == true)
        {
            
            GameObject shellObj = PhotonNetwork.Instantiate(m_Shell.name, m_FireTransform.position, m_FireTransform.rotation) as GameObject;
            Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
            shellRigidbody.velocity = m_FireTransform.forward * ShellExplosion.m_Speed;
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();

            fireOn = false;
            tankHealth.bufOn = false;
            shellExplosion.gameObject.tag = "Shell";
        }
       
        tankHealth.bufOn = false;
        tankHealth.fireOn = false;
        tankHealth.iseOn = false;
      

        
    }
  
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(shotgunPartl.activeSelf);
            stream.SendNext(minigunPartl.activeSelf);
            stream.SendNext(gunX2Partl.activeSelf);
           
        }
        else if (stream.IsReading)
        {
            shotgunPartl.SetActive((bool)stream.ReceiveNext());
            minigunPartl.SetActive((bool)stream.ReceiveNext());
            gunX2Partl.SetActive((bool)stream.ReceiveNext());

           
        }
    }
}