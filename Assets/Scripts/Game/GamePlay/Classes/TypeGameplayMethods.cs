using System.Diagnostics;

namespace Game.GamePlay.Classes
{
    internal static class TypeGameplayMethods
    {
        public static string GetString(this TypeGameplay typeGameplay)
        {
            return typeGameplay switch
            {
                TypeGameplay.Infinity => "Бесконечный режим",
                TypeGameplay.Levels => "Уровневый режим",
                TypeGameplay.Event => "Событие",
                _ => ""
            };
        }
    }
}