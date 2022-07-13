using System;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

[Serializable]
public class TankManager
{
    [HideInInspector] public Color m_PlayerColor;
          
    [HideInInspector] public string m_PlayerName;             
    [HideInInspector] public string m_ColoredPlayerText;
    public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;
    public Transform m_SpawnPoint_1;
    public Transform m_SpawnPoint_2;


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;
    private GameManager m_GameManager;
    private Transform m_SpawnPoint;

    public void Setup()
    {
       
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerName + "</color>";
        Debug.Log(m_Movement);
      
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        
        m_SpawnPoint = (PhotonNetwork.IsMasterClient) ? m_SpawnPoint_1 : m_SpawnPoint_2;
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;
        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
