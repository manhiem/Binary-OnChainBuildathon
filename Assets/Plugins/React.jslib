mergeInto(LibraryManager.library, {
  ConnectCoinbaseWallet: function () {
    window.dispatchReactUnityEvent("ConnectCoinbaseWallet");
  },

  SendNFTImage: function(imageName) {
    window.dispatchReactUnityEvent("SendNFTImage", UTF8ToString(imageName));
  },
});