using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Эффекты")]
    public GameObject mergeEffect;

    [Header("Линия")]
    public Sprite lineSprite;

    private Item myItem;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;
    private Image myImage;

    private bool isDragging = false;
    private bool isLineMode = false;
    private bool isPressed = false;

    private float holdTimer = 0.15f;
    private const float HOLD_TIME = 0.3f;

    // Линия — сегменты
    private List<GameObject> lineSegments = new List<GameObject>();
    private List<Vector2> linePoints = new List<Vector2>();

    private void Awake()
    {
        myItem = GetComponent<Item>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        myImage = GetComponent<Image>();
    }

    // ===== НАЖАТИЕ / ОТПУСКАНИЕ =====

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        holdTimer = 0f;
        isDragging = false;
        isLineMode = false;
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        holdTimer = 0f;

        if (isDragging || isLineMode)
            return;

        ClearLineSegments();
        linePoints.Clear();
        UnhighlightAll();
    }

    // ===== ПЕРЕТАСКИВАНИЕ =====

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLineMode) return;

        isDragging = true;
        rectTransform.localScale = Vector3.one * 1.1f;

        if (myImage != null)
            myImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLineMode) return;

        if (isDragging)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (myImage != null)
            myImage.raycastTarget = true;

        rectTransform.localScale = Vector3.one;

        if (isLineMode)
        {
            EndLineMode(eventData);
        }
        else if (isDragging)
        {
            EndDragMode(eventData);
        }

        isDragging = false;
        isLineMode = false;
        ClearLineSegments();
        linePoints.Clear();
        UnhighlightAll();
    }

    // ===== ТАЙМЕР УДЕРЖАНИЯ =====

    private void Update()
    {
        if (isPressed && !isDragging && !isLineMode)
        {
            holdTimer += Time.deltaTime;

            // Если палец двигается, ускоряем переход к линии
            if (Input.touchCount > 0 && Input.GetTouch(0).deltaPosition.magnitude > 5f)
                holdTimer += Time.deltaTime * 2f;

            if (holdTimer >= HOLD_TIME)
            {
                EnterLineMode();
            }
        }

        if (isLineMode)
        {
            DrawLineToPointer();
        }
    }

    // ===== РЕЖИМ ЛИНИИ =====

    private void EnterLineMode()
    {
        isLineMode = true;
        isDragging = false;
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition = originalPosition;

        linePoints.Clear();
        linePoints.Add(rectTransform.anchoredPosition + new Vector2(rectTransform.sizeDelta.x / 2 + 5, 0));

        HighlightValidTargets(true);
    }

    private Vector2 GetPointerPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;

        return Input.mousePosition;
    }

    private void DrawLineToPointer()
    {
        Vector2 pointerPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            GetPointerPosition(),
            null,
            out pointerPos
        );

        if (linePoints.Count == 0) return;

        Vector2 lastPoint = linePoints[linePoints.Count - 1];
        float dist = Vector2.Distance(lastPoint, pointerPos);

        // Заполняем промежутки между последней точкой и текущей
        int steps = Mathf.CeilToInt(dist / 10f); // Точка каждые 10 пикселей
        for (int i = 1; i <= steps; i++)
        {
            Vector2 intermediate = Vector2.Lerp(lastPoint, pointerPos, (float)i / steps);
            linePoints.Add(intermediate);
        }

        ClearLineSegments();

        for (int i = 0; i < linePoints.Count - 1; i++)
        {
            DrawSegment(linePoints[i], linePoints[i + 1]);
        }
    }

    private void ClearLineSegments()
    {
        foreach (GameObject seg in lineSegments)
        {
            Destroy(seg);
        }
        lineSegments.Clear();
    }

    private void DrawSegment(Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        float distance = direction.magnitude;

        if (distance < 1f) return;

        GameObject seg = new GameObject("LineSeg");
        seg.transform.SetParent(transform.parent);
        seg.transform.SetAsFirstSibling();

        RectTransform segRect = seg.AddComponent<RectTransform>();
        Image segImage = seg.AddComponent<Image>();
        segImage.raycastTarget = false;

        segImage.sprite = lineSprite;
        segImage.type = Image.Type.Tiled;
        segImage.preserveAspect = true;
        segImage.color = new Color(1f, 1f, 1f, 1f);

        segRect.pivot = new Vector2(0, 0.5f);
        segRect.anchoredPosition = from;
        segRect.sizeDelta = new Vector2(distance, 12f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        segRect.rotation = Quaternion.Euler(0, 0, angle);

        lineSegments.Add(seg);
    }

    private void EndLineMode(PointerEventData eventData)
    {
        if (myImage != null)
            myImage.raycastTarget = false;

        Item otherItem = FindItemUnderPointer(eventData);

        if (myImage != null)
            myImage.raycastTarget = true;

        if (otherItem != null && myItem.CanMergeWith(otherItem))
        {
            MergeWith(otherItem);
        }
    }

    // ===== РЕЖИМ ПЕРЕТАСКИВАНИЯ =====

    private void EndDragMode(PointerEventData eventData)
    {
        Item otherItem = FindItemUnderPointer(eventData);

        if (otherItem != null && myItem.CanMergeWith(otherItem))
        {
            MergeWith(otherItem);
            return;
        }

        if (IsOverTrash(eventData))
        {
            Destroy(gameObject);
            return;
        }

        rectTransform.anchoredPosition = originalPosition;
    }

    // ===== ПОИСК ЦЕЛИ =====

    private Item FindItemUnderPointer(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == gameObject) continue;

            Item item = result.gameObject.GetComponent<Item>();
            if (item != null)
                return item;
        }
        return null;
    }

    private bool IsOverTrash(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == gameObject) continue;
            if (result.gameObject.CompareTag("Trash"))
                return true;
        }
        return false;
    }

    // ===== СЛИЯНИЕ =====
    private static bool isMerging = false;

    private void MergeWith(Item other)
    {
        AudioManager.Instance.PlayMerge();

        if (isMerging) return;
        if (this == null || other == null) return;
        if (gameObject == null || other.gameObject == null) return;

        isMerging = true;

        RectTransform otherRect = other.GetComponent<RectTransform>();
        Vector2 mergePosition = otherRect.anchoredPosition;

        Destroy(other.gameObject);
        Destroy(gameObject);

        GameObject newItem = Instantiate(
            GameManager.Instance.itemSpawner.itemPrefab,
            GameManager.Instance.itemSpawner.itemsContainer
        );
        newItem.tag = "Item";

        RectTransform newRect = newItem.GetComponent<RectTransform>();
        newRect.anchorMin = new Vector2(0.5f, 0.5f);
        newRect.anchorMax = new Vector2(0.5f, 0.5f);
        newRect.pivot = new Vector2(0.5f, 0.5f);
        newRect.anchoredPosition = mergePosition;

        Item newItemScript = newItem.GetComponent<Item>();
        newItemScript.Setup(myItem.level + 1);

        bool alreadyUnlocked = AlbumManager.Instance.IsUnlocked(newItemScript.level);
        newItemScript.ShowAlbumButton(newItemScript.level >= 2 && !alreadyUnlocked);

        // Эффект ПОСЛЕ нового предмета, последним ребёнком (поверх всего)
        if (mergeEffect != null)
        {
            GameObject mergeFX = Instantiate(mergeEffect, GameManager.Instance.itemSpawner.itemsContainer);
            mergeFX.transform.SetAsLastSibling();
            RectTransform fxRect = mergeFX.GetComponent<RectTransform>();
            fxRect.anchoredPosition = mergePosition + new Vector2(10, 0); // 20 пикселей вправо
        }

        isMerging = false;
        SaveManager.Instance.SaveGame();
    }

    // ===== ПОДСВЕТКА =====

    private void HighlightValidTargets(bool enable)
    {
        Item[] allItems = FindObjectsByType<Item>(FindObjectsSortMode.None);
        foreach (Item item in allItems)
        {
            if (item == myItem) continue;
            if (myItem.CanMergeWith(item))
            {
                Image img = item.GetComponent<Image>();
                if (img != null)
                {
                    if (enable)
                    {
                        img.color = new Color(0.96f, 0.87f, 0.70f);
                        item.transform.localScale = Vector3.one * 1.15f;
                    }
                    else
                    {
                        img.color = Color.white;
                        item.transform.localScale = Vector3.one;
                    }
                }
            }
        }
    }

    private void UnhighlightAll()
    {
        Item[] allItems = FindObjectsByType<Item>(FindObjectsSortMode.None);
        foreach (Item item in allItems)
        {
            Image img = item.GetComponent<Image>();
            if (img != null) img.color = Color.white;
            item.transform.localScale = Vector3.one;
        }

    }

    // ===== ОГРАНИЧЕНИЕ ДЛИНЫ ЛИНИИ =====

    private float GetMaxLineDistance(int level)
    {
        return float.MaxValue; // Без ограничений
    }

    // ===== ОЧИСТКА =====

    private void OnDestroy()
    {
        ClearLineSegments();
    }
}