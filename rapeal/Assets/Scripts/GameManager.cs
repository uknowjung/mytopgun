using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }

            return m_instance;
        }
    }

    private static GameManager m_instance;

    public GameObject playerPrefab;
    public Slider lifeBarPrefab;
    public Text gameEndTextPrefab;
    public bool gameEnd = false;
    private Slider lifeBar;
    private Vector2 lifeBarPos = new Vector2(0.5f, 0.5f);
    private Health health;
    private Text gameEndText;
    private Vector2 gameEndTextPos = new Vector2(0.5f, 0.5f);
    private PhotonView pv;
    public float timeStart = 0f;
    public float timeCurrent;
    private float timeMax = 5f;
    private bool onlyOnce = false;
    private UDPReceive udpReceive;
    private UdpClient udpClient;

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        udpReceive = GameObject.Find("UDPReceive").GetComponent<UDPReceive>();

        Vector3 randomSpawnPos = Random.insideUnitSphere * 20f;
        randomSpawnPos.y = 0f;

        GameObject go = PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
        pv = go.GetComponent<PhotonView>();
        if (!pv.IsMine)
        {
            go.transform.Find("cam1").gameObject.SetActive(false);
            go.transform.Find("cam3").gameObject.SetActive(false);
        }

        float fScaleWidth = ((float)Screen.width / (float)Screen.height) / ((float)16 / (float)9);

        lifeBar = Instantiate(lifeBarPrefab);
        lifeBar.transform.SetParent(GameObject.Find("Canvas").transform);
        lifeBar.GetComponent<RectTransform>().anchorMin = lifeBarPos;
        lifeBar.GetComponent<RectTransform>().anchorMax = lifeBarPos;
        lifeBar.GetComponent<RectTransform>().pivot = lifeBarPos;
        lifeBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 292f);
        lifeBar.maxValue = 1;
        lifeBarPos = lifeBar.GetComponent<RectTransform>().localPosition;
        lifeBarPos.x = lifeBarPos.x * fScaleWidth;
        lifeBar.GetComponent<RectTransform>().localPosition = new Vector3(lifeBarPos.x, lifeBarPos.y);
        health = go.GetComponent<Health>();

        gameEndText = Instantiate(gameEndTextPrefab);
        gameEndText.transform.SetParent(GameObject.Find("Canvas").transform);
        gameEndText.GetComponent<RectTransform>().anchorMin = gameEndTextPos;
        gameEndText.GetComponent<RectTransform>().anchorMax = gameEndTextPos;
        gameEndText.GetComponent<RectTransform>().pivot = gameEndTextPos;
        gameEndText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        gameEndTextPos = lifeBar.GetComponent<RectTransform>().localPosition;
        gameEndTextPos.x = gameEndTextPos.x * fScaleWidth;
        lifeBar.GetComponent<RectTransform>().localPosition = new Vector3(gameEndTextPos.x, gameEndTextPos.y);
        gameEndText.gameObject.SetActive(false);
    }

    private void Update()
    {
        udpClient = udpReceive.client;

        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        int length = playerList.Length;

        for (int i = 0; i < length; ++i)
        {
            float playerHealth = playerList[i].GetComponent<Health>().health;
            Debug.LogFormat("Health{0}: {1}\t", i + 1, playerHealth);
            if (playerHealth <= 0f)
            {
                if (timeStart == 0f) { timeStart = Time.time; }
                gameEnd = true;
            }
        }

        lifeBar.value = health.health;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }

        if (gameEnd)
        {
            udpClient.Close();
            if (health.health <= 0)
            {
                gameEndText.text = "Game End\nYou Died";
                gameEndText.gameObject.SetActive(true);
            }
            else
            {
                gameEndText.text = "Game End\nYou Survived";
                gameEndText.gameObject.SetActive(true);
            }

            timeCurrent = Time.time - timeStart;
            if (timeCurrent > timeMax && !onlyOnce)
            {
                onlyOnce = true;
                PhotonNetwork.LeaveRoom();
            }
        }
    }

    private void LateUpdate()
    {
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
}
