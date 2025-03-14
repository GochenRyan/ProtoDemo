using Assets.GAS.Runtime.Core;
using UnityEngine;

namespace GAS.Runtime
{
    public class GameplayEffectPeriodTicker
    {
        private float _periodRemaining;
        private float _movePeriodRemaining;
        private float _turnPeriodRemaining;
        private readonly GameplayEffectSpec _spec;

        public GameplayEffectPeriodTicker(GameplayEffectSpec spec)
        {
            _spec = spec;
            _periodRemaining = Period;
            _movePeriodRemaining = MovePeriod;
            _turnPeriodRemaining = TurnPeriod;
        }

        private float Period => _spec.GameplayEffect.Period;
        private int MovePeriod => _spec.GameplayEffect.MovePeriod;
        private int TurnPeriod => _spec.GameplayEffect.TurnPeriod;

        public void Tick()
        {
            _spec.TriggerOnTick();

            UpdatePeriod();

            if (_spec.DurationPolicy == EffectsDurationPolicy.Duration && _spec.DurationRemaining() <= 0)
            {
                // 处理STACKING
                if (_spec.GameplayEffect.Stacking.stackingType == StackingType.None)
                {
                    _spec.RemoveSelf();
                }
                else
                {
                    if (_spec.GameplayEffect.Stacking.expirationPolicy == ExpirationPolicy.ClearEntireStack)
                    {
                        _spec.RemoveSelf();
                    }
                    else if (_spec.GameplayEffect.Stacking.expirationPolicy ==
                             ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                    {
                        if (_spec.StackCount > 1)
                        {
                            _spec.RefreshStack(_spec.StackCount - 1);
                            _spec.RefreshDuration();
                        }
                        else
                        {
                            _spec.RemoveSelf();
                        }
                    }
                    else if (_spec.GameplayEffect.Stacking.expirationPolicy == ExpirationPolicy.RefreshDuration)
                    {
                        //持续时间结束时,再次刷新Duration，这相当于无限Duration，
                        _spec.RefreshDuration();
                    }
                }
            }
        }

        public void MoveTick(MoveTickEvent e)
        {
            _spec.TriggerOnMoveTick(e);
            
            UpdateMovePeriod(e);

            if (_spec.DurationPolicy == EffectsDurationPolicy.MoveDuration && _spec.MoveDurationRemaining() <= 0)
            {
                // 处理STACKING
                if (_spec.GameplayEffect.Stacking.stackingType == StackingType.None)
                {
                    _spec.RemoveSelf();
                }
                else
                {
                    if (_spec.GameplayEffect.Stacking.expirationPolicy == ExpirationPolicy.ClearEntireStack)
                    {
                        _spec.RemoveSelf();
                    }
                    else if (_spec.GameplayEffect.Stacking.expirationPolicy ==
                             ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                    {
                        if (_spec.StackCount > 1)
                        {
                            _spec.RefreshStack(_spec.StackCount - 1);
                            _spec.RefreshMoveDuration();
                        }
                        else
                        {
                            _spec.RemoveSelf();
                        }
                    }
                    else if (_spec.GameplayEffect.Stacking.expirationPolicy == ExpirationPolicy.RefreshDuration)
                    {
                        _spec.RefreshMoveDuration();
                    }
                }
            }
        }

        public void TurnTick(TurnTickEvent e)
        {
            _spec.TriggerOnTurnTick(e);
            UpdateTurnPeriod(e);

            if (_spec.DurationPolicy == EffectsDurationPolicy.TurnDuration && _spec.TurnDurationRemaining() <= 0)
            {
                // 处理STACKING
                if (_spec.GameplayEffect.Stacking.stackingType == StackingType.None)
                {
                    _spec.RemoveSelf();
                }
                else
                {
                    if (_spec.GameplayEffect.Stacking.expirationPolicy == ExpirationPolicy.ClearEntireStack)
                    {
                        _spec.RemoveSelf();
                    }
                    else if (_spec.GameplayEffect.Stacking.expirationPolicy ==
                             ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                    {
                        if (_spec.StackCount > 1)
                        {
                            _spec.RefreshStack(_spec.StackCount - 1);
                            _spec.RefreshTurnDuration();
                        }
                        else
                        {
                            _spec.RemoveSelf();
                        }
                    }
                    else if (_spec.GameplayEffect.Stacking.expirationPolicy == ExpirationPolicy.RefreshDuration)
                    {
                        _spec.RefreshTurnDuration();
                    }
                }
            }
        }

        /// <summary>
        /// 注意: Period 小于 0.01f 可能出现误差, 基本够用了
        /// </summary>
        private void UpdatePeriod()
        {
            // 前提: Period不会动态修改
            if (Period <= 0) return;

            var actualDuration = Time.time - _spec.ActivationTime;
            if (actualDuration < Mathf.Epsilon)
            {
                // 第一次执行
                return;
            }

            var dt = Time.deltaTime;
            var excessDuration = actualDuration - _spec.Duration;
            if (excessDuration >= 0)
            {
                // 如果超出了持续时间，就减去超出的时间, 此时应该是最后一次执行
                dt -= excessDuration;
                // 为了避免误差, 保证最后一次边界得到执行机会
                dt += 0.0001f;
            }

            _periodRemaining -= dt;

            while (_periodRemaining < 0)
            {
                // 不能直接将_periodRemaining置为0, 这将累计误差
                _periodRemaining += Period;
                _spec.PeriodExecution?.TriggerOnExecute();
            }
        }

        public void ResetPeriod()
        {
            _periodRemaining = Period;
        }

        public void ResetMovePeriod()
        {
            _movePeriodRemaining = MovePeriod;
        }

        public void ResetTurnPeriod()
        {
            _turnPeriodRemaining = TurnPeriod;
        }

        private void UpdateMovePeriod(MoveTickEvent e)
        {
            if (MovePeriod <= 0) return;

            var actualMoveDuration = e.CurMove - _spec.ActivationMove;
            if (actualMoveDuration <= 0)
                return;

            if (actualMoveDuration > _spec.MoveDuration)
                return;

            _movePeriodRemaining--;

            if (_movePeriodRemaining == 0)
            {
                _movePeriodRemaining += MovePeriod;
                _spec.PeriodExecution?.TriggerOnExecute();
            }
        }

        private void UpdateTurnPeriod(TurnTickEvent e)
        {
            if (TurnPeriod <= 0) return;

            var actualTurnDuration = e.CurTurn - _spec.ActivationTurn;
            if (actualTurnDuration < 0)
                return;

            if (actualTurnDuration > _spec.TurnDuration)
                return;

            _turnPeriodRemaining--;

            if (_turnPeriodRemaining == 0)
            {
                _turnPeriodRemaining += TurnPeriod;
                _spec.PeriodExecution?.TriggerOnExecute();
            }
        }
    }
}