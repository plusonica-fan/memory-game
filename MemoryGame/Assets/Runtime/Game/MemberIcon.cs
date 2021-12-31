using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MemberIcon
{
    public Member member;
    public List<Texture2D> textures;

    public MemberIcon(Member member, List<Texture2D> textures)
    {
        this.member = member;
        this.textures = textures;
    }
}