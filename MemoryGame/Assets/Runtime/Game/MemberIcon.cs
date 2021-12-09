using System;
using UnityEngine;

[Serializable]
public struct MemberIcon
{
    public Member member;
    public Texture2D texture;

    public MemberIcon(Member member, Texture2D texture)
    {
        this.member = member;
        this.texture = texture;
    }
}