using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CoinbaseWalletConnector : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ConnectCoinbaseWallet();

    [DllImport("__Internal")]
    private static extern void SendNFTImage(int index);

    [DllImport("__Internal")]
    private static extern void MintNFT();

    public Button connectButton;
    public Button nftButton;
    public Button mintNFTButton;
    public Texture2D imageTexture;

    void Start()
    {
        connectButton.onClick.AddListener(OnConnectButtonClicked);
        nftButton.onClick.AddListener(OnNFTButtonClicked);
        mintNFTButton.onClick.AddListener(OnMintButtonClicked);
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

    void OnMintButtonClicked()
    {
        MintNFT(); ;
    }
}
