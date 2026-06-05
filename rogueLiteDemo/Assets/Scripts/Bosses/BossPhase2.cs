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

    // FIX: Public field so it can be assigned directly in Inspector.
    // GetComponentInChildren was failing silently → null crash every frame.
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
        // dialogueHandler is now assigned via Inspector — no GetComponentInChildren
    }

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerController = p.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.OnActionPressed += TryHug;
        }
        else
        {
            Debug.LogError("[BossPhase2] Player not found in Start(). Check tag.");
        }

        if (hugPromptUI != null) hugPromptUI.SetActive(false);
    }

    void Update()
    {
        if (hugDone) return;

        // Retry player if missed on Start (handles late spawning edge case)
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

        // FIX: Set dialoguePlayed = true BEFORE starting coroutine.
        // Previously set inside coroutine → multiple coroutines started each frame
        // before the flag was set → duplicate/stuttering dialogue.
        if (!dialoguePlayed && dist <= dialogueTriggerRange)
        {
            dialoguePlayed = true;
            StartCoroutine(TriggerDialogueDelayed());
        }

        // FIX: Null-check dialogueHandler before accessing .IsRunning.
        // If dialogueHandler was null, this line crashed every frame and stopped
        // Update() from running → dialogue never triggered, hug never worked.
        bool dialogueRunning = dialogueHandler != null && dialogueHandler.IsRunning;
        if (hugPromptUI != null && !dialogueRunning)
            hugPromptUI.SetActive(dist <= hugRange);
    }

    private IEnumerator TriggerDialogueDelayed()
    {
        yield return new WaitForSeconds(dialogueTriggerDelay);

        if (dialogueHandler != null)
        {
            playerController.OnActionPressed += dialogueHandler.AdvanceDialogue;

            dialogueHandler.OnDialogueComplete += () =>
            {
                playerController.OnActionPressed -= dialogueHandler.AdvanceDialogue;
            };

            dialogueHandler.StartDialogue(dialogueLines, playerController);
        }
        else
        {
            Debug.LogWarning("[BossPhase2] DialogueHandler not assigned in Inspector.");
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
        else
            Debug.LogWarning("[BossPhase2] potionFragmentPrefab not assigned.");

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