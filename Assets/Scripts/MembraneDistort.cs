using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MembraneDistort : MonoBehaviour
{
    public Material distortionMaterial;

    [Header("Distortion Parameters")]
    [Range(0.0f, 2.0f)]
    public float magnitude = 1f; // >1 enlarges, <1 shrinks
    [Range(0.0f, 1.0f)]
    public float radius = 0.2f;    // normalized radius
    public Vector2 center = new Vector2(0.5f, 0.5f); // normalized [0..1]

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (distortionMaterial != null)
        {
            distortionMaterial.SetFloat("_Magnitude", magnitude);
            distortionMaterial.SetFloat("_Radius", radius);
            distortionMaterial.SetVector("_Center", center);
            Graphics.Blit(source, destination, distortionMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}