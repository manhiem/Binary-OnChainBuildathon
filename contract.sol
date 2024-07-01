// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC721/extensions/ERC721URIStorage.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/Counters.sol";

contract Game is ERC721URIStorage, Ownable {
    using Counters for Counters.Counter;

    Counters.Counter private _tokenIdCounter;

    struct UserProfile {
        string nickname;
        uint256 raceScore;
        uint256 mazeScore;
        string fusionId;
    }

    mapping(address => UserProfile) public profiles;
    mapping(address => uint256) public userRefuels;
    mapping(address => uint256) public userNFTs;

    // Leaderboard structure
    struct LeaderboardEntry {
        address player;
        uint256 score;
    }

    // Arrays for leaderboards
    LeaderboardEntry[] public raceLeaderboard;
    LeaderboardEntry[] public mazeLeaderboard;

    // Events
    event ProfileCreated(address indexed user, uint256 indexed tokenId, string uri);
    event CarSwapped(address indexed user, string oldCarUri, string newCarUri);
    event NFTMinted(address indexed user, uint256 tokenId, string uri);
    event Refueled(address indexed user);

    uint256 public refuelCost = 0.01 ether; // Define the cost for refuel after 5 free refuels

    constructor() ERC721("GameNFT", "GNFT") {}

    // Function to update leaderboard
    function _updateLeaderboard(LeaderboardEntry[] storage leaderboard, address player, uint256 score) internal {
        // Check if the player already exists in the leaderboard
        bool playerExists = false;
        for (uint256 i = 0; i < leaderboard.length; i++) {
            if (leaderboard[i].player == player) {
                leaderboard[i].score = score;
                playerExists = true;
                break;
            }
        }

        // If player does not exist in the leaderboard, add them
        if (!playerExists) {
            leaderboard.push(LeaderboardEntry(player, score));
        }

        // Sort the leaderboard by score in descending order
        for (uint256 i = 0; i < leaderboard.length - 1; i++) {
            for (uint256 j = i + 1; j < leaderboard.length; j++) {
                if (leaderboard[j].score > leaderboard[i].score) {
                    LeaderboardEntry memory temp = leaderboard[i];
                    leaderboard[i] = leaderboard[j];
                    leaderboard[j] = temp;
                }
            }
        }

        // Trim the leaderboard to top 10 entries
        if (leaderboard.length > 10) {
            leaderboard.pop();
        }
    }

    // Function to create a profile and mint an NFT
    function createProfile(string memory nickname, string memory fusionId, string memory uri) external {
        require(bytes(profiles[msg.sender].nickname).length == 0, "Profile already exists");

        _tokenIdCounter.increment();
        uint256 newTokenId = _tokenIdCounter.current();
        _mint(msg.sender, newTokenId);
        _setTokenURI(newTokenId, uri);

        profiles[msg.sender] = UserProfile(nickname, 0, 0, fusionId);
        userNFTs[msg.sender] = newTokenId;

        emit ProfileCreated(msg.sender, newTokenId, uri);
    }

    // Function to update a user profile
    function updateProfile(uint256 raceScore, uint256 mazeScore, string memory nickname, string memory fusionId) external {
        require(bytes(profiles[msg.sender].nickname).length != 0, "Profile does not exist");

        profiles[msg.sender].raceScore = raceScore;
        profiles[msg.sender].mazeScore = mazeScore;
        profiles[msg.sender].nickname = nickname;
        profiles[msg.sender].fusionId = fusionId;

        _updateLeaderboard(raceLeaderboard, msg.sender, raceScore);
        _updateLeaderboard(mazeLeaderboard, msg.sender, mazeScore);
    }

    // Function to refuel
    function refuel() external payable {
        if (userRefuels[msg.sender] < 5) {
            // Free refuel
            userRefuels[msg.sender]++;
            emit Refueled(msg.sender);
        } else {
            // Paid refuel
            require(msg.value >= refuelCost, "Insufficient ETH for refuel");
            userRefuels[msg.sender]++;
            emit Refueled(msg.sender);
        }
    }

    // Function to swap cars (NFTs) by new URI
    function swapCar(string memory newCarUri) external {
        uint256 oldCarId = userNFTs[msg.sender];
        require(oldCarId != 0, "Old car not found");
        require(ownerOf(oldCarId) == msg.sender, "Only owner can swap car");

        string memory oldCarUri = tokenURI(oldCarId);

        // Burn old car token
        _burn(oldCarId);

        // Mint new car token
        _tokenIdCounter.increment();
        uint256 newCarId = _tokenIdCounter.current();
        _mint(msg.sender, newCarId);
        _setTokenURI(newCarId, newCarUri);

        // Set the new car as the current NFT for the user
        userNFTs[msg.sender] = newCarId;

        emit CarSwapped(msg.sender, oldCarUri, newCarUri);
    }

    // Helper function to get tokenId by URI
    function _getTokenIdByUri(string memory uri) internal view returns (uint256) {
        uint256 totalSupply = _tokenIdCounter.current();
        for (uint256 i = 1; i <= totalSupply; i++) {
            if (keccak256(abi.encodePacked(tokenURI(i))) == keccak256(abi.encodePacked(uri))) {
                return i;
            }
        }
        return 0; // Token with the given URI not found
    }

    // Function to mint an NFT
    function mintNFT(address toAddress, string memory uri) external onlyOwner {
        _tokenIdCounter.increment();
        uint256 tokenId = _tokenIdCounter.current();
        _mint(toAddress, tokenId);
        _setTokenURI(tokenId, uri);

        emit NFTMinted(toAddress, tokenId, uri);
    }

    // Function to get the top players for the race game
    function getTopPlayersRace() external view returns (address[] memory, uint256[] memory, string[] memory) {
        uint256 leaderboardLength = raceLeaderboard.length < 10 ? raceLeaderboard.length : 10;
        address[] memory addresses = new address[](leaderboardLength);
        uint256[] memory scores = new uint256[](leaderboardLength);
        string[] memory nicknames = new string[](leaderboardLength);

        for (uint256 i = 0; i < leaderboardLength; i++) {
            addresses[i] = raceLeaderboard[i].player;
            scores[i] = raceLeaderboard[i].score;
            nicknames[i] = profiles[raceLeaderboard[i].player].nickname;
        }

        return (addresses, scores, nicknames);
    }

    // Function to get the top players for the maze game
    function getTopPlayersMaze() external view returns (address[] memory, uint256[] memory, string[] memory) {
        uint256 leaderboardLength = mazeLeaderboard.length < 10 ? mazeLeaderboard.length : 10;
        address[] memory addresses = new address[](leaderboardLength);
        uint256[] memory scores = new uint256[](leaderboardLength);
        string[] memory nicknames = new string[](leaderboardLength);

        for (uint256 i = 0; i < leaderboardLength; i++) {
            addresses[i] = mazeLeaderboard[i].player;
            scores[i] = mazeLeaderboard[i].score;
            nicknames[i] = profiles[mazeLeaderboard[i].player].nickname;
        }

        return (addresses, scores, nicknames);
    }

    // Function to get user achievements
    function getUserAchievements(address user) external view returns (uint256, uint256, string memory, string memory) {
        return (
            profiles[user].raceScore, 
            profiles[user].mazeScore, 
            profiles[user].nickname,
            profiles[user].fusionId
        );
    }
}
