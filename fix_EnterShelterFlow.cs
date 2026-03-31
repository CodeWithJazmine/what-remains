// ─────────────────────────────────────────────────────────────────────────────
// BUG FIX: EnterShelterFlow() — synchronous scene load runs too early
// File: GameManager.cs
// ─────────────────────────────────────────────────────────────────────────────
//
// PROBLEM
// -------
// The current implementation calls SceneManager.LoadScene() synchronously
// inside EnterShelterFlow(). LoadScene() is immediate — it destroys the
// active scene on the same frame it's called. This means:
//
//   1. Any PhaseIntro or PhaseSummary UI that should play BEFORE the
//      transition is torn down immediately — the player never sees it.
//   2. Any cleanup logic after the LoadScene call is unreachable or
//      runs against already-destroyed objects.
//
// FIX
// ---
// Replace the synchronous call with a coroutine that:
//   1. Shows PhaseIntro / PhaseSummary UI and waits for it to finish.
//   2. Starts loading the shelter scene in the background (allowSceneActivation = false).
//   3. Activates the new scene only after the UI sequence is complete.
//
// This keeps the game responsive during the load and gives you a natural
// hook for a fade or transition effect.
//
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string shelterSceneName = "ShelterScene";

    // ── Public entry point ────────────────────────────────────────────────────
    // Call this wherever you previously called EnterShelterFlow() directly.
    public void EnterShelterFlow()
    {
        StartCoroutine(EnterShelterCoroutine());
    }

    // ── Coroutine ─────────────────────────────────────────────────────────────
    private IEnumerator EnterShelterCoroutine()
    {
        // 1. Play phase summary / intro UI.
        //    Replace this with your actual UI call and wait for it to finish.
        //    Example assumes a UIManager singleton with an awaitable coroutine.
        yield return StartCoroutine(ShowPhaseSummaryUI());

        // 2. Begin loading the shelter scene in the background.
        //    allowSceneActivation = false keeps it paused at 90% until we're ready.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(shelterSceneName);
        asyncLoad.allowSceneActivation = false;

        // 3. Wait until Unity has finished background loading (progress reaches 0.9).
        while (asyncLoad.progress < 0.9f)
        {
            // Optional: drive a loading bar here.
            // float loadPercent = asyncLoad.progress / 0.9f;
            yield return null;
        }

        // 4. Optional: play a screen fade or transition before switching.
        // yield return StartCoroutine(FadeOut());

        // 5. Activate the scene — this is when the old scene is destroyed.
        asyncLoad.allowSceneActivation = true;

        // 6. Wait one frame for the new scene to finish activating.
        yield return null;

        // 7. Any post-load initialisation (e.g. resetting phase state) goes here.
        OnShelterSceneReady();
    }

    // ── Helpers — replace these stubs with your real implementations ──────────

    private IEnumerator ShowPhaseSummaryUI()
    {
        // TODO: trigger your PhaseSummary / PhaseIntro UI panel here and
        //       yield until it signals completion. For example:
        //
        //   UIManager.Instance.ShowPhaseSummary(currentPhaseData);
        //   yield return new WaitUntil(() => UIManager.Instance.IsSummaryDone);
        //
        // Temporary placeholder — waits 2 seconds so you can see it works:
        yield return new WaitForSeconds(2f);
    }

    private void OnShelterSceneReady()
    {
        // Called once the shelter scene is fully active.
        // Reset or initialise anything that needs a fresh state here.
        Debug.Log("[GameManager] Shelter scene loaded and ready.");
    }
}
