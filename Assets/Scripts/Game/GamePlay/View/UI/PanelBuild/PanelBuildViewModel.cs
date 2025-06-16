using System;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.State.Gameplay;
using MVVM.FSM;
using MVVM.UI;
using Newtonsoft.Json;
using R3;
using UnityEngine;
using Object = System.Object;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class PanelBuildViewModel : WindowViewModel
    {
        public override string Id => "PanelBuild";
        public override string Path => "Gameplay/";


        //TODO В дальнейшем заменить картинками 
        public ReactiveProperty<string> TextButton1 = new();
        public ReactiveProperty<string> TextButton2 = new();
        public ReactiveProperty<string> TextButton3 = new();
        private FsmGameplay _fsmGameplay;

        public PanelBuildViewModel(DIContainer container)
        {
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _fsmGameplay.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmStateBuildBegin))
                {
                    var rewards = GetRewards();
                    TextButton1.Value = GetTextRewardButton(rewards.Card1);
                    TextButton2.Value = GetTextRewardButton(rewards.Card2);
                    TextButton3.Value = GetTextRewardButton(rewards.Card3);
                    
                    //    Debug.Log(" Новые награды ==== " + JsonConvert.SerializeObject(rewards, Formatting.Indented));
                    //Надо менять кнопки
                }
            });
        }

        private string GetTextRewardButton(RewardCardData cardData)
        {
            switch (cardData.RewardType)
            {
                case RewardType.Tower : return "Построить башню";
                case RewardType.Ground : return "Построить участок";
                case RewardType.Road : return "Построить дорогу";
                case RewardType.TowerBust : return "Улучшить башню";
                case RewardType.SkillBust : return "Улучшить навык";
                case RewardType.HeroBust : return "Улучшить героя";
                case RewardType.TowerMove : return "Передвинуть башню";
                case RewardType.TowerReplace : return "Заменить башни";
                default: return "Не известное значение";
            }
            
        }

        public void GenerateButton()
        {
            //TODO Сервис, который слушает состояние FsmStateBuildBegin
            //Генерирует 3 карточки
            //Здесь присваиваем сущности Entity или бафы Bust
        }

        public void OnBuild1()
        {
            var rewards = GetRewards(); //Вытаскиваем варианты наград из состояния
            BuildStateProgress(rewards.Card1); //Строим или применяем навык
        }

        public void OnBuild2()
        {
            var rewards = GetRewards(); //Вытаскиваем варианты наград из состояния
            BuildStateProgress(rewards.Card2); //Строим или применяем навык
        }

        public void OnBuild3()
        {
            var rewards = GetRewards(); //Вытаскиваем варианты наград из состояния
            BuildStateProgress(rewards.Card3); //Строим или применяем навык
        }

        private RewardsProgress GetRewards()
        {
            var currentState = _fsmGameplay.Fsm.StateCurrent.Value;
        //    Debug.Log("currentState + " + JsonConvert.SerializeObject(currentState.Params, Formatting.Indented));

            if (currentState.GetType() == typeof(FsmStateBuildBegin))
                return ((FsmStateBuildBegin)currentState).GetRewards();

            throw new Exception("Исключительная ситуация текущее состояние не FsmStateBuildBegin");
        }

        private void BuildStateProgress(RewardCardData cardData)
        {
//            Debug.Log("Отправка для применения + " + JsonConvert.SerializeObject(cardData, Formatting.Indented));
            if (cardData.IsBuild())
            {
//                Debug.Log("Строим");
                _fsmGameplay.Fsm.SetState<FsmStateBuild>(cardData);
            }
            else
            {
     //           Debug.Log("Бафаем");
                _fsmGameplay.Fsm.SetState<FsmStateBuildEnd>(cardData);
            }
        }
    }
}