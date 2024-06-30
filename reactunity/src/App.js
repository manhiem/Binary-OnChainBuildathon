import React, { useEffect, useCallback, useState } from 'react';
import { Unity, useUnityContext } from 'react-unity-webgl';
import { contractAddress, contractABI } from './contract';
import { useCoinbaseWallet } from './Components/CoinbaseWalletConnector';
import { uploadToPinata, uploadMetadataToPinata } from './Components/PinataUploader';
import { Web3Provider } from '@ethersproject/providers';
import { ethers } from 'ethers';

// Sample local images stored in React
import image1 from './images/Arceus.png';
import image2 from './images/Celebi.png';
import image3 from './images/Charizard.jpg';

// Unity Build Importing
const App = () => {
  const { connectWallet } = useCoinbaseWallet();
  const { unityProvider, addEventListener, removeEventListener } = useUnityContext({
    loaderUrl: 'Build/build.loader.js',
    dataUrl: 'Build/build.data',
    frameworkUrl: 'Build/build.framework.js',
    codeUrl: 'Build/build.wasm',
  });

  // State for managing selected image and NFT metadata
  const [selectedImageIndex, setSelectedImageIndex] = useState(null);
  const [latestNFTMetadata, setLatestNFTMetadata] = useState(null);
  const [provider, setProvider] = useState(null); // Ethereum provider
  const [signer, setSigner] = useState(null); // Ethereum signer

  // Effect to handle provider changes
  useEffect(() => {
    if (provider) {
      const ethersProvider = new Web3Provider(provider);
      console.log('Coinbase Wallet connected:', ethersProvider);
    }
  }, [provider]);

  // Function to connect wallet
  const handleConnectWallet = useCallback(async () => {
    try {
      const walletProvider = await connectWallet();
      setProvider(walletProvider); // Set Ethereum provider
      const ethSigner = walletProvider.signer;
      setSigner(ethSigner); // Update signer state
    } catch (error) {
      console.error('Error connecting wallet:', error);
    }
  }, [connectWallet]);

  // Function to upload image to Pinata
  const handleSendNFTImage = useCallback(async (index) => {
    try {
      let base64Image = null;
      let imageName = '';
      switch (index) {
        case 0:
          base64Image = await getBase64Image(image1);
          imageName = 'Arceus';
          break;
        case 1:
          base64Image = await getBase64Image(image2);
          imageName = 'Celebi';
          break;
        case 2:
          base64Image = await getBase64Image(image3);
          imageName = 'Charizard';
          break;
        default:
          console.error('Invalid image index:', index);
          return;
      }

      const pinataResponse = await uploadToPinata(base64Image);

      // Upload the metadata of the NFT
      if (pinataResponse) {
        const cID = pinataResponse.IpfsHash;
        const metadata = {
          name: imageName,
          description: `${imageName} NFT`,
          image: `ipfs://${cID}`,
        };

        const pinataMetadataResponse = await uploadMetadataToPinata(metadata);
        if (!pinataMetadataResponse) {
          console.error('Metadata upload failed');
          return;
        }

        const metadataCID = pinataMetadataResponse.IpfsHash;
        setLatestNFTMetadata({ ...metadata, cid: metadataCID });
        console.log('Image uploaded to Pinata:', metadataCID);
      }
    } catch (error) {
      console.error('Error uploading image to Pinata:', error);
    }
  }, []);

  // Function to convert image to base64
  const getBase64Image = (img) => {
    return new Promise((resolve, reject) => {
      fetch(img)
        .then(res => res.blob())
        .then(blob => {
          const reader = new FileReader();
          reader.onloadend = () => {
            resolve(reader.result.split(',')[1]);
          };
          reader.readAsDataURL(blob);
        })
        .catch(error => {
          console.error('Error fetching image:', error);
          reject(error);
        });
    });
  };

  // Function to mint the latest NFT
  const mintLatestNFT = useCallback(async () => {
    if (!signer) {
      console.error('Signer is not available. Connect the wallet first.');
      return;
    }
    if (!latestNFTMetadata) {
      console.error('No metadata available. Upload an NFT first.');
      return;
    }

    try {
      const contract = new ethers.Contract(contractAddress, contractABI, signer);
      const tx = await contract.mintNFT(latestNFTMetadata.cid);
      await tx.wait();
      console.log('NFT minted successfully:', tx);
    } catch (error) {
      console.error('Error minting NFT:', error);
    }
  }, [signer, latestNFTMetadata]);

  // Effect to handle Unity events
  useEffect(() => {
    addEventListener("ConnectCoinbaseWallet", handleConnectWallet);
    addEventListener("SendNFTImage", handleSendNFTImage);
    addEventListener("MintNFT", mintLatestNFT);

    return () => {
      removeEventListener("ConnectCoinbaseWallet", handleConnectWallet);
      removeEventListener("SendNFTImage", handleSendNFTImage);
      removeEventListener("MintNFT", mintLatestNFT);
    };
  }, [addEventListener, removeEventListener, handleConnectWallet, handleSendNFTImage, mintLatestNFT]);

  // Function to handle Unity's selected image index
  const handleReceiveIndex = useCallback((index) => {
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
      <button onClick={() => unityProvider.send('MintNFT', selectedImageIndex)}>
        Mint NFT
      </button>
    </div>
  );
};

export default App;
