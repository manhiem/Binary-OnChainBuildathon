import axios from 'axios';

const PINATA_API_KEY = 'e5905f3485da9fbdf700';
const PINATA_SECRET_API_KEY = '9674f209943aa4728622dea70d9b257612e6c5bebcc49e8ca44d7abe9f1bef39';

const uploadToPinata = async (base64Image) => {
  const url = `https://api.pinata.cloud/pinning/pinFileToIPFS`;
  const data = new FormData();
  const blob = await fetch(`data:image/png;base64,${base64Image}`).then(res => res.blob());

  data.append('file', blob);

  const metadata = JSON.stringify({
    name: 'nft-image',
  });

  data.append('pinataMetadata', metadata);

  const pinataOptions = JSON.stringify({
    cidVersion: 0,
  });

  data.append('pinataOptions', pinataOptions);

  try {
    const response = await axios.post(url, data, {
      maxBodyLength: 'Infinity',
      headers: {
        'Content-Type': `multipart/form-data; boundary=${data._boundary}`,
        pinata_api_key: PINATA_API_KEY,
        pinata_secret_api_key: PINATA_SECRET_API_KEY,
      },
    });
    return response.data;
  } catch (error) {
    console.error('Error uploading image to Pinata:', error);
    return null;
  }
};

const uploadMetadataToPinata = async (metadata) => {
  const url = `https://api.pinata.cloud/pinning/pinJSONToIPFS`;

  try {
    const response = await axios.post(url, metadata, {
      headers: {
        'Content-Type': 'application/json',
        pinata_api_key: PINATA_API_KEY,
        pinata_secret_api_key: PINATA_SECRET_API_KEY,
      },
    });
    return response.data;
  } catch (error) {
    console.error('Error uploading metadata to Pinata:', error);
    return null;
  }
};

export {uploadToPinata , uploadMetadataToPinata};
