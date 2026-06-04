using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Full-screen memory painting viewer.
/// One shared instance in the lobby scene on a Canvas.
/// Called by MemoryPickup.Show() — player is paralyzed while viewing.
/// Close with Action button OR the CloseButton in the UI.
///
/// CANVAS STRUCTURE:
///   MemoryCanvas  (Canvas - Screen Space Overlay, sort order high)
///   └── MemoryPanel  (dark background Image, full screen)
///       ├── PaintingImage   (Image — your pixel art sprite goes here at runtime)
///       ├── TitleText       (TMP_Text)
///       └── CloseButton     (Button → calls Close())
///             └── Label: "Cerrar  [E]"
/// </summary>
public class MemoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public Image paintingImage;
    public TMP_Text titleText;

    private PlayerController playerController;
    public bool IsOpen { get; private set; }

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Show(Sprite painting, string title)
    {
        // Find and paralyze player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerController = p.GetComponent<PlayerController>();
            playerController?.SetParalyzed(true);
            if (playerController != null)
                playerController.OnActionPressed += Close;
        }

        if (paintingImage != null && painting != null)
            paintingImage.sprite = painting;

        if (titleText != null)
            titleText.text = title;

        if (panel != null) panel.SetActive(true);
        IsOpen = true;
    }

    // Called by CloseButton.onClick OR Action button
    public void Close()
    {
        if (!IsOpen) return;

        playerController?.SetParalyzed(false);
        if (playerController != null)
            playerController.OnActionPressed -= Close;

        if (panel != null) panel.SetActive(false);
        IsOpen = false;
        playerController = null;
    }
}