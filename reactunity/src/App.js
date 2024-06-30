import React, { useEffect, useCallback, useState } from 'react';
import { Unity, useUnityContext } from 'react-unity-webgl';
import { useCoinbaseWallet } from './Components/CoinbaseWalletConnector';
import uploadToPinata from './Components/PinataUploader';

// Sample local images stored in React
import image1 from './images/Arceus.png';
import image2 from './images/Celebi.png';
import image3 from './images/Charizard.jpg';

const App = () => {
  const { connectWallet } = useCoinbaseWallet();
  const { unityProvider, addEventListener, removeEventListener } = useUnityContext({
    loaderUrl: 'Build/build.loader.js',
    dataUrl: 'Build/build.data',
    frameworkUrl: 'Build/build.framework.js',
    codeUrl: 'Build/build.wasm',
  });

  const [selectedImageIndex, setSelectedImageIndex] = useState(null);

  const handleConnectWallet = useCallback(() => {
    connectWallet();
  }, [connectWallet]);

  const handleSendNFTImage = useCallback(async (index) => {
    try {
      let base64Image = null;
      switch (index) {
        case 1:
          base64Image = await getBase64Image(image1);
          break;
        case 2:
          base64Image = await getBase64Image(image2);
          break;
        case 3:
          base64Image = await getBase64Image(image3);
          break;
        default:
          console.error('Invalid image index:', index);
          return;
      }
      
      const pinataResponse = await uploadToPinata(base64Image);
      if (pinataResponse) {
        console.log('Image uploaded to Pinata:', pinataResponse);
      }
    } catch (error) {
      console.error('Error uploading image to Pinata:', error);
    }
  }, []);

  const getBase64Image = (img) => {
    return new Promise((resolve, reject) => {
      // Fetch the image as Blob
      fetch(img)
        .then(res => res.blob())
        .then(blob => {
          const reader = new FileReader();
          reader.onloadend = () => {
            resolve(reader.result.split(',')[1]);
          };
          reader.onerror = reject;
          reader.readAsDataURL(blob); // Read as data URL
        })
        .catch(error => {
          console.error('Error fetching image:', error);
          reject(error);
        });
    });
  };

  useEffect(() => {
    addEventListener("ConnectCoinbaseWallet", handleConnectWallet);
    addEventListener("SendNFTImage", handleSendNFTImage);

    return () => {
      removeEventListener("ConnectCoinbaseWallet", handleConnectWallet);
      removeEventListener("SendNFTImage", handleSendNFTImage);
    };
  }, [addEventListener, removeEventListener, handleConnectWallet, handleSendNFTImage]);

  const handleReceiveIndex = useCallback((index) => {
    // Received index from Unity, set it to state
    setSelectedImageIndex(index);
  }, []);

  return (
    <div>
      <Unity unityProvider={unityProvider} />
      <button onClick={() => unityProvider.send('ConnectCoinbaseWallet')}>
        Connect Wallet
      </button>
      <button onClick={() => unityProvider.send('SendNFTImage', selectedImageIndex)}>
        Send NFT Image
      </button>
    </div>
  );
};

export default App;
