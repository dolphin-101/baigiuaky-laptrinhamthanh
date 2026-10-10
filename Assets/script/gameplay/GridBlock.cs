using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridBlock : MonoBehaviour, IPointerClickHandler
{
    [Header("Tọa độ trên bàn cờ")]
    public int gridX;
    public int gridY;

    [Header("Cài đặt viên màu")]
    public ColorType blockColor;
    public float moveSpeed = 2500f;

    [Header("Khối Tím (Vật cản cố định)")]
    public bool isStaticObstacle = false;

    [Header("Cơ chế Ẩn (Dấu ?)")]
    public bool isHidden = false;
    public Sprite normalSprite;
    public Sprite hiddenSprite;

    private bool isMoving = false;
    private QueueManager queueManager;
    private Image blockImage;

    void Start()
    {
        queueManager = FindObjectOfType<QueueManager>();
        blockImage = GetComponent<Image>();
        UpdateVisual();

        GridManager.Instance.RegisterBlock(this, gridX, gridY);
    }

    public void UpdateVisual()
    {
        if (blockImage == null) return;
        blockImage.sprite = isHidden ? hiddenSprite : normalSprite;
    }

    public void RevealColor()
    {
        if (!isHidden) return;
        isHidden = false;
        UpdateVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isStaticObstacle || isHidden || isMoving || queueManager == null) return;

        List<Vector2Int> path = GridManager.Instance.GetPathToExit(gridX, gridY);

        if (path == null)
        {
            GridManager.Instance.RegisterWrongClick();
            return;
        }

        Transform targetPos;
        if (queueManager.TryAddBlock(blockColor, out targetPos))
        {
            AudioManager.Instance.PlayObjectClickSound();

            GridManager.Instance.RemoveBlock(gridX, gridY);

            isMoving = true;
            transform.SetAsLastSibling();
            StartCoroutine(MoveAlongPath(path, targetPos));
        }
    }


    private IEnumerator MoveAlongPath(List<Vector2Int> path, Transform finalTarget)
    {
        foreach (Vector2Int node in path)
        {
            Vector3 targetCellPos = GridManager.Instance.cellPositions[node.x, node.y];

            while (Vector2.Distance(transform.position, targetCellPos) > 5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetCellPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        while (finalTarget != null && Vector2.Distance(transform.position, finalTarget.position) > 5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalTarget.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}