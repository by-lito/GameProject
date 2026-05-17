using UnityEngine;

/// <summary>
/// Attach to any GameObject with a SpriteRenderer.
/// Forces the sprite to always face the main camera (billboard effect),
/// maintaining correct 2D proportions without affecting physics or colliders.
/// Works for Player, Enemies, Bosses — any sprite in a 3D scene.
/// </summary>
public class Billboard : MonoBehaviour
{
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    /// <summary>
    /// LateUpdate ensures rotation happens after all movement/physics this frame.
    /// </summary>
    void LateUpdate()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
            return;
        }

        // Match camera rotation — sprite always faces camera plane
        transform.rotation = mainCam.transform.rotation;
    }
}