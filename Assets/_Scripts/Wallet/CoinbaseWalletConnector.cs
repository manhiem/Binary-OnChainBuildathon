using UnityEngine;
using UnityEngine.UI;

public class CoinbaseWalletConnector : MonoBehaviour
{
    public Button connectButton;

    void Start()
    {
        connectButton.onClick.AddListener(OnConnectButtonClicked);
    }

    void OnConnectButtonClicked()
    {
        Application.ExternalCall("ConnectCoinbaseWallet");
    }
}
