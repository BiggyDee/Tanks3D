using Photon.Pun;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region PUBLIC
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public Text m_MessageText;
    public Text m_PingText;
    public Text m_NameFirst;
    public Text m_NameSecond;
    public Text m_LivesText;
    public Transform m_SpawnPoint_1;
    public Transform m_SpawnPoint_2;
    public GameObject m_TankPrefab;
    public TankManager m_Tank;
    public Transform currentSpawnPoint;
    public GameObject endLvl;
    public int m_FirstScoreInt;
    public int m_SecondScoreInt;
    public int m_MyLives;
    public string m_FirstPlayerName;
    public string m_SecondPlayerName;
    public Text m_GameWinnerText;
    public GameObject m_GoldPlace;
    public MoneyScript moneyScript;
    public GameObject[] tanksList;
    
    #endregion

    #region PRIVATE
    private int m_RoundNumber = 0;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private TankHealth tankHealth;
    #endregion


    private void Awake()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
    }

    [System.Obsolete]
    private void Start()
    {
       
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        m_FirstScoreInt = 10;
        m_SecondScoreInt = 10;
        m_MyLives = 10;
        m_LivesText.text = "Lives: " + m_MyLives;
       
        SpawnAllTanks();
        
        StartCoroutine(GameLoop());
        

    }


    private void SpawnAllTanks()
    {
        currentSpawnPoint = (PhotonNetwork.IsMasterClient) ? m_SpawnPoint_1 : m_SpawnPoint_2;

        ChangeCharacter();
       




        Rigidbody targetRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        

        tankHealth = targetRigidbody.GetComponent<TankHealth>();
     
        m_Tank.m_PlayerColor = (PhotonNetwork.IsMasterClient) ? Color.red : Color.blue;
        m_Tank.m_PlayerName = PhotonNetwork.LocalPlayer.NickName;

        m_FirstPlayerName = PhotonNetwork.PlayerList[0].NickName;
        m_SecondPlayerName = PhotonNetwork.PlayerList[1].NickName;
        
       
       
        m_Tank.Setup();
        
    }

    public void ChangeCharacter()
    {
        if (Convert.ToBoolean(PlayerPrefs.GetInt("firstSkin")) == true)
        {
            m_Tank.m_Instance = PhotonNetwork.Instantiate(tanksList[0].name, currentSpawnPoint.position, currentSpawnPoint.rotation) as GameObject;
        }
        else if (Convert.ToBoolean(PlayerPrefs.GetInt("secondSkin")) == true)
        {
            m_Tank.m_Instance = PhotonNetwork.Instantiate(tanksList[1].name, currentSpawnPoint.position, currentSpawnPoint.rotation) as GameObject;
        }
        else if (Convert.ToBoolean(PlayerPrefs.GetInt("thirdSkin")) == true)
        {
            m_Tank.m_Instance = PhotonNetwork.Instantiate(tanksList[2].name, currentSpawnPoint.position, currentSpawnPoint.rotation) as GameObject;
        }
        else if(Convert.ToBoolean(PlayerPrefs.GetInt("firstSkin")) == false && Convert.ToBoolean(PlayerPrefs.GetInt("secondSkin")) == false && Convert.ToBoolean(PlayerPrefs.GetInt("thirdSkin")) == false)
        {
            m_Tank.m_Instance = PhotonNetwork.Instantiate(tanksList[0].name, currentSpawnPoint.position, currentSpawnPoint.rotation) as GameObject;
        }

       
    }

    

    [System.Obsolete]
    private IEnumerator GameLoop()
    {
        StartCoroutine(GameLoop());
        yield return StartCoroutine(RoundStarting());
        //yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        //if (m_GameWinner != null)
        //{
        //    Application.LoadLevel(Application.loadedLevel);
        //}
        //else
        //{
            
        //}
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
       

        m_RoundNumber++;
       // m_MessageText.text = "ROUND " + m_RoundNumber;
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        

        m_MessageText.text = string.Empty;

        while (true/*OneTankLeft()*/)
        {
            m_PingText.text = "Ping: " + PhotonNetwork.GetPing();
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
       

        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
        {
            m_RoundWinner.m_Wins++;
        }

        m_GameWinner = GetGameWinner();

        //m_MessageText.text = EndMessage();

        yield return m_EndWait;
    }


    //private bool OneTankLeft()
    //{

    //    if (!m_TankPrefab.active)
    //    {

    //        return numTanksLeft <= 1;
    //    }
    //    else
    //    {
    //        return numTanksLeft > 1;

    //    }


    //}


    private TankManager GetRoundWinner()
    {
        return null;
    }


    private TankManager GetGameWinner()
    {
        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";
        return message;
    }

    private void FixedUpdate()
    {
       
        ResetAllTanks();

    }
    
    public void ResetAllTanks()
    {
        
        if(tankHealth.m_Dead == true && PhotonNetwork.IsMasterClient == true )
        {
            
            
            m_FirstScoreInt -= 1;
            m_MyLives = m_FirstScoreInt;
            m_LivesText.text = "Lives: " + m_MyLives;
            //m_ScoreFirst.text = m_FirstPlayerName + ": " + m_MyLives;
            // m_ScoreSecond.text = m_SecondPlayerName + ": " + m_SecondScoreInt;
            //photonView.SendMessage(m_SecondScoreInt.ToString(), m_SecondScoreInt);
            //photonView.RPC("ResetAllTanks", RpcTarget.All);
            
            m_Tank.Reset();
        }
        else if (tankHealth.m_Dead == true && PhotonNetwork.IsMasterClient == false)
        {
            m_SecondScoreInt -= 1;
            m_MyLives = m_SecondScoreInt;
            m_LivesText.text = "Lives: " + m_MyLives;
            //m_ScoreFirst.text = m_FirstPlayerName + ": " + m_FirstScoreInt;
            //m_ScoreSecond.text = m_SecondPlayerName + ": " + m_MyLives;
            //photonView.RPC("ResetAllTanks", RpcTarget.All);

            
            m_Tank.Reset();
        }
        m_NameFirst.text ="1: " + m_FirstPlayerName;
        m_NameSecond.text = "2: " + m_SecondPlayerName;

        tankHealth.GameEnd();
       
    }
    
    
    public void PlayerList()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Debug.Log(p.NickName);
            
            
        }
    }

    

    
   
    public void Leave()
    {
        
        
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // When current player left the gameRoom
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
        m_SecondPlayerName = PhotonNetwork.PlayerList[1].NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }
    

}