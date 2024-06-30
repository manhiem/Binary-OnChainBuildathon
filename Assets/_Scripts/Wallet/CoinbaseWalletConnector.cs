using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CoinbaseWalletConnector : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ConnectCoinbaseWallet();

    [DllImport("__Internal")]
    private static extern void SendNFTImage(string imageString);

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
        // Convert the texture to a byte array
        byte[] imageBytes = imageTexture.EncodeToPNG();
        // Convert the byte array to a base64 string
        string base64Image = System.Convert.ToBase64String(imageBytes);

        SendNFTImage(base64Image);
    }
}
