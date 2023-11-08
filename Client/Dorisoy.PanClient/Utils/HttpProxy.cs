//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading.Tasks;

//using Sinol.CaptureManager.Common.ApiModels;
//using Sinol.CaptureManager.Models;

//namespace Sinol.CaptureManager.Utils;


//public static class HttpProxy
//{
//    private readonly static HttpClient HttpClient = new();
//    private readonly static JsonSerializerOptions SerializerSettings = new()
//    {
//        PropertyNameCaseInsensitive = true
//    };

//    static HttpProxy()
//    {
//        var url = Globals.ServerUri;
//        if (url.EndsWith("/"))
//            url = url.Substring(0, url.Length - 1);

//        HttpClient.BaseAddress = new Uri($"{url}/api/");
//        HttpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Accept.ToString(), "application/json");
//    }

//    /// <summary>
//    /// 加入小组验证
//    /// </summary>
//    /// <param name="room"></param>
//    /// <param name="nickname"></param>
//    /// <returns></returns>
//    public static async Task<JoinRoomValidationResult> ValidateJoinRoomAsync(string room, string nickname)
//    {
//        try
//        {
//            var url = $"rooms/{room}/validate-join/{nickname}";
//            var response = await HttpClient.GetAsync(url);
//            response.EnsureSuccessStatusCode();
//            var content = await response.Content.ReadAsStringAsync();
//            return JsonSerializer.Deserialize<JoinRoomValidationResult>(content, SerializerSettings);
//        }
//        catch (Exception ex)
//        {
//            return JsonSerializer.Deserialize<JoinRoomValidationResult>("");
//        }
//    }

//    /// <summary>
//    /// 获取参与人员
//    /// </summary>
//    /// <param name="room"></param>
//    /// <returns></returns>
//    public static async Task<List<string>> GetParticipantsAsync(string room)
//    {
//        try
//        {
//            var response = await HttpClient.GetAsync($"rooms/{room}/participants");
//            response.EnsureSuccessStatusCode();
//            var content = await response.Content.ReadAsStringAsync();
//            return JsonSerializer.Deserialize<List<string>>(content, SerializerSettings);
//        }
//        catch (Exception ex)
//        {
//            return JsonSerializer.Deserialize<List<string>>("");
//        }
//    }


//    /// <summary>
//    /// 获取小组
//    /// </summary>
//    /// <param name="room"></param>
//    /// <returns></returns>
//    public static async Task<List<Rooms>> GetRoomsAsync(Guid uid)
//    {
//        try
//        {
//            //http://localhost:5000/alls/4b352b37-332a-40c6-ab05-e38fcf109719
//            var response = await HttpClient.GetAsync($"rooms/alls/{uid}");
//            response.EnsureSuccessStatusCode();
//            var content = await response.Content.ReadAsStringAsync();
//            return JsonSerializer.Deserialize<List<Rooms>>(content, SerializerSettings);
//        }
//        catch (Exception ex)
//        {
//            return JsonSerializer.Deserialize<List<Rooms>>("");
//        }
//    }


//    /// <summary>
//    /// 删除小组
//    /// </summary>
//    /// <param name="roomid"></param>
//    /// <returns></returns>
//    public static async Task<bool> DeleteRoomAsync(int roomid)
//    {
//        try
//        {
//            var response = await HttpClient.GetAsync($"rooms/delete/{roomid}");
//            response.EnsureSuccessStatusCode();
//            var content = await response.Content.ReadAsStringAsync();
//            return true;
//        }
//        catch (Exception ex)
//        {
//            return false;
//        }
//    }
    
//    /// <summary>
//    /// 创建小组
//    /// </summary>
//    /// <param name="uid"></param>
//    /// <param name="nickname"></param>
//    /// <param name="room"></param>
//    /// <returns></returns>
//    public static async Task<bool> CreateRoomAsync(Guid uid, string nickname, string room)
//    {
//        try
//        {
//            var response = await HttpClient.PostAsync($"rooms/createroom/{uid}/{nickname}/{room}", null);
//            response.EnsureSuccessStatusCode();
//            var content = await response.Content.ReadAsStringAsync();
//            return JsonSerializer.Deserialize<bool>(content, SerializerSettings);
//        }
//        catch (Exception ex)
//        {
//            return JsonSerializer.Deserialize<bool>(false);
//        }
//    }
//}
