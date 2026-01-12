namespace Game.Common
{
    public static class MyFunc
    {
        
        public static string CurrencyToStr(long value)
        {
            float t;
            switch (value)
            {
                case >= 1000000000:
                    t = (int)(value / 10000000) / 100f;
                    return $"{t}B";
                case >= 1000000:
                    t = (int)(value / 10000) / 100f;
                    return $"{t}M";
                case >= 1000:
                    t = (int)(value / 10) / 100f;
                    return $"{t}K";
                default:
                    return value.ToString();
            }
        }
    }
}