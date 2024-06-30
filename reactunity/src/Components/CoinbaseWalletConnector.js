import { useEffect, useState } from 'react';
import { Web3Provider } from '@ethersproject/providers';
import { CoinbaseWalletSDK } from '@coinbase/wallet-sdk';

const APP_NAME = "YourAppName";
const APP_LOGO_URL = "https://example.com/logo.png";
const DEFAULT_ETH_JSONRPC_URL = "https://mainnet.infura.io/v3/your-project-id";
const DEFAULT_CHAIN_ID = 1; // Mainnet

const coinbaseWallet = new CoinbaseWalletSDK({
  appName: APP_NAME,
  appLogoUrl: APP_LOGO_URL,
  darkMode: false
});

const ethereum = coinbaseWallet.makeWeb3Provider(DEFAULT_ETH_JSONRPC_URL, DEFAULT_CHAIN_ID);

export const useCoinbaseWallet = () => {
  const [provider, setProvider] = useState(null);

  useEffect(() => {
    if (provider) {
      const web3Provider = new Web3Provider(provider);
      console.log('Coinbase Wallet connected:', web3Provider);
    }
  }, [provider]);

  const connectWallet = async () => {
    try {
      const accounts = await ethereum.request({ method: 'eth_requestAccounts' });
      setProvider(ethereum);
      console.log('Connected account:', accounts[0]);
      return ethereum; // Return Ethereum provider instance
    } catch (error) {
      console.error('Connection error:', error);
      throw error; // Handle error appropriately in your UI or logic
    }
  };

  return {
    connectWallet
  };
};
