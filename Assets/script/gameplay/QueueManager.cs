using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[System.Serializable]
public class SlotData
{
    public ColorType colorType;
    public int capacity;
    public Color displayColor;
}

public class QueueManager : MonoBehaviour
{

    public static QueueManager Instance;

    private int currentCombo = 1;

    [Header("Cài đặt Prefab & Vị trí")]
    public QueueSlot queueSlotPrefab;
    public Transform spawnPoint;
    public Transform[] slotPositions;
    public Transform uiContainer;

    [Header("Kịch bản Level")]
    public List<SlotData> levelQueueData;

    private List<SlotData> currentLevelQueue;
    public List<QueueSlot> activeSlots = new List<QueueSlot>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        currentLevelQueue = new List<SlotData>(levelQueueData);
        for (int i = 0; i < 3; i++)
        {
            SpawnNextSlot(i);
        }
    }

    public void SpawnNextSlot(int targetIndex)
    {
        if (currentLevelQueue.Count == 0 || targetIndex >= slotPositions.Length) return;

        SlotData data = currentLevelQueue[0];
        currentLevelQueue.RemoveAt(0);

        QueueSlot newSlot = Instantiate(queueSlotPrefab, spawnPoint.position, Quaternion.identity, uiContainer);

        bool isKnown = (targetIndex < 2);
        newSlot.InitializeSlot(data.colorType, data.capacity, data.displayColor, slotPositions[targetIndex].position, isKnown);

        activeSlots.Add(newSlot);
    }

    public void ResetCombo()
    {
        currentCombo = 1;
        Debug.Log("Đã Reset Combo về 1 do không có đường / bấm sai!");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFailClickSound();
        }
    }

    public bool TryAddBlock(ColorType blockColor, out Transform targetSlotTransform)
    {
        targetSlotTransform = null;

        if (activeSlots.Count == 0)
        {
            ResetCombo(); 
            return false;
        }

        for (int i = 0; i < activeSlots.Count; i++)
        {
            QueueSlot slot = activeSlots[i];

            if (slot.isCapacityKnown && slot.slotColor == blockColor && slot.currentCount < slot.maxCapacity)
            {
                slot.currentCount++;
                targetSlotTransform = slot.transform;

                if (AudioManager.Instance != null) AudioManager.Instance.PlayObjectClickSound();

                if (slot.currentCount >= slot.maxCapacity)
                {
                    StartCoroutine(CompleteSpecificSlot(slot));
                }

                return true;
            }
        }

        ResetCombo();
        return false;
    }

    private IEnumerator CompleteSpecificSlot(QueueSlot slot)
    {
        yield return new WaitForSeconds(0.3f);

        currentCombo = Mathf.Min(currentCombo + 1, 10);
        Debug.Log("Combo hien tai la: " + currentCombo);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMatchSoundWithCombo(currentCombo);
        }

        int index = activeSlots.IndexOf(slot);
        if (index == -1) yield break;

        activeSlots.RemoveAt(index);
        Destroy(slot.gameObject);

        for (int i = index; i < activeSlots.Count; i++)
        {
            activeSlots[i].MoveToNewPosition(slotPositions[i].position);

            if (i == 1 && !activeSlots[i].isCapacityKnown)
            {
                activeSlots[i].RevealCapacity();
            }
        }

        SpawnNextSlot(2);
    }
}