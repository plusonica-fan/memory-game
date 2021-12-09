using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// ツイート機能のサンプル
/// </summary>
public static class Twitter
{
    // 新しいタブでURLを開く
#if !UNITY_EDITOR && UNITY_WEBGL
    // WebGLビルドで有効になる
    [DllImport("__Internal")]
    private static extern void OpenNewTab(string url);
#else
    // UnityエディタやWebGL以外のプラットフォームで有効になる
    private static void OpenNewTab(string url) => Application.OpenURL(url);
#endif
    
    // ツイート画面を開く
    public static void Tweet(string text, string linkUrl, string hashtags)
    {
        var url = "https://twitter.com/intent/tweet?" + "text=" + text + "&url=" + linkUrl + "&hashtags=" + hashtags;
        var uri = new Uri(url);
        // WebGLの場合は、ゲームプレイ画面と同じウィンドウでツイート画面が開かないよう、処理を変える
        OpenNewTab($"window.open('{uri.AbsoluteUri}','_blank')");
    }
}