using System;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class SerializableDictionary<T1, T2>
{
    private Dictionary<T1, T2> value;

    public SerializableDictionary(Dictionary<T1, T2> value)
    {
        this.value = value;
    }

    public override string ToString()
    {
        Type t1 = typeof(T1);
        Type t2 = typeof(T2);

        if (t1 != typeof(String) || !t2.IsPrimitive)
        {
            return "{}";
        }

        StringBuilder json = new StringBuilder();

        int itemCount = value.Count;
        foreach (KeyValuePair<T1, T2> pair in value)
        {
            itemCount--;
            json.Append("\"" + pair.Key + "\": ");

            if (pair.Value is bool) {
                bool parsed;
                bool.TryParse(pair.Value.ToString(), out parsed);
                json.Append(parsed);
            } else if (pair.Value is int) {
                int parsed;
                int.TryParse(pair.Value.ToString(), out parsed);
                json.Append(parsed);
            } else if (pair.Value is float) {
                float parsed;
                float.TryParse(pair.Value.ToString(), out parsed);
                json.Append(parsed);
            } else if (pair.Value is double) {
                double parsed;
                double.TryParse(pair.Value.ToString(), out parsed);
                json.Append(parsed);
            } else if (pair.Value is string) {
                json.Append("\"" + pair.Value + "\": ");
            }

            if (itemCount > 0) {
                json.Append(",");
            }
            json.AppendLine();
        }

        json.Insert(0, "{\n");
        json.Append("}");

        return json.ToString();
    }
}
