import React from 'react';
import {Unity,  useUnityContext } from 'react-unity-webgl'; // Import Unity and useUnityContext
import { useCoinbaseWallet } from './Components/CoinbaseWalletConnector'; // Import your CoinbaseWalletConnector

const App = () => {
  const { connectWallet } = useCoinbaseWallet();
  const unityContext = useUnityContext({
    loaderUrl: "Build/build.loader.js",
    dataUrl: "Build/build.data",
    frameworkUrl: "Build/build.framework.js",
    codeUrl: "Build/build.wasm",
  });

  React.useEffect(() => {
    unityContext.on("ConnectCoinbaseWallet", () => {
      connectWallet();
    });
  }, [connectWallet, unityContext]);

    return (
        <div>
            <Unity unityContext={unityContext} />
            <button onClick={() => unityContext.send("ConnectCoinbaseWallet")}>
                Connect Wallet
            </button>
        </div>
    );
};

export default App;
