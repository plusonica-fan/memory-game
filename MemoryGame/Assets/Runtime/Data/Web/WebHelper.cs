using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

public static class WebHelper
{
    public static async Task<Texture2D> GetTexture(string url)
    {
        var www = UnityWebRequestTexture.GetTexture(url);
        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error + " from " + url);
            return null;
        }

        var myTexture = ((DownloadHandlerTexture) www.downloadHandler).texture;
        return myTexture;
    }
    
    public static async Task<string> GetText(string url)
    {
        var www = UnityWebRequest.Get(url);
        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            return null;
        }

        return www.downloadHandler.text;
    }
}