using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CoinbaseWalletConnector : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ConnectCoinbaseWallet();

    [DllImport("__Internal")]
    private static extern void SendNFTImage(int index);

    public Button connectButton;
    public Button nftButton;
    public Texture2D imageTexture;

    void Start()
    {
        connectButton.onClick.AddListener(OnConnectButtonClicked);
        nftButton.onClick.AddListener(OnNFTButtonClicked);
    }

    void OnConnectButtonClicked()
    {
        ConnectCoinbaseWallet();
    }

    void OnNFTButtonClicked()
    {
        int randomIndex = Random.Range(0, 4);
        SendNFTImage(randomIndex);
    }
}
