using UnityEngine;
using UnityEngine.UI;

public class AlbumOpenButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OpenAlbum);
    }

    private void OpenAlbum()
    {
        AlbumPanel.Instance.Show();
    }
}