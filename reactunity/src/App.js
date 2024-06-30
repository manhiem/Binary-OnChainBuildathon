import React, { useEffect, useCallback } from 'react';
import { Unity, useUnityContext } from 'react-unity-webgl';
import { useCoinbaseWallet } from './Components/CoinbaseWalletConnector';

const App = () => {
  const { connectWallet } = useCoinbaseWallet();
  const { unityProvider, addEventListener, removeEventListener } = useUnityContext({
    loaderUrl: 'Build/build.loader.js',
    dataUrl: 'Build/build.data',
    frameworkUrl: 'Build/build.framework.js',
    codeUrl: 'Build/build.wasm',
  });

  const handleSetScore = useCallback(() => {
    connectWallet();
  }, [connectWallet]);

  useEffect(() => {
    addEventListener("ConnectCoinbaseWallet", handleSetScore);
    return () => {
      removeEventListener("ConnectCoinbaseWallet", handleSetScore);
    };
  }, [addEventListener, removeEventListener, handleSetScore]);

  return (
    <div>
      <Unity unityProvider={unityProvider} />
      <button onClick={() => unityProvider.send('ConnectCoinbaseWallet')}>
        Connect Wallet
      </button>
    </div>
  );
};

export default App;
