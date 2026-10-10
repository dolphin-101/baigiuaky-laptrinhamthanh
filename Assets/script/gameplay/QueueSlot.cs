using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QueueSlot : MonoBehaviour
{
    [Header("Cài đặt ô chứa")]
    public ColorType slotColor;
    public int maxCapacity = 3;
    public int currentCount = 0;
    public bool isCapacityKnown = true;

    [Header("Giao diện UI")]
    public Image backgroundFrame;     
    public GameObject slotsContainer;  
    public List<Image> childSlotImages; 

    private Vector3 targetPosition;
    private float moveSpeed = 1500f;

    void Update()
    {
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void InitializeSlot(ColorType color, int capacity, Color actualColor, Vector3 targetPos, bool isKnown)
    {
        slotColor = color;
        maxCapacity = capacity;
        currentCount = 0;
        targetPosition = targetPos;
        isCapacityKnown = isKnown;

        foreach (Image img in childSlotImages)
        {
            img.color = actualColor;
        }

        UpdateVisual();
    }

    public void MoveToNewPosition(Vector3 newTargetPos)
    {
        targetPosition = newTargetPos;
    }

    public void RevealCapacity()
    {
        isCapacityKnown = true;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (isCapacityKnown)
        {

            slotsContainer.SetActive(true);

   
            for (int i = 0; i < childSlotImages.Count; i++)
            {
                childSlotImages[i].gameObject.SetActive(i < maxCapacity);
            }

            backgroundFrame.color = new Color(1, 1, 1, 0);
        }
        else
        {
            slotsContainer.SetActive(false);
            backgroundFrame.color = new Color(1, 1, 1, 1);
        }
    }
}