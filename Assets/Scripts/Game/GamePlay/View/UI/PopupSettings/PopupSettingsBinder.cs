using Game.GameRoot.View.Input;
using MVVM.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay.View.UI.PopupSettings
{
    public class PopupSettingsBinder : PopupBinder<PopupSettingsViewModel>
    {
        [SerializeField] private Button btnCommunity;
        [SerializeField] private Button btnSupport;

        [SerializeField] private SwitchBinder vibrator;
        [SerializeField] private SwitchBinder damage;
        [SerializeField] private SwitchBinder sound;
        [SerializeField] private SwitchBinder music;
        protected override void OnBind(PopupSettingsViewModel viewModel)
        {
            base.OnBind(viewModel);
            //TODO Передать данные из настроек 
            //viewModel.Container.Resolve<>()
            
            vibrator.Bind(true);
            damage.Bind(true);
            sound.Bind(false);
            music.Bind(true);
            vibrator.Value.Subscribe(x =>
            {

            });
        }
    }
}