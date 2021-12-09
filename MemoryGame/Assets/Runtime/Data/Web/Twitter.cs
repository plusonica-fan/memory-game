using System;
using UnityEngine;

/// <summary>
/// ツイート機能のサンプル
/// </summary>
public static class Twitter
{
    // ツイート画面を開く
    public static void Tweet(string text, string linkUrl, string hashtags)
    {
        var url = "https://twitter.com/intent/tweet?" + "text=" + text + "&url=" + linkUrl + "&hashtags=" + hashtags;
        var uri = new Uri(url);
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGLの場合は、ゲームプレイ画面と同じウィンドウでツイート画面が開かないよう、処理を変える
            Application.ExternalEval($"window.open('{uri.AbsoluteUri}','_blank')");
        }
        else
        {
            Application.OpenURL(uri.AbsoluteUri);
        }
    }
}