/*
    These are internal functions of the Score Milk SDK.
    Do not use any of the functions in this file.
*/

using System;
using UnityEngine;
using Newtonsoft.Json;

using OPS.AntiCheat.Field;

namespace ScoreMilk{
    public class NetworkManager : Singleton<NetworkManager>
    {
        private ProtectedString matchId = string.Empty;
        private ProtectedString userId = string.Empty;

        // Initialization
        protected override void StartUp()
        {
            EmitGameLoaded();
        }
        static void EmitGameLoaded()
        {
            Application.ExternalCall("gameLoaded");
        }
        static void EnableFullScreen()
        {
            Application.ExternalCall("enableFullScreen");
        }
        private void DisableFullScreen(){
            Application.ExternalCall("disableFullScreen");
        }
        
        void setUrl(string jsonData)
        {
            Envs envs = JsonConvert.DeserializeObject<Envs>(jsonData);
            WebConnection.Instance.SetUrl(envs.API_URL);
        }
        /// <summary>
        /// Called when wallet connects 
        /// NOT necessary to call in Unity
        /// </summary>
        public void walletConnected(string jsonData)
        {
            WalletConnectedData data = JsonConvert.DeserializeObject<WalletConnectedData>(jsonData);
            ScoreMilk.Connection.Instance.walletConnectedCall(data.walletAddress);
        }

        /// <summary>
        /// Called when wallet disconnects
        /// NOT necessary to call in Unity
        /// </summary>
        public void walletDisconnected()
        {
            ScoreMilk.Connection.Instance.walletDisconnectedCall();
        }
        /// <summary>
        /// Received message that indicates game should go to practice mode
        /// NOT necessary to call. Use event instead.
        /// </summary>
        void startPracticeGame(){
            Connection.Instance.startPracticeGameCall();
        }
        /// <summary>
        /// Received before a real match starts
        /// NOT necessary to call in Unity
        /// </summary>
        void getReady(string json){
            GetReadyData data = new GetReadyData();
            try
            {
                data = JsonUtility.FromJson<GetReadyData>(json);
                NetworkManager.Instance.matchId = data.matchId;
                NetworkManager.Instance.userId = data.userId;
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            ScoreMilk.Connection.Instance.getReadyCall(data);
        }
        /// <summary>
        /// Received message that indicates game can properly start
        /// NOT necessary to call. Use event instead.
        /// </summary>
        void startRealGame() 
        {
            Connection.Instance.startRealGameCall();
        }
        /// <summary>
        /// Called when match is cancelled
        /// NOT necessary to call.
        /// </summary>
        public void quitToMenu()
        {
            ScoreMilk.Connection.Instance.quitToMenuCall();
        }
        /// <summary>
        /// Emits message to server that says the match ended. Accumulated points must be the same as added points
        /// points: total points acquired during match
        /// </summary>
        public void EmitGameOver(int points)
        {
            HttpRequestData data = new HttpRequestData();
            data.match_room_id = NetworkManager.Instance.matchId;
            data.player_id = NetworkManager.Instance.userId;

            data.points = points.ToString();

            WebConnection.Instance.Emit("/matches/player-finished-game", data);
        }
        /// <summary>
        /// Emits message to server that says player got points during match
        /// </summary>
        public void EmitAddScore(int score)
        {
            HttpRequestData data = new HttpRequestData();
            data.match_room_id = NetworkManager.Instance.matchId;
            data.player_id = NetworkManager.Instance.userId;
            data.points = score.ToString();

            WebConnection.Instance.Emit("/matches/player-score-game", data);
        }
        /// <summary>
        /// Emits message to server that says player is ready to start match
        /// </summary>
        public void EmitReady()
        {
            HttpRequestData data = new HttpRequestData();
            data.match_room_id = NetworkManager.Instance.matchId;
            data.player_id = NetworkManager.Instance.userId;
            data.points = "";

            //Application.ExternalCall("socket.emit", "EMIT_READY", new JSONObject(data));
            WebConnection.Instance.Emit("/matches/player-loaded-game", data);
        }
        void OnApplicationQuit()
        {
            OnlineApplicationQuit();
        }
        void OnlineApplicationQuit()
        {
            EmitGameOver(0);
        }
    }

    [Serializable]
    public class GetReadyData {
        public string matchId;
        public string userId;
        public string name;
        public string username;
        public string avatar;
        public string bio;
    }

    public class HttpRequestData {
        public string match_room_id;
        public string player_id;
        public string points;
    }

    class WalletConnectedData {
        public string walletAddress;
    }
    
    class Envs {
        public string API_URL;
    }
}