namespace Game.Common
{
    public static class AppConstants
    {
       // public const string EXIT_SCENE_REQUEST_TAG = nameof(EXIT_SCENE_REQUEST_TAG);
        public const string GAME_PLAY_STATE = nameof(GAME_PLAY_STATE);
        public const string CAMERA_MOVING = nameof(CAMERA_MOVING);
        
        
        public const string CLICK_WORLD_ENTITY = nameof(CLICK_WORLD_ENTITY);
        public const int COST_UPDATE_BUILD = 20; //стоимость повышения обновления карточек за каждый раз
        public const int COST_REPAIR_CASTLE = 40; //стоимость восстановления замка
        public const int TIME_WAVE_NEW = 60;
        public const float MOB_BASE_SPEED = 0.5f;
        public const float SHOT_BASE_SPEED = 10f;
        public const float MOB_SPEED_ATTACK = 1f;
        public const float SPEED_REDICE_CASTLE = 1f;
        
        public const string MAIN_MENU_SHOP = nameof(MAIN_MENU_SHOP);
        public const string MAIN_MENU_INVENTORY = nameof(MAIN_MENU_INVENTORY);
        public const string MAIN_MENU_PLAY = nameof(MAIN_MENU_PLAY);
        public const string MAIN_MENU_CLAN = nameof(MAIN_MENU_CLAN);
        public const string MAIN_MENU_RESEARCH = nameof(MAIN_MENU_RESEARCH);

        public const string MAIN_MENU_SCREEN = nameof(MAIN_MENU_SCREEN);


        public const string IMAGE_MANAGER = "[IMAGEMANAGER]";
        public const string COROUTINES = "[COROUTINES]";
    }
}