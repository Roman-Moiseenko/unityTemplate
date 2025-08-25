namespace Game.State.Inventory
{
    internal static class TypeEpicCardMethods
    {
        public static string GetString(this TypeEpicCard type)
        {
            return type switch
            {
                TypeEpicCard.Normal => "обычная",
                TypeEpicCard.Good => "хорошая",
                TypeEpicCard.Rare => "раритетная",
                TypeEpicCard.Epic => "эпическая",
                TypeEpicCard.EpicPlus => "эпическая +",
                TypeEpicCard.Legend => "легендарная",
                _ => ""
            };
        }
    }
}