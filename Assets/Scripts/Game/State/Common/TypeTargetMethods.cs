namespace Game.State.Common
{
    internal static class TypeTargetMethods
    {

        public static bool IsTarget(this TypeTarget type, TypeTarget target)
        {
            if (type == TypeTarget.Universal) return true;
            return type == target;
        }

        public static bool IsTarget(this TypeTarget type, bool isFly)
        {
            if (type == TypeTarget.Universal) return true;
            if (type == TypeTarget.Air && isFly) return true;
            if (type == TypeTarget.Ground && !isFly) return true;
            return false;
        }
    }
}