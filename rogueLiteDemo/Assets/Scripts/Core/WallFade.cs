using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class WallFade : MonoBehaviour
{
    [Header("Materiales")]
    [Tooltip("Material normal opaco (fondoParedBlanca). Si se deja vacío, usa el que ya tenga el muro.")]
    [SerializeField] private Material opaqueMaterial;

    [Tooltip("Copia transparente del mismo material (Surface Type = Transparent).")]
    [SerializeField] private Material fadeMaterial;

    [Header("Transparencia")]
    [Range(0f, 1f)]
    [SerializeField] private float fadedAlpha = 0.25f;
    [SerializeField] private float fadeSpeed = 8f;

    private Renderer rend;
    private Collider col;
    private Camera cam;
    private Material fadeInstance;
    private float currentAlpha = 1f;
    private bool fading = false;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();

        if (opaqueMaterial == null) opaqueMaterial = rend.sharedMaterial;

        if (fadeMaterial != null) fadeInstance = new Material(fadeMaterial);
    }

    private void LateUpdate()
    {
        if (fadeMaterial == null) return;
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        bool occluding = IsOccludingAnyCharacter();

        if (occluding && !fading) StartFade();

        if (fading)
        {
            float target = occluding ? fadedAlpha : 1f;
            currentAlpha = Mathf.MoveTowards(currentAlpha, target, fadeSpeed * Time.deltaTime);
            SetAlpha(currentAlpha);

            if (!occluding && currentAlpha >= 0.999f) EndFade();
        }
    }

    private void StartFade()
    {
        fading = true;
        rend.material = fadeInstance;
        currentAlpha = 1f;
        SetAlpha(1f);
    }

    private void EndFade()
    {
        fading = false;
        rend.material = opaqueMaterial; 
    }

    private bool IsOccludingAnyCharacter()
    {
        if (Occludes(GameObject.FindGameObjectWithTag("Player"))) return true;

        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
            if (Occludes(e)) return true;

        return false;
    }

    private bool Occludes(GameObject character)
    {
        if (character == null) return false;
        Ray ray = new Ray(character.transform.position, -cam.transform.forward);
        return col.Raycast(ray, out _, 50f);
    }

    private void SetAlpha(float a)
    {
        if (fadeInstance.HasProperty("_BaseColor"))
        {
            Color c = fadeInstance.GetColor("_BaseColor");
            c.a = a;
            fadeInstance.SetColor("_BaseColor", c);
        }
        else
        {
            Color c = fadeInstance.color;
            c.a = a;
            fadeInstance.color = c;
        }
    }
}