mergeInto(LibraryManager.library, {
  CreateProfile: function() {
    window.dispatchReactUnityEvent("CreateProfile");
  },

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