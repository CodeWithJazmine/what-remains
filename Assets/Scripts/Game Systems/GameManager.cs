using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using System.Linq;
using Sirenix.Utilities;

#region Enums
enum GamePhase
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

enum PhaseType
{
    BasePhase,          // Assign Tasks, Manage Survivors
    ScavengingPhase,    // Looting, Missions, Exploring
    PhaseSummary,       // Result screen at the end of the phase (and loading in a saved game)
    PhaseIntro          // Sets the phase to day or night
}

public enum StationType
{
    ScavengingTable,
    RestArea,
    CommunalArea,
    CraftingTable
}
#endregion

[RequireComponent(typeof(PlayerInput))]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] ShelterStationsManager shelterStationsManager;

    [Header("Input References")]
    [SerializeField] private HUDController hud;
    public PlayerInput playerInput;

    [Header("Survivor References")]
    [SerializeField] private List<GameObject> survivors;
    [SerializeField] private Survivor currentSurvivor;
    private int currentIndex = 0;
    private List<Transform> spawnPoints = new List<Transform>();
    private readonly List<GameObject> spawnedSurvivors = new();
    private Survivor scavengingSurvivor;

    [Header("Game Loop")]
    //private GamePhase currentGamePhase;
    private PhaseType currentPhaseType;
    private readonly Dictionary<StationType, Survivor> assignedSurvivors = new();
    private int phaseCount = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(hud);
        playerInput = GetComponent<PlayerInput>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        GameLoop();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.f1Key.wasPressedThisFrame)
            AdvancePhase();
        if (Keyboard.current.f2Key.wasPressedThisFrame)
            EnterShelterFlow();
