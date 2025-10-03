using System;
using System.Collections.Generic;
using MessagePack;
using Dorisoy.Pan.SharedLibrary.Data.Models;
using Dorisoy.Pan.SharedLibrary.Data.Requests;
using Dorisoy.Pan.SharedLibrary.Data.Responses;

namespace Dorisoy.Pan.SharedLibrary.Data;



[Serializable]
public class DataObject
{

    public enum DataObjectTypes
    {
        // Requests
        //signUpRequest,
        //loginInRequest,
        loginRequest,
        createLobbyRequest,
        joinLobbyRequest,
        leaveLobbyRequest,
        resetLobbyRequest,
        removeLobbyRequest,
        getLobbiesRequest,
        // Responses
        successResponse,
        errorResponse,
        userInfoResponse,
        lobbyInfoResponse,
        lobbiesInfoResponse,
        userJoinedToLobbyResponse,
        userLeavedFromLobbyResponse,
        // UDP Data
        newVideoFrame,
        newAudioFrame
    }

    //[Key(0)]
    public DataObjectTypes dataObjectType { get; set; }

    //[Key(1)]
    public object dataObjectInfo { get; set; }

    // Requests
    //public static DataObject signUpRequest(string userName, string password)
    //{
    //    var result = new DataObject
    //    {
    //        dataObjectType = DataObjectTypes.signUpRequest,
    //        dataObjectInfo = new SignUp(userName, password)
    //    };
    //    return result;
    //}

    //public static DataObject loginInRequest(string userName, string password)
    //{
    //    var result = new DataObject
    //    {
    //        dataObjectType = DataObjectTypes.loginInRequest,
    //        dataObjectInfo = new LoginIn(userName, password)
    //    };
    //    return result;
    //}

    public static DataObject loginRequest(string userName, string id)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.loginRequest,
            dataObjectInfo = new UserModel() 
            { 
                UserName = userName, 
                Id = id 
            }
        };
        return result;
    }

    public static DataObject createLobbyRequest(string rid, string lobbyName, int lobbyCapacity, string lobbyPassword)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.createLobbyRequest,
            dataObjectInfo = new CreateLobby(rid, lobbyName, lobbyCapacity, lobbyPassword)
        };
        return result;
    }

    public static DataObject joinLobbyRequest(string lobbyId, string lobbyPassword)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.joinLobbyRequest,
            dataObjectInfo = new JoinLobby(lobbyId, lobbyPassword)
        };
        return result;
    }

    public static DataObject removeLobbyRequest(string lobbyId)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.removeLobbyRequest,
            dataObjectInfo = lobbyId
        };
        return result;
    }

    public static DataObject leaveLobbyRequest()
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.leaveLobbyRequest,
            dataObjectInfo = null
        };
        return result;
    }

    public static DataObject resetLobbyRequest()
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.resetLobbyRequest,
            dataObjectInfo = null
        };
        return result;
    }

    public static DataObject getLobbiesRequest()
    {
        DataObject result = new DataObject
        {
            dataObjectType = DataObjectTypes.getLobbiesRequest,
            dataObjectInfo = null
        };
        return result;
    }

    // Responses
    public static DataObject successResponse(string successString)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.successResponse,
            dataObjectInfo = successString
        };
        return result;
    }
    public static DataObject errorResponse(string errorString)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.errorResponse,
            dataObjectInfo = errorString
        };
        return result;
    }
    public static DataObject userInfoResponse(string userJson)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.userInfoResponse,
            dataObjectInfo = new UserInfo(userJson)
        };
        return result;
    }
    public static DataObject lobbyInfoResponse(string lobbyJson)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.lobbyInfoResponse,
            dataObjectInfo = new LobbyInfo(lobbyJson)
        };
        return result;
    }
    public static DataObject lobbiesInfoResponse(string lobbiesJson)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.lobbiesInfoResponse,
            dataObjectInfo = new LobbiesInfo(lobbiesJson)
        };
        return result;
    }
    public static DataObject userJoinedToLobbyResponse(string userJson)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.userJoinedToLobbyResponse,
            dataObjectInfo = new UserJoinedToLobby(userJson)
        };
        return result;
    }
    public static DataObject userLeavedFromLobbyResponse(string userJson)
    {
        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.userLeavedFromLobbyResponse,
            dataObjectInfo = new UserLeavedFromLobby(userJson)
        };
        return result;
    }

    // UDP Data

    public static DataObject newVideoFrame(XMessage xm, XXMessageEND eNDxm, string userId)
    {
        var videoFrame = new VideoFrameModel
        {
            UserId = userId,
            XM = xm,
            EndXM = eNDxm
        };

        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.newVideoFrame,
            dataObjectInfo = videoFrame
        };

        return result;
    }


    public static DataObject newAudioFrame(byte[] bytes, string userId)
    {
        var audioFrame = new AudioFrameModel
        {
            UserId = userId,
            Bytes = bytes
        };

        var result = new DataObject
        {
            dataObjectType = DataObjectTypes.newAudioFrame,
            dataObjectInfo = audioFrame
        };

        return result;
    }
}



