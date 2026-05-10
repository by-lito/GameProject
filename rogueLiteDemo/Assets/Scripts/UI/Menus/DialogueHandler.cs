using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Minimal dialogue system. Displays lines one by one.
/// Paralyzes the player while text is shown.
/// Player advances dialogue with the Action button.
/// Assign a UI Text (or TMP_Text) in the Inspector.
/// </summary>
public class DialogueHandler : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public Text dialogueText;              // Swap for TMP_Text if using TextMeshPro

    [Header("Display")]
    public float delayBetweenChars = 0.03f;

    private PlayerController playerController;
    private bool waitingForInput = false;
    private bool isRunning = false;

    public bool IsRunning => isRunning;

    // Event fired when all lines are done
    public System.Action OnDialogueComplete;

    void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] lines, PlayerController player)
    {
        if (isRunning) return;
        playerController = player;
        StartCoroutine(RunDialogue(lines));
    }

    private IEnumerator RunDialogue(string[] lines)
    {
        isRunning = true;
        playerController?.SetParalyzed(true);
        dialoguePanel?.SetActive(true);

        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeLine(line));

            // Wait until player presses Action
            waitingForInput = true;
            yield return new WaitUntil(() => !waitingForInput);
        }

        dialoguePanel?.SetActive(false);
        playerController?.SetParalyzed(false);
        isRunning = false;

        OnDialogueComplete?.Invoke();
    }

    private IEnumerator TypeLine(string line)
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
            foreach (char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(delayBetweenChars);
            }
        }
        else
        {
            Debug.LogWarning("DialogueHandler: dialogueText not assigned.");
            yield return null;
        }
    }

    /// <summary>
    /// Call this when the player presses Action during dialogue.
    /// Hooked up by BossPhase2 via PlayerController.OnActionPressed.
    /// </summary>
    public void AdvanceDialogue()
    {
        if (waitingForInput)
            waitingForInput = false;
    }
}