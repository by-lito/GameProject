using System.Collections;
using UnityEngine;

public class BossPhase2 : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] dialogueLines = {
        "Aurora... I've been waiting for you.",
        "You don't have to carry all of this alone.",
        "Come back to me. Please."
    };
    public float dialogueTriggerDelay = 2.5f;
    public float dialogueTriggerRange = 6f;

    [Header("Dialogue Handler")]
    public DialogueHandler dialogueHandler;

    [Header("Hug Interaction")]
    public float hugRange = 2f;
    public GameObject hugPromptUI;

    [Header("Spawn on Hug")]
    public GameObject potionFragmentPrefab;
    public Vector3 spawnOffset = new Vector3(0f, 0.5f, 1f);

    [Header("Float Animation")]
    public float floatAmplitude = 0.3f;
    public float floatSpeed = 1.2f;

    private PlayerController playerController;
    private Vector3 startPos;
    private bool dialoguePlayed = false;
    private bool hugDone = false;

    void Awake()
    {
        startPos = transform.position;
    }

    void Start()
    {
        // Connect to the player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerController = p.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.OnActionPressed += TryHug;
        }

        // ─── THE SAFEST AUTO-CONNECT ───
        // This modern Unity method guarantees it only searches the active scene, never project folders!
        if (dialogueHandler == null)
        {
            dialogueHandler = Object.FindFirstObjectByType<DialogueHandler>(FindObjectsInactive.Include);
        }

        if (dialogueHandler == null)
        {
            Debug.LogError("[BossPhase2] CRITICAL: Could not find a DialogueHandler in the active scene!");
        }
        else
        {
            Debug.Log($"[BossPhase2] FOUND REAL SCENE UI: {dialogueHandler.gameObject.name}");
        }
        // ───────────────────────────────

        if (hugPromptUI != null) hugPromptUI.SetActive(false);
    }

    void Update()
    {
        if (hugDone) return;

        // Retry player if missed on Start
        if (playerController == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                playerController = p.GetComponent<PlayerController>();
                if (playerController != null)
                    playerController.OnActionPressed += TryHug;
            }
            return;
        }

        HandleFloat();
        HandleProximity();
    }

    private void HandleFloat()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void HandleProximity()
    {
        float dist = Vector3.Distance(transform.position, playerController.transform.position);

        if (!dialoguePlayed && dist <= dialogueTriggerRange)
        {
            dialoguePlayed = true;
            StartCoroutine(TriggerDialogueDelayed());
        }

        bool dialogueRunning = dialogueHandler != null && dialogueHandler.IsRunning;

        // Safety check: Only show the "E" prompt if dialogue has finished playing entirely
        if (hugPromptUI != null && dialoguePlayed && !dialogueRunning)
        {
            hugPromptUI.SetActive(dist <= hugRange);
        }
    }

    IEnumerator TriggerDialogueDelayed()
    {
        yield return new WaitForSeconds(dialogueTriggerDelay);

        if (dialogueHandler != null)
        {
            // Instead of SetActive, we manipulate the visual state
            CanvasGroup cg = dialogueHandler.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1;            // Make it visible
                cg.interactable = true;  // Make it clickable/active
                cg.blocksRaycasts = true;
            }

            // Now the object is ALWAYS active, so this CANNOT crash
            dialogueHandler.StartDialogue(dialogueLines, playerController);
        }
    }

    private void TryHug()
    {
        if (hugDone || playerController == null) return;
        if (dialogueHandler != null && dialogueHandler.IsRunning) return;

        float dist = Vector3.Distance(transform.position, playerController.transform.position);
        if (dist > hugRange) return;

        StartCoroutine(HugSequence());
    }

    private IEnumerator HugSequence()
    {
        hugDone = true;
        if (hugPromptUI != null) hugPromptUI.SetActive(false);

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            float elapsed = 0f;
            Color startColor = sr.color;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                sr.color = new Color(startColor.r, startColor.g, startColor.b, 1f - elapsed);
                yield return null;
            }
        }

        if (potionFragmentPrefab != null)
            Instantiate(potionFragmentPrefab, transform.position + spawnOffset, Quaternion.identity);

        // --- FINAL STEP: LOAD YOUR BOOT SCENE ---
        // Ensure your "Boot" scene is added in Build Settings
        StatsTracker.Instance?.AddRunCompleted();
        StatsTracker.Instance?.Flush();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Boot");

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (playerController != null)
            playerController.OnActionPressed -= TryHug;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dialogueTriggerRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, hugRange);
    }
}