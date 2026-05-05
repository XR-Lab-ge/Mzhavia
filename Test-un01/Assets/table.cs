using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class table : UdonSharpBehaviour
{
    public Renderer tableRenderer;

    public override void Interact()
    {
        Color newColor = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );

        tableRenderer.material.color = newColor;
    }
}