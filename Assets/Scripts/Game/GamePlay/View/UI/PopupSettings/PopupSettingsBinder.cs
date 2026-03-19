using System;
using Cysharp.Threading.Tasks;
using Game.GameRoot.Commands;
using Game.GameRoot.View.Input;
using Game.State;
using MVVM.CMD;
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
        private IDisposable _disposable;
        protected override void OnBind(PopupSettingsViewModel viewModel)
        {
            base.OnBind(viewModel);
            var d = Disposable.CreateBuilder();

            //TODO Передать данные из настроек 
            //viewModel.Container.Resolve<>()
            var settings = viewModel.Container.Resolve<IGameStateProvider>().SettingsState;
            var cmd = viewModel.Container.Resolve<ICommandProcessor>();
            var command = new CommandSaveGameState();
            
            vibrator.Bind(settings.Vibration.CurrentValue);
            damage.Bind(settings.Damage.CurrentValue);
            sound.Bind(settings.Sound.CurrentValue);
            music.Bind(settings.Music.CurrentValue);
            
            vibrator.Value.Skip(1).Subscribe(v =>
            {
                settings.Vibration.Value = v;
                cmd.Process(command);
            }).AddTo(ref d);
            damage.Value.Skip(1).Subscribe(v =>
            {
                settings.Damage.Value = v;
                cmd.Process(command);
            }).AddTo(ref d);
            sound.Value.Skip(1).Subscribe(v =>
            {
                settings.Sound.Value = v;
                cmd.Process(command);
            }).AddTo(ref d);
            music.Value.Skip(1).Subscribe(v =>
            {
                settings.Music.Value = v;
                cmd.Process(command);
            }).AddTo(ref d);
            
            _disposable = d.Build();
        }
        
        private void OnEnable()
        {
            btnSupport.onClick.AddListener(OnSupportClick);
            btnCommunity.onClick.AddListener(OnCommunityClick);
        }

        private void OnCommunityClick()
        {
            ViewModel.RequestCommunity();
        }

        private void OnSupportClick()
        {
            ViewModel.RequestSupport();
        }

        private void OnDisable()
        {
            btnSupport.onClick.RemoveListener(OnSupportClick);
            btnCommunity.onClick.RemoveListener(OnCommunityClick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _disposable?.Dispose();
        }
    }
}