using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SelectPlaces : MonoBehaviour
{
    private static bool _isCompleteDownloadDetails;

    private static readonly string[] Places = { "東京", "名古屋", "大阪", "札幌", "福岡" };
    private static readonly string[] Icon = { "youtube_img", "twitter_main_img" };

    public Action<List<string>, bool, int> onGameStart;

    private void Start()
    {
        //テクスチャーを一気にダウンロードする
        GetMemberDetails();

        //チェックボックス
        var select = GameObject.Find("Select");
        var toggles = new List<Toggle>(select.transform.childCount);
        for (var i = 0; i < select.transform.childCount; i++)
        {
            var target = select.transform.GetChild(i);
            toggles.Add(target.GetChild(1).GetComponent<Toggle>());
        }

        //始めるボタン
        var startButton = GameObject.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(TryStart);

        async void TryStart()
        {
            startButton.interactable = false;
            var status = GameObject.Find("Bye").transform.GetChild(1).GetComponent<Toggle>();
            var result = await StartGame(toggles, status.isOn);
            if (result == false)
            {
                startButton.interactable = true;
            }
        }
    }

    private async UniTask<bool> StartGame(IReadOnlyList<Toggle> toggles, bool status)
    {
        //どれかがTrueになってないとだめなので
        var anyPlace = toggles.Any(x => x.isOn);
        if (anyPlace || status)
        {
            //選択されている場所だけのリストを作成
            Assert.AreEqual(toggles.Count, Places.Length);
            var selectedCount = toggles.Count(x => x.isOn);

            var places = new List<string>();
            //選択されてる数と、場所全体の数が等しいかどうか
            if (selectedCount == Places.Length)
            {
                //選択されてる数と、場所全部の数が等しいので、値をコピーするだけでいい
                places = Places.ToList();
            }
            else
            {
                //選択されてる数と、場所全部の数がひとしくないので、
                //ToggleがOnになってるところだけ追加する
                places.AddRange(Places.Where((t, i) => toggles[i].isOn));
            }

            await UniTask.WaitUntil(() => _isCompleteDownloadDetails);

            
            var iconDropdown = GameObject.Find("IconSelect").GetComponent<Dropdown>();
            onGameStart?.Invoke(places, status, iconDropdown.value);
            
            GameObject.Find("SelectCanvas").SetActive(false);
            return true;
        }

        Debug.LogError("Failed");
        return false;
    }

    private static async void GetMemberDetails()
    {
        //すでにダウンロードが完了されている
        if (_isCompleteDownloadDetails)
        {
            return;
        }

        if (Storage.MemberIcons == null)
        {
            Storage.MemberIcons = new List<MemberIcon>();
        }

        var text = await WebHelper.GetText(DataUrl.MemberProfile);
        var json = JsonNode.Parse(text);

        var textures = new List<Texture2D>();

        foreach (var member in json)
        {
            foreach (var icon in Icon)
            {
                var iconImgUrl = member[icon].Get<string>();
                if (string.IsNullOrEmpty(iconImgUrl) || string.IsNullOrWhiteSpace(iconImgUrl))
                {
                    iconImgUrl = DataUrl.DefaultIcon;
                }

                var texture = await WebHelper.GetTexture(iconImgUrl);

                //TextureがNullならデフォルトのテクスチャを取得する
                if (texture == null)
                {
                    texture = await WebHelper.GetTexture(DataUrl.DefaultIcon);
                }
                
                textures.Add(texture);
            }
            
            Storage.MemberIcons.Add(new MemberIcon(
                new Member(member["display_name"].Get<string>(), member["belongs"].Get<string>(),
                    member["status"].Get<string>() == "現役"), new List<Texture2D>(textures)));
            Debug.Log(textures.Count.ToString());
            
            textures.Clear();
        }

        _isCompleteDownloadDetails = true;
    }
}