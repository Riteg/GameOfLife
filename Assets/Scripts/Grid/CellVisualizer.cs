using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CellVisualizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        SetDefaultColor();
    }
    public void SetCellSprite(Sprite sprite)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = sprite;

    }
    public void SetDefaultColor()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = null;
    }
}