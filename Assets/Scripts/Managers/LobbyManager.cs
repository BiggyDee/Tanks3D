using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject m_UserNameScreen, m_ConnectScreen, m_RoomListScreen, m_RoomNameScreen;

    [SerializeField]
    private GameObject m_CreateUserNameButton;

    [SerializeField]
    private InputField m_UserNameField, m_RoomNameInput, m_UserNameChangeField;

    [SerializeField]
    private GameObject m_MessagePanel;

    [SerializeField]
    private Text m_MessageText;

    [SerializeField]
    private GameObject m_RoomButtonPrefab;
    private bool activeSkin;


    private Color m_DefaultTextFieldColor;
    private int m_MinNameLength;
    public Dropdown dropdown;
    public Text textNick;
    public Button nameChangeButton;
    public Text moneyText;
    public GameObject goldPlate;
    public Button shopButton;
    
    public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
   
   
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "0.0.1";
        textNick.text = null;
        m_UserNameScreen.SetActive(false);
        m_ConnectScreen.SetActive(false);
        m_RoomListScreen.SetActive(false);
        nameChangeButton.gameObject.SetActive(false);
        m_MessagePanel.SetActive(false);
        m_MessageText.text = string.Empty;

        m_DefaultTextFieldColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        m_MinNameLength = 3;




        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.SetInt("Gold", 100000);

    }   


    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby(TypedLobby.Default);

        base.OnConnectedToMaster();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to Lobby");
        if (PlayerPrefs.HasKey("Nick"))
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("Nick");
            textNick.text = PhotonNetwork.NickName;
            goldPlate.SetActive(true);
            shopButton.gameObject.SetActive(true);
            if(PlayerPrefs.GetInt("Gold") != 0)
            {
                moneyText.text = PlayerPrefs.GetInt("Gold").ToString();
            }
           
            m_ConnectScreen.SetActive(true);
            nameChangeButton.gameObject.SetActive(true);
        }
        else
        {
            m_UserNameScreen.SetActive(true);
        }
        
        

        base.OnJoinedLobby();
    }


    private void ClearRoomList(List<RoomInfo> roomList)
    {
      
        Transform content = m_RoomListScreen.transform.Find("Scroll View/Viewport/Content");
        foreach (Transform obj in content)
        {
          
                Destroy(obj.gameObject);
            
                
            
           
        }
            
        

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Transform content = m_RoomListScreen.transform.Find("Scroll View/Viewport/Content");
        
        foreach (RoomInfo roomInfo in roomList)
        {
            Debug.Log(roomInfo.ToStringFull());
           if (roomInfo.PlayerCount != 0)
           {
           
             GameObject newRoomButton = Instantiate(m_RoomButtonPrefab, content) as GameObject;
             newRoomButton.transform.Find("Name").GetComponent<Text>().text = roomInfo.Name;
             newRoomButton.transform.Find("Players").GetComponent<Text>().text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;
             newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { OnJoinRoomButtonClick(newRoomButton.transform); });
           }
           else 
           {
                ClearRoomList(roomList);
           }

          

        }
        
    }
    

    #region UIMethods

 

    public void OnCreateNameButtonClick()
    {
        m_UserNameField.text.Trim();
        if (m_UserNameField.text.Length < m_MinNameLength)
        {
            ShowMessage("The name must be at least " + m_MinNameLength + " letters long");
            m_UserNameField.textComponent.color = Color.red;
            return;
        }

        PhotonNetwork.NickName = m_UserNameField.text;
        PlayerPrefs.SetString("Nick", PhotonNetwork.NickName);
        textNick.text = PhotonNetwork.NickName;
        nameChangeButton.gameObject.SetActive(true);
        goldPlate.SetActive(true);
        shopButton.gameObject.SetActive(true);
        m_UserNameScreen.SetActive(false);
        m_ConnectScreen.SetActive(true);
    }
    public void ChangeNickname()
    {
        PhotonNetwork.NickName = m_UserNameChangeField.text;
        PlayerPrefs.SetString("Nick", PhotonNetwork.NickName);
        textNick.text = PhotonNetwork.NickName;
    }

    public void OnUserNameFieldValueChanged()
    {
        m_UserNameField.textComponent.color = m_DefaultTextFieldColor;
    }

    private void ShowMessage(string messageText)
    {
        m_MessagePanel.SetActive(true);
        m_MessageText.text = messageText;
    }

    public void HideMessage()
    {
        m_MessagePanel.SetActive(false);
        m_MessageText.text = string.Empty;
    }

    public void OnCreateRoomClick()
    {
        m_ConnectScreen.SetActive(false);
        m_RoomNameScreen.SetActive(true);
    }
    
    public void CreateRoomName()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add(1, "Level_1");
        roomOptions.CustomRoomProperties.Add(2, "Level_2");
        roomOptions.CustomRoomProperties.Add(3, "Level_3");
        roomOptions.EmptyRoomTtl = 0;   
        PhotonNetwork.CreateRoom(m_RoomNameInput.text, roomOptions, null);

        

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room");
        Debug.Log(dropdown.value);
        PhotonNetwork.LoadLevel(dropdown.value + 1);

     }


    public void OnJoinRoomClick()
    {
        m_ConnectScreen.SetActive(false);
        m_RoomListScreen.SetActive(true);
    }

    public void OnBackButtonClock()
    {
        m_ConnectScreen.SetActive(true);
        m_RoomListScreen.SetActive(false);
        m_RoomNameScreen.SetActive(false);
    }

    private void OnJoinRoomButtonClick(Transform button)
    {
        Debug.Log("JOINING ROOM @ " + Time.time);
        string roomName = button.transform.Find("Name").GetComponent<Text>().text;
        PhotonNetwork.JoinRoom(roomName);
    }

    #endregion


  
}
