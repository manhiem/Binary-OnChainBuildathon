import { useEffect, useState } from 'react';
import { Web3Provider } from '@ethersproject/providers';
import { CoinbaseWalletSDK } from '@coinbase/wallet-sdk';

const APP_NAME = "YourAppName";
const APP_LOGO_URL = "https://example.com/logo.png";
const DEFAULT_ETH_JSONRPC_URL = "https://sepolia.base.org";
const DEFAULT_CHAIN_ID = 84532; // Base

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
      const accounts = await ethereum.send('eth_requestAccounts');
      setProvider(ethereum);
      console.log('Connected account:', accounts[0]);
    } catch (error) {
      console.error('Connection error:', error);
    }
  };

  return {
    connectWallet
  };
};
