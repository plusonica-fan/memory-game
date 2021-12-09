using System;

[Serializable]
public struct Member
{
    public string name;
    public string belongs;
    public bool status;
    
    public Member(string name, string belongs, bool status)
    {
        this.name = name;
        this.belongs = belongs;
        this.status = status;
    }
}