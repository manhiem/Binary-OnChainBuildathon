import { uploadToPinata, uploadMetadataToPinata } from './Components/PinataUploader';
import React, { useState, useEffect, useCallback } from 'react';
import Web3 from 'web3';
import image1 from './images/Arceus.png'; // Replace with actual path
import image2 from './images/Celebi.png'; // Replace with actual path
import image3 from './images/Dragonite.png'; // Replace with actual path
import { Unity, useUnityContext } from 'react-unity-webgl';

const CONTRACT_ADDRESS = "0x8521DFFf5ffA2943a0A98DB4AA3c0455b93586f3";
const contractABI = [
	{
		"inputs": [],
		"stateMutability": "nonpayable",
		"type": "constructor"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "owner",
				"type": "address"
			},
			{
				"indexed": true,
				"internalType": "address",
				"name": "approved",
				"type": "address"
			},
			{
				"indexed": true,
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			}
		],
		"name": "Approval",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "owner",
				"type": "address"
			},
			{
				"indexed": true,
				"internalType": "address",
				"name": "operator",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "bool",
				"name": "approved",
				"type": "bool"
			}
		],
		"name": "ApprovalForAll",
		"type": "event"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "to",
				"type": "address"
			},
			{
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			}
		],
		"name": "approve",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "user",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "string",
				"name": "oldCarUri",
				"type": "string"
			},
			{
				"indexed": false,
				"internalType": "string",
				"name": "newCarUri",
				"type": "string"
			}
		],
		"name": "CarSwapped",
		"type": "event"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "nickname",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "fusionId",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "uri",
				"type": "string"
			}
		],
		"name": "createProfile",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "toAddress",
				"type": "address"
			},
			{
				"internalType": "string",
				"name": "uri",
				"type": "string"
			}
		],
		"name": "mintNFT",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "user",
				"type": "address"
			},
			{
				"indexed": false,
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "string",
				"name": "uri",
				"type": "string"
			}
		],
		"name": "NFTMinted",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "previousOwner",
				"type": "address"
			},
			{
				"indexed": true,
				"internalType": "address",
				"name": "newOwner",
				"type": "address"
			}
		],
		"name": "OwnershipTransferred",
		"type": "event"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "user",
				"type": "address"
			},
			{
				"indexed": true,
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			},
			{
				"indexed": false,
				"internalType": "string",
				"name": "uri",
				"type": "string"
			}
		],
		"name": "ProfileCreated",
		"type": "event"
	},
	{
		"inputs": [],
		"name": "refuel",
		"outputs": [],
		"stateMutability": "payable",
		"type": "function"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "user",
				"type": "address"
			}
		],
		"name": "Refueled",
		"type": "event"
	},
	{
		"inputs": [],
		"name": "renounceOwnership",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "from",
				"type": "address"
			},
			{
				"internalType": "address",
				"name": "to",
				"type": "address"
			},
			{
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			}
		],
		"name": "safeTransferFrom",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "from",
				"type": "address"
			},
			{
				"internalType": "address",
				"name": "to",
				"type": "address"
			},
			{
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			},
			{
				"internalType": "bytes",
				"name": "data",
				"type": "bytes"
			}
		],
		"name": "safeTransferFrom",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "operator",
				"type": "address"
			},
			{
				"internalType": "bool",
				"name": "approved",
				"type": "bool"
			}
		],
		"name": "setApprovalForAll",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "string",
				"name": "newCarUri",
				"type": "string"
			}
		],
		"name": "swapCar",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"anonymous": false,
		"inputs": [
			{
				"indexed": true,
				"internalType": "address",
				"name": "from",
				"type": "address"
			},
			{
				"indexed": true,
				"internalType": "address",
				"name": "to",
				"type": "address"
			},
			{
				"indexed": true,
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			}
		],
		"name": "Transfer",
		"type": "event"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "from",
				"type": "address"
			},
			{
				"internalType": "address",
				"name": "to",
				"type": "address"
			},
			{
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			}
		],
		"name": "transferFrom",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "newOwner",
				"type": "address"
			}
		],
		"name": "transferOwnership",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "raceScore",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "mazeScore",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "nickname",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "fusionId",
				"type": "string"
			}
		],
		"name": "updateProfile",
		"outputs": [],
		"stateMutability": "nonpayable",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "owner",
				"type": "address"
			}
		],
		"name": "balanceOf",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			}
		],
		"name": "getApproved",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "getTopPlayersMaze",
		"outputs": [
			{
				"internalType": "address[]",
				"name": "",
				"type": "address[]"
			},
			{
				"internalType": "uint256[]",
				"name": "",
				"type": "uint256[]"
			},
			{
				"internalType": "string[]",
				"name": "",
				"type": "string[]"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "getTopPlayersRace",
		"outputs": [
			{
				"internalType": "address[]",
				"name": "",
				"type": "address[]"
			},
			{
				"internalType": "uint256[]",
				"name": "",
				"type": "uint256[]"
			},
			{
				"internalType": "string[]",
				"name": "",
				"type": "string[]"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "user",
				"type": "address"
			}
		],
		"name": "getUserAchievements",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "",
				"type": "string"
			},
			{
				"internalType": "string",
				"name": "",
				"type": "string"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "owner",
				"type": "address"
			},
			{
				"internalType": "address",
				"name": "operator",
				"type": "address"
			}
		],
		"name": "isApprovedForAll",
		"outputs": [
			{
				"internalType": "bool",
				"name": "",
				"type": "bool"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"name": "mazeLeaderboard",
		"outputs": [
			{
				"internalType": "address",
				"name": "player",
				"type": "address"
			},
			{
				"internalType": "uint256",
				"name": "score",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "name",
		"outputs": [
			{
				"internalType": "string",
				"name": "",
				"type": "string"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "owner",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			}
		],
		"name": "ownerOf",
		"outputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"name": "profiles",
		"outputs": [
			{
				"internalType": "string",
				"name": "nickname",
				"type": "string"
			},
			{
				"internalType": "uint256",
				"name": "raceScore",
				"type": "uint256"
			},
			{
				"internalType": "uint256",
				"name": "mazeScore",
				"type": "uint256"
			},
			{
				"internalType": "string",
				"name": "fusionId",
				"type": "string"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"name": "raceLeaderboard",
		"outputs": [
			{
				"internalType": "address",
				"name": "player",
				"type": "address"
			},
			{
				"internalType": "uint256",
				"name": "score",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "refuelCost",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "bytes4",
				"name": "interfaceId",
				"type": "bytes4"
			}
		],
		"name": "supportsInterface",
		"outputs": [
			{
				"internalType": "bool",
				"name": "",
				"type": "bool"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [],
		"name": "symbol",
		"outputs": [
			{
				"internalType": "string",
				"name": "",
				"type": "string"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "uint256",
				"name": "tokenId",
				"type": "uint256"
			}
		],
		"name": "tokenURI",
		"outputs": [
			{
				"internalType": "string",
				"name": "",
				"type": "string"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"name": "userNFTs",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	},
	{
		"inputs": [
			{
				"internalType": "address",
				"name": "",
				"type": "address"
			}
		],
		"name": "userRefuels",
		"outputs": [
			{
				"internalType": "uint256",
				"name": "",
				"type": "uint256"
			}
		],
		"stateMutability": "view",
		"type": "function"
	}
]
;

const App = () => {

  const { unityProvider, addEventListener, removeEventListener } = useUnityContext({
    loaderUrl: 'Build/build.loader.js',
    dataUrl: 'Build/build.data',
    frameworkUrl: 'Build/build.framework.js',
    codeUrl: 'Build/build.wasm',
  });
	
	 useEffect(() => {
    addEventListener("ConnectCoinbaseWallet", connectWallet);
    addEventListener("SendNFTImage", handleSendNFTImage);
	addEventListener("CreateProfile", createProfile);
	addEventListener("UpdateProfile", updateProfile);
	addEventListener("Refuel", refuel);
	addEventListener("GetTopRaceScore", getTopPlayersRace);
	addEventListener("GetTopMazeScore", getTopPlayersMaze);
	addEventListener("GetAchievements", getUserAchievements);
	addEventListener("SwapCar", swapCar);

    return () => {
    removeEventListener("ConnectCoinbaseWallet", connectWallet);
    removeEventListener("SendNFTImage", handleSendNFTImage);
	removeEventListener("CreateProfile", createProfile);
	removeEventListener("UpdateProfile", updateProfile);
	removeEventListener("Refuel", refuel);
	removeEventListener("GetTopRaceScore", getTopPlayersRace);
	removeEventListener("GetTopMazeScore", getTopPlayersMaze);
	removeEventListener("GetAchievements", getUserAchievements);
	removeEventListener("SwapCar", swapCar);
		 };
		 
  }, [connectWallet, handleSendNFTImage, createProfile, updateProfile, refuel, getTopPlayersRace, getTopPlayersMaze, getUserAchievements, swapCar]);
	
  const [account, setAccount] = useState('');
  const [contract, setContract] = useState(null);
  const [nickname, setNickname] = useState('');
  const [car, setCar] = useState('');
  const [fusionId, setFusionId] = useState('');
  const [uri, setUri] = useState('');
  const [raceScore, setRaceScore] = useState('');
  const [mazeScore, setMazeScore] = useState('');
  const [oldCarUri, setOldCarUri] = useState('');
  const [newCarUri, setNewCarUri] = useState('');
  const [imageIndex, setImageIndex] = useState(0);

  useEffect(() => {
    if (window.ethereum) {
      const web3 = new Web3(window.ethereum);
      const contractInstance = new web3.eth.Contract(contractABI, CONTRACT_ADDRESS);
      setContract(contractInstance);
    } else {
      alert('Please install MetaMask to use this feature.');
    }
  }, []);

  const connectWallet = async () => {
    if (window.ethereum) {
      try {
        const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
        setAccount(accounts[0]);
        
        const web3 = new Web3(window.ethereum);
        const message = `Sign this message to confirm your wallet address: ${accounts[0]}`;
        await web3.eth.personal.sign(message, accounts[0]);

      } catch (error) {
        console.error('Error connecting to wallet:', error);
      }
    } else {
      alert('Please install MetaMask to use this feature.');
    }
  };

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
          imageName = 'Dragonite';
          break;
        default:
          console.error('Invalid image index:', index);
          return null;
      }

      const pinataResponse = await uploadToPinata(base64Image);

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
          return null;
        }

        const metadataCID = pinataMetadataResponse.IpfsHash;
        setUri(`ipfs://${metadataCID}`);
        return `ipfs://${metadataCID}`;
      }
    } catch (error) {
      console.error('Error uploading image to Pinata:', error);
      return null;
    }
  }, []);

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

  const createProfile = async () => {
    try {
      const metadataUri = await handleSendNFTImage(imageIndex);
      if (metadataUri) {
        await contract.methods.createProfile(nickname, fusionId, metadataUri).send({ from: account ,gas:2000000});
        alert('Profile created and NFT minted!');
      }
    } catch (error) {
      console.error(error);
      alert('An error occurred while creating the profile.');
    }
  };

  const updateProfile = async () => {
    try {
      await contract.methods.updateProfile(raceScore, mazeScore, nickname, fusionId).send({ from: account,gas:200000 });
      alert('Profile updated!');
    } catch (error) {
      console.error(error);
      alert('An error occurred while updating the profile.');
    }
  };

  const refuel = async () => {
    try {
      await contract.methods.refuel().send({ from: account,gas:200000 });
      alert('Refueled successfully!');
    } catch (error) {
      console.error(error);
      alert('An error occurred while refueling.');
    }
  };

  const swapCar = async () => {
    const metadataUri = await handleSendNFTImage(imageIndex);
    try {
      if (metadataUri) {
      await contract.methods.swapCar(metadataUri).send({ from: account,gas:200000 });
      alert('Car swapped successfully!');}
    } catch (error) {
      console.error(error);
      alert('An error occurred while swapping the car.');
    }
  };

  const mintNFT = async () => {
    try {
      const metadataUri = await handleSendNFTImage(imageIndex);
      if (metadataUri) {
        const gasEstimate = await contract.methods.mintNFT(account, metadataUri).estimateGas({ from: account });

        await contract.methods.mintNFT(account, metadataUri).send({
          from: account,
          gas: gasEstimate,
        });
        alert('NFT minted successfully!');
      }
    } catch (error) {
      console.error(error);
      alert('An error occurred while minting the NFT.');
    }
  };

  const getTopPlayersRace = async () => {
    try {
      const topPlayers = await contract.methods.getTopPlayersRace().call();
      console.log(topPlayers);
      alert(`Top Race Players: ${JSON.stringify(topPlayers)}`);
    } catch (error) {
      console.error(error);
      alert('An error occurred while fetching the top players.');
    }
  };

  const getTopPlayersMaze = async () => {
    try {
      const topPlayers = await contract.methods.getTopPlayersMaze().call();
      console.log(topPlayers);
      alert(`Top Maze Players: ${JSON.stringify(topPlayers)}`);
    } catch (error) {
      console.error(error);
      alert('An error occurred while fetching the top players.');
    }
  };

  const getUserAchievements = async () => {
    try {
      const achievements = await contract.methods.getUserAchievements(account).call();
      console.log(achievements);
      alert(`User Achievements: ${JSON.stringify(achievements)}`);
    } catch (error) {
      console.error(error);
      alert('An error occurred while fetching user achievements.');
    }
  };

  return (
	  <div>
		  <Unity unityProvider={unityProvider} />
    </div>
  );
};

export default App;
