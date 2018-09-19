using System.Collections.Generic;

[System.Serializable]
public class DictionaryOfStringAndFloat : SerializableDictionary<string, float>
{
    public DictionaryOfStringAndFloat() : base(new Dictionary<string, float>())
    {
    }

    public DictionaryOfStringAndFloat(Dictionary<string, float> dict) : base(dict)
    {
    }
}