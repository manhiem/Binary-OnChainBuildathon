using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    public void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }
}
