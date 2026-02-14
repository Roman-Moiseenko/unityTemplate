namespace Game.State.Maps.Towers
{
    internal static class TowerTypeEnemyMethods
    {

        public static bool IsTarget(this TowerTypeEnemy type, TowerTypeEnemy target)
        {
            if (type == TowerTypeEnemy.Universal) return true;
            return type == target;
        }

        public static bool IsTarget(this TowerTypeEnemy type, bool isFly)
        {
            if (type == TowerTypeEnemy.Universal) return true;
            if (type == TowerTypeEnemy.Air && isFly) return true;
            if (type == TowerTypeEnemy.Ground && !isFly) return true;
            return false;
        }
    }
}