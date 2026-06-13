using UnityEngine;
using UnityEngine.UI;

public class AlbumButtonHandler : MonoBehaviour
{
    private Item myItem;
    private Button button;

    private void Awake()
    {
        myItem = GetComponentInParent<Item>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (myItem == null) return;
        if (myItem.level < 2) return;

        // Проверяем, что все предыдущие уровни добавлены
        for (int i = 2; i < myItem.level; i++)
        {
            if (!AlbumManager.Instance.IsUnlocked(i))
            {
                // Показываем подсказку
                ShowTip($"Сначала добавьте уровень {i}");
                return;
            }
        }

        // Если этот уровень уже в альбоме
        if (AlbumManager.Instance.IsUnlocked(myItem.level))
        {
            ShowTip("Этот уровень уже в альбоме");
            return;
        }

        AlbumConfirmation.Instance.Show(myItem);
    }

    private void ShowTip(string message)
    {
        // Используем всплывающую подсказку
        TipManager.Instance.Show(message);
    }
}
