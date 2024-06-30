mergeInto(LibraryManager.library, {
  ConnectCoinbaseWallet: function () {
    window.dispatchReactUnityEvent("ConnectCoinbaseWallet");
  },

  SendNFTImage: function(index) {
    window.dispatchReactUnityEvent("SendNFTImage", index);
  },

  MintNFT: function() {
    window.dispatchReactUnityEvent("MintNFT");
  },
});