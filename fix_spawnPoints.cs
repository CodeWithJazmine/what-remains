// ─────────────────────────────────────────────────────────────────────────────
// BUG FIX: spawnPoints NullReferenceException
// File: GameManager.cs
// ─────────────────────────────────────────────────────────────────────────────
//
// PROBLEM
// -------
// The spawnPoints field is declared but never assigned a new List<Transform>().
// Any call to spawnPoints.Add(), spawnPoints.Count, or iterating over it
// throws a NullReferenceException at runtime because the list object
// doesn't exist yet — the field is null.
//
// FIX — Option A (inline initializer, preferred)
// -----------------------------------------------
// Initialize the list directly at the declaration site.
// The object is created as soon as GameManager is instantiated.

// BEFORE:
private List<Transform> spawnPoints;

// AFTER:
private List<Transform> spawnPoints = new List<Transform>();


// ─────────────────────────────────────────────────────────────────────────────
// FIX — Option B (initialize in Awake)
// ─────────────────────────────────────────────────────────────────────────────
// Use this if you populate spawnPoints from the Inspector or via
// GetComponentsInChildren, so you want to assign it fresh each time
// the scene loads rather than carrying stale data between runs.

private void Awake()
{
    // If spawnPoints is populated manually (e.g. dragged in Inspector),
    // this guard prevents overwriting a [SerializeField] assignment.
    if (spawnPoints == null)
        spawnPoints = new List<Transform>();

    // -- or, if you're collecting them from child objects at startup: --
    // spawnPoints = new List<Transform>(GetComponentsInChildren<Transform>());
}
