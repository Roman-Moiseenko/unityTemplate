namespace Game.Common
{
    public static class AppConstants
    {
       // public const string EXIT_SCENE_REQUEST_TAG = nameof(EXIT_SCENE_REQUEST_TAG);
        public const string GAME_PLAY_STATE = nameof(GAME_PLAY_STATE);
        public const string CAMERA_MOVING = nameof(CAMERA_MOVING);
        
        public const string USER_ID = nameof(USER_ID);
        public const string USER_TOKEN = nameof(USER_TOKEN);


        
        public const string CLICK_WORLD_ENTITY = nameof(CLICK_WORLD_ENTITY);
        public const int COST_UPDATE_BUILD = 20; //стоимость повышения обновления карточек за каждый раз
        public const int COST_REPAIR_CASTLE = 40; //стоимость восстановления замка
        public const int TIME_WAVE_NEW = 60; //Кол-во кадров на таймере волны
        public const float TIME_PAUSE_WAVE_NEW = 0.04f; //Время между кадрами таймера волны
        
        public const float MOB_BASE_SPEED = 0.5f;
        public const float SHOT_BASE_SPEED = 10f;
        public const float MOB_SPEED_ATTACK = 60f; //Коэф.деления, 60 = 1с
        public const float SPEED_REDICE_CASTLE = 1f;

        public const float RATIO_COST_OPEN_CHEST = 0.4f;
        
        public const string MAIN_MENU_SHOP = nameof(MAIN_MENU_SHOP);
        public const string MAIN_MENU_INVENTORY = nameof(MAIN_MENU_INVENTORY);
        public const string MAIN_MENU_PLAY = nameof(MAIN_MENU_PLAY);
        public const string MAIN_MENU_CLAN = nameof(MAIN_MENU_CLAN);
        public const string MAIN_MENU_RESEARCH = nameof(MAIN_MENU_RESEARCH);

        public const string MAIN_MENU_SCREEN = nameof(MAIN_MENU_SCREEN);


        public const string IMAGE_MANAGER = "[IMAGEMANAGER]";
        public const string COROUTINES = "[COROUTINES]";

        public const int WIDTH_MAP = 10;
        public const int HIGHT_MAP = 10;
        public const int CENTER_MAP = 2;

        public const float CAMERA_SCALE_SPEED = 0.3f;
        public const float CAMERA_SCALE_MIN = 3.5f;
        public const float CAMERA_SCALE_MAX = 7f;
    }
}