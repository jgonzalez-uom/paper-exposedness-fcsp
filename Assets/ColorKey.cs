using UnityEngine;

[System.Serializable]
public class ColorKey
{
    public byte r;
    public byte g;
    public byte b;

    public Color32 color
    {
        get
        {
            return new Color32(r, g, b, 1);
        }
    }

    public void Set(Color32 value)
    {
        this.r = value.r;
        this.g = value.g;
        this.b = value.b;
    }

    public ColorKey(byte r, byte g, byte b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public ColorKey(string value)
    {
        string[] parts = value.Split(',');

        if (parts.Length < 3)
        {
            Debug.Log("Wrong format, not enough elements in " + value);
        }

        if (int.TryParse(parts[0], out int r) && int.TryParse(parts[1], out int g) && int.TryParse(parts[2], out int b))
        {
            this.r = (byte)r;
            this.g = (byte)g;
            this.b = (byte)b;
        }
    }

    public ColorKey(Color32 value)
    {
        this.r = value.r;
        this.g = value.g;
        this.b = value.b;
    }

    public ColorKey()
    {
    }

    public static bool operator ==(ColorKey left, ColorKey right)
    {
        if (left.r == right.r && left.g == right.g && left.b == right.b)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(ColorKey left, ColorKey right)
    {
        if (left.r == right.r && left.g == right.g && left.b == right.b)
        {
            return false;
        }

        return true;
    }

    public static bool operator ==(ColorKey left, Color32 right)
    {
        if (left.r == right.r && left.g == right.g && left.b == right.b)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(ColorKey left, Color32 right)
    {
        if (left.r == right.r && left.g == right.g && left.b == right.b)
        {
            return false;
        }

        return true;
    }

    public static implicit operator string(ColorKey a)
    {
        return string.Format("{0},{1},{2}", a.r, a.g, a.b);
    }
}
