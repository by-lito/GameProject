using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueHandler : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Display")]
    public float delayBetweenChars = 0.03f;

    private PlayerController playerController;
    private bool waitingForInput = false;
    private bool isRunning = false;

    public bool IsRunning => isRunning;
    public System.Action OnDialogueComplete;

    // Removed Awake() that was causing the panel to deactivate on Play.

    public void StartDialogue(string[] lines, PlayerController player)
    {
        if (isRunning) return;
        playerController = player;

        // Wire up the event
        playerController.OnActionPressed += AdvanceDialogue;

        StartCoroutine(RunDialogue(lines));
    }

    private IEnumerator RunDialogue(string[] lines)
    {
        isRunning = true;
        playerController?.SetParalyzed(true);
        dialoguePanel?.SetActive(true);

        foreach (string line in lines)
        {
            // Type the line
            yield return StartCoroutine(TypeLine(line));

            // Wait for player to press E
            waitingForInput = true;
            yield return new WaitUntil(() => !waitingForInput);
        }

        // Cleanup
        playerController.OnActionPressed -= AdvanceDialogue;
        dialoguePanel?.SetActive(false);
        playerController?.SetParalyzed(false);
        isRunning = false;
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void AdvanceDialogue()
    {
        if (waitingForInput)
        {
            waitingForInput = false;
        }
    }
}