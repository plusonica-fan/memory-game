using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<Card> cards;
    [SerializeField] private Clear clear;

    //カード生成が終わっているか
    private bool _isEndPrepareCard;

    private Card _firstSelectCard;
    private Text _timeText;
    private float _time;
    private bool _isClearGame;

    //２枚めくった後に元に戻るアニメーションが完了しているかどうか
    private bool _isCardsToDefault = true;

    private const float CardTurnTime = 0.5f;

    private static readonly Vector2 CardDiff = new Vector2(0.5f, 0.8f);
    private static readonly Vector2Int FieldSize = new Vector2Int(20, 6);
    private static readonly Vector2 FieldSpace = new Vector2(9.5f, 4);
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static Camera _camera;
    private static int _cardAmount;

    private void Start()
    {
        _camera = Camera.main;
        var selectPlaces = FindObjectOfType<SelectPlaces>();
        selectPlaces.OnGameStart = CreateCard;
        _timeText = GameObject.Find("TimeText").GetComponent<Text>();
    }

    private void Update()
    {
        CheckInput();
    }

    #region カード作成

    private void CreateCard(IReadOnlyCollection<string> places, bool status)
    {
        var addMember = Storage.MemberIcons.Where(x => places.Any(y => y == x.member.belongs)).ToList();
        
        //卒業メンバーを含めないなら
        if (status == false)
        {
            //卒業してないメンバーだけを選ぶ
            addMember = addMember.Where(x => x.member.status).ToList();
        }

        _cardAmount = addMember.Count;
        
        var createCards = new List<MemberIcon>(_cardAmount * 2);
        //ペア分必要なので、二回代入する
        createCards.AddRange(addMember);
        createCards.AddRange(addMember);

        createCards.OrderBy(x => Guid.NewGuid())
            .Select((x, i) => (Icon: x, i))
            .ToList()
            .ForEach(x =>
            {
                var cardObj = Instantiate(cardPrefab);
                var (icon, index) = x;
                //メンバー情報をコピー
                var card = cardObj.GetComponent<Card>();
                card.member = x.Icon.member;
                cards.Add(card);
                //テクスチャや座標、回転の設定
                cardObj.transform.GetChild(0)
                    .GetChild(0)
                    .GetComponent<MeshRenderer>()
                    .material.SetTexture(MainTex, icon.texture);
                cardObj.transform.GetChild(0).GetChild(3).GetComponent<TextMesh>().text = card.member.name;

                cardObj.transform.localPosition = GetPosition(index);
                cardObj.transform.localRotation = Quaternion.Euler(new Vector3(270, 0, 0));
            });
        _isEndPrepareCard = true;
    }

    private static Vector3 GetPosition(int current)
    {
        var height = Mathf.FloorToInt((float) current / FieldSize.x);
        var x = current % FieldSize.x * CardDiff.x;
        var z = height * CardDiff.y * -1 - 0.4f;
        var diff = new Vector3(x, 0, z);
        return new Vector3(FieldSpace.x * -1 / 2, 0, FieldSpace.y / 2) + diff;
    }

    #endregion

    #region カード管理

    private void CheckInput()
    {
        if (_isEndPrepareCard == false) return;

        if (_isClearGame == false)
        {
            _time += Time.deltaTime;
            _timeText.text = FormatTime((int)_time);
        }

        if (Input.GetMouseButtonDown(0) && _isCardsToDefault)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var obj = hit.collider.gameObject;

                var card = obj.GetComponent<Card>();
                if (card)
                {
                    //同じカードはだめ
                    if (_firstSelectCard && card.GetHashCode() == _firstSelectCard.GetHashCode())
                    {
                        return;
                    }

                    card.transform.DORotate(new Vector3(90, 0, 0), CardTurnTime);
                    if (_firstSelectCard)
                    {
                        var oldCard = _firstSelectCard;
                        var newCard = card;
                        var isSame = oldCard.member.name == newCard.member.name;
                        _isCardsToDefault = false;

                        var sequence = DOTween.Sequence();
                        sequence.AppendInterval(CardTurnTime);
                        
                        //同じカードかどうか
                        if (isSame)
                        {
                            //同じカードなので、リストからカードを消す
                            cards.Remove(oldCard);
                            cards.Remove(newCard);
                            
                            sequence.Append(oldCard.transform.DOScale(Vector3.zero, CardTurnTime));
                            sequence.Append(newCard.transform.DOScale(Vector3.zero, CardTurnTime));
                        }
                        else
                        {
                            sequence.Append(oldCard.transform.DORotate(new Vector3(-90, 0, 0), CardTurnTime));
                            sequence.Append(newCard.transform.DORotate(new Vector3(-90, 0, 0), CardTurnTime));
                        }
                        
                        sequence.SetDelay(CardTurnTime).OnComplete(() =>
                        {
                            _isCardsToDefault = true;
                            //カードが0枚なのでクリアにする
                            if (cards.Count < 1)
                            {
                                _isClearGame = true;
                                clear.GameClear((int)_time, _cardAmount);
                            }
                        });
                        sequence.Play();

                        _firstSelectCard = null;
                    }
                    else
                    {
                        _firstSelectCard = card;
                    }
                }
            }
        }
    }

    public static (int, int) SecondToTime(int time)
    {
        return (Mathf.FloorToInt(time / 60f), time % 60);
    }

    public static string FormatTime(int time)
    {
        var (minute, second) = SecondToTime(time);
        return $"{minute:00} : {second:00}";
    }

    #endregion
}