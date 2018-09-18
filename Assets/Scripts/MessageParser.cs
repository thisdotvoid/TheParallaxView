using System;
using UnityEngine;

public static class MessageParser
{

    public static String Serialize(String action, Vector3 pos, Quaternion rot)
    {
        return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", action, pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, rot.w);
    }

    public static String[] Parse(String data)
    {
        return data.Split('|');
    }

}