using System.Collections;
using UnityEngine;

/// <summary>
/// Phase 2 narrative boss. Cannot take damage or deal damage.
/// Floats in place. Triggers dialogue when player moves nearby.
/// Player can Hug the boss when close — boss disappears and spawns PotionFragment.
///
/// SETUP:
///   BossPhase2Root
///     ??? Collider (Is Trigger = true, large — detection zone)
///     ??? BossPhase2 (this script)
///     ??? DialogueHandler (on same or child GameObject)
///     ??? SpriteChild
///           ??? SpriteRenderer (assign ghost sprite)
///           ??? Billboard
/// </summary>
public class BossPhase2 : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] dialogueLines = {
        "Aurora... I've been waiting for you.",
        "You don't have to carry all of this alone.",
        "Come back to me. Please."
    };
    public float dialogueTriggerDelay = 2.5f;   // Delay after player moves
    public float dialogueTriggerRange = 6f;

    [Header("Hug Interaction")]
    public float hugRange = 2f;
    public GameObject hugPromptUI;              // "Press [E] to Hug" panel

    [Header("Spawn on Hug")]
    public GameObject potionFragmentPrefab;
    public Vector3 spawnOffset = new Vector3(0f, 0.5f, 1f);

    [Header("Float Animation")]
    public float floatAmplitude = 0.3f;
    public float floatSpeed = 1.2f;

    // ?? Internal ?????????????????????????????????????????????????????

    private PlayerController playerController;
    private DialogueHandler dialogueHandler;

    private Vector3 startPos;
    private bool dialoguePlayed = false;
    private bool hugDone = false;
    private bool playerInRange = false;

    void Awake()
    {
        dialogueHandler = GetComponentInChildren<DialogueHandler>();
        startPos = transform.position;
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
            Debug.LogError("[BossPhase2] Player not found.");
        }

        if (hugPromptUI != null)
            hugPromptUI.SetActive(false);
    }

    void Update()
    {
        if (hugDone) return;

        HandleFloat();
        HandleProximity();
    }

    // ?? Float ?????????????????????????????????????????????????????????

    private void HandleFloat()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    // ?? Proximity + Dialogue + Hug Prompt ????????????????????????????

    private void HandleProximity()
    {
        if (playerController == null) return;

        float dist = Vector3.Distance(transform.position, playerController.transform.position);

        // Dialogue trigger zone
        if (!dialoguePlayed && dist <= dialogueTriggerRange)
            StartCoroutine(TriggerDialogueDelayed());

        // Hug prompt
        bool inHugRange = dist <= hugRange;
        if (hugPromptUI != null && !dialogueHandler.IsRunning)
            hugPromptUI.SetActive(inHugRange);
    }

    private IEnumerator TriggerDialogueDelayed()
    {
        if (dialoguePlayed) yield break;
        dialoguePlayed = true;

        yield return new WaitForSeconds(dialogueTriggerDelay);

        if (dialogueHandler != null)
        {
            // Hook Action button to advance dialogue
            playerController.OnActionPressed += dialogueHandler.AdvanceDialogue;

            dialogueHandler.OnDialogueComplete += () =>
            {
                playerController.OnActionPressed -= dialogueHandler.AdvanceDialogue;
            };

            dialogueHandler.StartDialogue(dialogueLines, playerController);
        }
        else
        {
            Debug.LogWarning("[BossPhase2] DialogueHandler not found.");
        }
    }

    // ?? Hug ??????????????????????????????????????????????????????????

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

        if (hugPromptUI != null)
            hugPromptUI.SetActive(false);

        // Graceful disappear: fade out over 1 second
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

        // Spawn PotionFragment
        if (potionFragmentPrefab != null)
            Instantiate(potionFragmentPrefab, transform.position + spawnOffset, Quaternion.identity);
        else
            Debug.LogWarning("[BossPhase2] potionFragmentPrefab not assigned.");

        Destroy(gameObject);
    }

    // ?? Invulnerability (no damage, no health component needed) ??????

    // BossPhase2 intentionally has NO Health component.
    // If one is added by mistake, this prevents interaction:
    void OnTriggerEnter(Collider other) { /* intentionally empty */ }

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