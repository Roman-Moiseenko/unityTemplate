using System;
using DI;
using Game.GamePlay.Fsm;
using Game.GamePlay.Fsm.States;
using Game.State.Gameplay;
using MVVM.FSM;
using MVVM.UI;
using Newtonsoft.Json;
using ObservableCollections;
using R3;
using UnityEngine;
using Object = System.Object;

namespace Game.GamePlay.View.UI.PanelBuild
{
    public class PanelBuildViewModel : WindowViewModel
    {
        public override string Id => "PanelBuild";
        public override string Path => "Gameplay/Panels/";


        //TODO В дальнейшем заменить картинками 
        public ReactiveProperty<string> TextButton1 = new();
        public ReactiveProperty<string> TextButton2 = new();
        public ReactiveProperty<string> TextButton3 = new();
        
        public ObservableDictionary<int, ButtonData> ButtonCards = new();
        private FsmGameplay _fsmGameplay;

        public PanelBuildViewModel(DIContainer container)
        {
            _fsmGameplay = container.Resolve<FsmGameplay>();
            _fsmGameplay.Fsm.StateCurrent.Subscribe(newState =>
            {
                if (newState.GetType() == typeof(FsmStateBuildBegin))
                {
                    ButtonCards.Clear();
                    var rewards = GetRewards();
                    //Debug.Log(JsonConvert.SerializeObject(rewards, Formatting.Indented));
                    foreach (var rewardsCard in rewards.Cards)
                    {
                        ButtonCards.Add(rewardsCard.Key, GetTextRewardButton(rewardsCard.Value));
                    }
                    /*TextButton1.Value = GetTextRewardButton(rewards.Cards[1]);
                    TextButton2.Value = GetTextRewardButton(rewards.Cards[2]);
                    TextButton3.Value = GetTextRewardButton(rewards.Cards[3]); */
                    
                    //    Debug.Log(" Новые награды ==== " + JsonConvert.SerializeObject(rewards, Formatting.Indented));
                    //Надо менять кнопки
                }
            });
        }

        private ButtonData GetTextRewardButton(RewardCardData cardData)
        {
            var buttonData = new ButtonData();
            switch (cardData.RewardType)
            {
                case RewardType.Tower : 
                    buttonData.Caption = "Построить башню";
                    buttonData.PrehabImage = "Towers/" + cardData.ConfigId + "/Level_" + cardData.RewardLevel;
                    buttonData.Level = cardData.RewardLevel.ToString();
                    buttonData.Description = "Башня " + cardData.Name; 
                    break;
                case RewardType.Ground : 
                    buttonData.Caption = "Построить участок";
                    buttonData.PrehabImage = "Ground";
                    
                    break;
                case RewardType.Road : 
                    buttonData.Caption = "Построить дорогу";
                    buttonData.PrehabImage = "Roads/" + cardData.ConfigId;
                    buttonData.Description = cardData.Description;
                    break;
                case RewardType.TowerLevelUp : 
                    buttonData.Caption = "Улучшить башню"; 
                    buttonData.PrehabImage = "Towers/" + cardData.ConfigId + "/Level_" + cardData.RewardLevel;
                    buttonData.Level = cardData.RewardLevel.ToString() + " +1";
                    buttonData.Description = cardData.Description;
                    break;
                case RewardType.SkillLevelUp : buttonData.Caption = "Улучшить навык"; break;
                case RewardType.HeroLevelUp : buttonData.Caption = "Улучшить героя"; break;
                case RewardType.TowerMove : buttonData.Caption = "Передвинуть башню"; break;
                case RewardType.TowerReplace : buttonData.Caption = "Заменить башни"; break;
                default: throw new  Exception("Не известное значение");
            }

            return buttonData;
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
            BuildStateProgress(rewards.Cards[1]); //Строим или применяем навык
        }

        public void OnBuild2()
        {
            var rewards = GetRewards(); //Вытаскиваем варианты наград из состояния
            BuildStateProgress(rewards.Cards[2]); //Строим или применяем навык
        }

        public void OnBuild3()
        {
            var rewards = GetRewards(); //Вытаскиваем варианты наград из состояния
            BuildStateProgress(rewards.Cards[3]); //Строим или применяем навык
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