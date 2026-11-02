using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SimulationController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject timeUpPanel;
    public GameObject safeDestinationPanel;

    [Header("UI Texts (TextMeshPro)")]
    public TextMeshProUGUI floodAlertText;
    public TextMeshProUGUI bridgeCollapseText;
    public TextMeshProUGUI countdownText;

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera firstPersonCamera;

    [Header("Bridge Setup")]
    public GameObject bridge;  // Parent object of the bridge
    public Collider bridgeTrigger;

    [Header("First Person Controller")]
    public GameObject firstPersonController;

    [Header("Water Settings")]
    public WaterRise waterRiseScript;  // Reference to your WaterRise script

    private float timer = 600f; // 10 minutes
    private bool timerRunning = false;
    private bool gameEnded = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        
        startPanel.SetActive(true);
        timeUpPanel.SetActive(false);
        safeDestinationPanel.SetActive(false);
        floodAlertText.gameObject.SetActive(false);
        bridgeCollapseText.gameObject.SetActive(false);
        firstPersonController.SetActive(false);
        countdownText.gameObject.SetActive(false);
        
        //mainCamera.enabled = true;
        //firstPersonCamera.enabled = false;

        DisableBridgePhysics();
        Time.timeScale = 1;
    }

    public void OnStartButtonClicked()
    {
        Debug.Log("Start Button Clicked");

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        //firstPersonController.SetActive(true);
        startPanel.SetActive(false);
        countdownText.gameObject.SetActive(true);

        //mainCamera.enabled = false;
        //firstPersonCamera.enabled = true;

        // Start the water rising
        waterRiseScript.StartRising();  // Starts the water rise

        StartCoroutine(ShowFloodAlert());
        timerRunning = true;
    }

    private IEnumerator ShowFloodAlert()
    {
        floodAlertText.gameObject.SetActive(true);
        floodAlertText.text = "Flood started. Evacuate!";
        yield return new WaitForSeconds(5);
        floodAlertText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (timerRunning && !gameEnded)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timer <= 0)
            {
                timer = 0;
                EndGame(false);
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        countdownText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other == bridgeTrigger)
        {
            TriggerBridgeCollapse();
        }
    }

    public void TriggerBridgeCollapse()
    {
        EnableBridgePhysics();
        StartCoroutine(ShowBridgeCollapseAlert());
    }

    private IEnumerator ShowBridgeCollapseAlert()
    {
        bridgeCollapseText.gameObject.SetActive(true);
        bridgeCollapseText.text = "Find alternative evacuation route!";
        yield return new WaitForSeconds(5);
        bridgeCollapseText.gameObject.SetActive(false);
    }

    public void OnSafeDestinationReached()
    {
        EndGame(true);
    }

    private void EndGame(bool reachedSafeDestination)
    {
        gameEnded = true;
        timerRunning = false;
        Time.timeScale = 0;

        if (reachedSafeDestination)
        {
            safeDestinationPanel.SetActive(true);
        }
        else
        {
            timeUpPanel.SetActive(true);
        }
    }

    private void DisableBridgePhysics()
    {
        foreach (Rigidbody rb in bridge.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }
    }

    private void EnableBridgePhysics()
    {
        foreach (Rigidbody rb in bridge.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
        }
    }
}
