using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    private Dictionary<TKey, TValue> dict;

    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public SerializableDictionary(Dictionary<TKey, TValue> dict)
    {
        this.dict = dict;
    }

    public Dictionary<TKey, TValue> GetDictionary()
    {
        return dict;
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in dict)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dict.Clear();

        if (keys.Count != values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

        for (int i = 0; i < keys.Count; i++)
            dict.Add(keys[i], values[i]);
    }

    public static implicit operator SerializableDictionary<TKey, TValue>(Dictionary<TKey, TValue> rValue)
    {
        return new SerializableDictionary<TKey, TValue>(rValue);
    }

    public static implicit operator Dictionary<TKey, TValue>(SerializableDictionary<TKey, TValue> rValue)
    {
        return rValue.GetDictionary();
    }
}