#endif
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Zone1" && scavengingSurvivor != null)
        {
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

            GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");

            if (spawnPoint != null)
            {
                scavengingSurvivor.transform.position = spawnPoint.transform.position;
                scavengingSurvivor.transform.rotation = spawnPoint.transform.rotation;
            }

            currentSurvivor = scavengingSurvivor;

            if (currentSurvivor.TryGetComponent<PlayerInputManager>(out var inputManager))
            {
                inputManager.enabled = true;
            }

            hud.UpdateSurvivorPanel(currentSurvivor);
            cinemachineCamera.Follow = currentSurvivor.transform;
        }
    }

    void InitializeGame()
    {
        if (survivors == null || survivors.Count == 0)
        {
            return;
        }

        // If survivors have not been spawned yet, wait until spawn points exist and spawn them
        if (spawnedSurvivors.Count == 0)
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("GameManager: No spawn points registered yet.");
                return;
            }

            SpawnSurvivors();
        }

        // Disable the input managers for all the spawned survivors
        foreach (var s in spawnedSurvivors)
        {
            if (s != null && s.TryGetComponent<PlayerInputManager>(out var pim))
            {
                pim.enabled = false;
                pim.isMovementEnabled = true;
            }
        }

        // Set the starting survivor and enable its input manager
        if (spawnedSurvivors[currentIndex].TryGetComponent<Survivor>(out var survivor))
            currentSurvivor = survivor;

        if (currentSurvivor != null && currentSurvivor.TryGetComponent<PlayerInputManager>(out var playerInputManager))
        {
            playerInputManager.enabled = true;
            // Update HUD
            hud.UpdateSurvivorPanel(currentSurvivor);
            cinemachineCamera.Follow = currentSurvivor.transform;
        }

    }

    void GameLoop()
    {
        // Beginning of game
        //currentGamePhase = GamePhase.Playing;

        EnterShelterFlow();
    }

    #region Phase Lifecycle

    void BeginPhase(PhaseType phaseType)
    {
        switch (phaseType)
        {
            case PhaseType.BasePhase:
                {
                    // TODO: Implement BasePhase
                    break;
                }
            case PhaseType.ScavengingPhase:
                {
                    // TODO: Implement ScavengingPhase
                    break;
                }
            case PhaseType.PhaseSummary:
                {
                    // TODO: Implement PhaseSummary
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    // Decides what the next phase will be based on assignments
    [Button("Advance Phase")]
    void AdvancePhase()
    {
        // Capture any needed info BEFORE clearing assignments
        assignedSurvivors.TryGetValue(StationType.ScavengingTable, out var scavenger);

        // End/cleanup the current phase (unassign + re-enable movement)
        EndPhase();

        // Next phase tick (used for day/night)
        phaseCount++;

        if (scavenger != null)
        {
            scavengingSurvivor = scavenger;
            currentSurvivor = scavengingSurvivor;
            DontDestroyOnLoad(scavengingSurvivor.gameObject);
            // TODO: Load scavenging scene and set the active survivor to `scavenger`
            var scavengerGO = scavenger.gameObject;
            foreach (var s in spawnedSurvivors)
            {
                if (s != null)
                {
                    s.SetActive(s == scavengerGO);
                }
            }
            currentPhaseType = PhaseType.ScavengingPhase;
            SceneManager.LoadScene("Zone1");
        }
        else
        {
            EnterShelterFlow();
        }
    }

    void EndPhase()
    {
        // Remove all survivor assignments from the previous phase
        assignedSurvivors.Clear();

        // Ensure everyone can move again when transitioning to shelter or another scene
        if (spawnedSurvivors == null) return;

        foreach (var go in spawnedSurvivors)
        {
            if (go == null) continue;
            if (go.TryGetComponent<PlayerInputManager>(out var pim))
            {
                pim.isMovementEnabled = true;
            }
        }

        // Refresh Shelter Stations
        shelterStationsManager.RefreshStations();
    }

    void PhaseSummary()
    {
        currentPhaseType = PhaseType.PhaseSummary;

        // TODO: Implement PhaseSummary
        // What happened to each survivor?
        // What happened to the base?
        // What resources were made?

        // TODO: Wait for input...
    }

    void PhaseIntro()
    {
        // TODO: Lock controls. Setting time to 0 doesn't work because coroutines use real time.
        StartCoroutine(hud.ShowPhaseIntro(phaseCount));
    }

    #endregion

    [Button("Enter Shelter Flow")]
    void EnterShelterFlow()
    {
        if (currentPhaseType == PhaseType.ScavengingPhase)
        {
            if (scavengingSurvivor != null)
            {
                Destroy(scavengingSurvivor.gameObject);
                scavengingSurvivor = null;
            }
            SceneManager.LoadScene("Shelter");

            foreach (var s in spawnedSurvivors)
            {
                if (s != null)
                {
                    s.SetActive(true);
                }
            }
        }

        InitializeGame();

        PhaseIntro();
        PhaseSummary();

        currentPhaseType = PhaseType.BasePhase;
        BeginPhase(currentPhaseType);
    }

    public void NextSurvivor()
    {
        if (currentPhaseType != PhaseType.BasePhase || spawnedSurvivors == null || spawnedSurvivors.Count == 0) return;

        // Disable the previous survivor's input manager
        if (currentSurvivor != null && currentSurvivor.TryGetComponent<PlayerInputManager>(out var oldInput))
            oldInput.enabled = false;

        // Switch to the next survivor
        currentIndex = (currentIndex + 1) % spawnedSurvivors.Count;

        if (spawnedSurvivors[currentIndex].TryGetComponent<Survivor>(out var survivor))
        {
            currentSurvivor = survivor;
        }

        // Enable the current survivor's input manager and update the HUD
        if (currentSurvivor != null && currentSurvivor.TryGetComponent<PlayerInputManager>(out var newInput))
        {
            newInput.enabled = true;
            // Update HUD
            hud.UpdateSurvivorPanel(currentSurvivor);
            // Update camera view
            cinemachineCamera.Follow = currentSurvivor.transform;
        }

    }

    public bool AssignSurvivorToStation(StationType stationType)
    {
        // Get the Player Input Manager
        currentSurvivor.TryGetComponent<PlayerInputManager>(out var playerInputManager);

        // If the current survivor is already assigned to this station, unassign
        if (assignedSurvivors.ContainsKey(stationType) &&
            assignedSurvivors[stationType] == currentSurvivor)
        {
            assignedSurvivors.Remove(stationType);
            playerInputManager.isMovementEnabled = true;

            return false;
        }
        // else if no one is assigned to this station, assign this survivor
        else if (!assignedSurvivors.ContainsKey(stationType))
        {
            assignedSurvivors.Add(stationType, currentSurvivor);
            playerInputManager.isMovementEnabled = false;

            return true;
        }

        return true;

    }

    public void RegisterSpawnPoints(List<Transform> sps)
    {
        foreach (Transform p in sps)
        {
            spawnPoints.Add(p);
            Debug.Log($"Added point: {p}");
        }

    }

    void SpawnSurvivors()
    {
        if (survivors == null)
        {
            Debug.Log("GameManager: No survivor prefabs assigned");
            return;
        }

        if (spawnPoints.Count < survivors.Count)
        {
            Debug.Log("GameManager: Not enough spawn points");
            return;
        }

        spawnedSurvivors.Clear();

        for (int i = 0; i < survivors.Count; i++)
        {
            GameObject spawnedSurvivor = Instantiate(
                survivors[i],
                spawnPoints[i].position,
                spawnPoints[i].rotation
            );

            spawnedSurvivors.Add(spawnedSurvivor);
        }

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
