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
    [SerializeField] private List<GameObject> survivorPrefabs;   // prefabs only — never scene instances
    private List<GameObject> spawnedSurvivors = new List<GameObject>(); // live instances
    [SerializeField] private Survivor currentSurvivor;
    private int currentIndex = 0;
    private List<Transform> spawnPoints = new List<Transform>();

    [Header("Game Loop")]
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
        playerInput = GetComponent<PlayerInput>();
        // Survivors are no longer DontDestroyOnLoad — they are spawned fresh each scene load
    }

    void Start()
    {
        if (currentPhaseType != PhaseType.ScavengingPhase)
        {
            InitializeGame();
        }

        GameLoop();
    }

    void InitializeGame()
    {
        if (spawnedSurvivors == null || spawnedSurvivors.Count == 0)
        {
            return;
        }

        // Disable the input managers for all the survivors
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
            hud.UpdateSurvivorPanel(currentSurvivor);
        }
    }

    void GameLoop()
    {
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

    [Button("Advance Phase")]
    void AdvancePhase()
    {
        assignedSurvivors.TryGetValue(StationType.ScavengingTable, out var scavenger);

        EndPhase();

        phaseCount++;

        if (scavenger != null)
        {
            var scavengerGO = scavenger.gameObject;
            foreach (var s in spawnedSurvivors)
            {
                s.SetActive(s == scavengerGO);
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
        assignedSurvivors.Clear();

        if (spawnedSurvivors == null) return;

        foreach (var go in spawnedSurvivors)
        {
            if (go == null) continue;
            if (go.TryGetComponent<PlayerInputManager>(out var pim))
            {
                pim.isMovementEnabled = true;
            }
        }

        shelterStationsManager.RefreshStations();
    }

    void PhaseSummary()
    {
        currentPhaseType = PhaseType.PhaseSummary;

        // TODO: Implement PhaseSummary
    }

    void PhaseIntro()
    {
        // TODO: Lock controls.
        StartCoroutine(hud.ShowPhaseIntro(phaseCount));
    }

    #endregion

    [Button("Enter Shelter Flow")]
    void EnterShelterFlow()
    {
        if (currentPhaseType == PhaseType.ScavengingPhase)
        {
            SceneManager.LoadScene("Shelter");
            // Survivors will be re-spawned by RegisterSpawnPoints once the scene loads
            return;
        }

        PhaseIntro();
        PhaseSummary();

        currentPhaseType = PhaseType.BasePhase;
        BeginPhase(currentPhaseType);
    }

    #region Spawning

    public void RegisterSpawnPoints(List<Transform> sps)
    {
        spawnPoints.Clear(); // prevent duplicates on scene reload
        foreach (Transform p in sps)
        {
            spawnPoints.Add(p);
            Debug.Log($"Added point: {p}");
        }

        SpawnSurvivors();
    }

    void SpawnSurvivors()
    {
        if (survivorPrefabs == null || survivorPrefabs.Count == 0)
        {
            Debug.LogWarning("[GameManager] No survivor prefabs assigned.");
            return;
        }
        if (spawnPoints.Count < survivorPrefabs.Count)
        {
            Debug.LogWarning("[GameManager] Not enough spawn points for all survivors.");
            return;
        }

        // Destroy any survivors from a previous scene load
        foreach (var s in spawnedSurvivors)
            if (s != null) Destroy(s);
        spawnedSurvivors.Clear();
        currentIndex = 0;

        // Instantiate fresh instances at their spawn points
        for (int i = 0; i < survivorPrefabs.Count; i++)
        {
            var instance = Instantiate(survivorPrefabs[i], spawnPoints[i].position, spawnPoints[i].rotation);
            instance.name = $"Survivor{i + 1}";
            spawnedSurvivors.Add(instance);
        }

        InitializeGame();
    }

    #endregion

    public void NextSurvivor()
    {
        if (currentPhaseType != PhaseType.BasePhase || spawnedSurvivors == null || spawnedSurvivors.Count == 0) return;

        if (currentSurvivor != null && currentSurvivor.TryGetComponent<PlayerInputManager>(out var oldInput))
            oldInput.enabled = false;

        currentIndex = (currentIndex + 1) % spawnedSurvivors.Count;

        if (spawnedSurvivors[currentIndex].TryGetComponent<Survivor>(out var survivor))
        {
            currentSurvivor = survivor;
        }

        if (currentSurvivor != null && currentSurvivor.TryGetComponent<PlayerInputManager>(out var newInput))
        {
            newInput.enabled = true;
            hud.UpdateSurvivorPanel(currentSurvivor);
            cinemachineCamera.Follow = currentSurvivor.transform;
        }
    }

    public bool AssignSurvivorToStation(StationType stationType)
    {
        currentSurvivor.TryGetComponent<PlayerInputManager>(out var playerInputManager);

        if (assignedSurvivors.ContainsKey(stationType) &&
            assignedSurvivors[stationType] == currentSurvivor)
        {
            assignedSurvivors.Remove(stationType);
            playerInputManager.isMovementEnabled = true;
            return false;
        }
        else if (!assignedSurvivors.ContainsKey(stationType))
        {
            assignedSurvivors.Add(stationType, currentSurvivor);
            playerInputManager.isMovementEnabled = false;
            return true;
        }

        return true;
    }
}
