using UnityEngine;

namespace TopDownSurvivor.Core
{
    /// <summary>
    /// 游戏状态枚举。
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        LevelUp,
        GameOver
    }

    /// <summary>
    /// 游戏状态变化事件，通过 EventBus 广播。
    /// </summary>
    public readonly struct GameStateChangedEvent
    {
        public readonly GameState Previous;
        public readonly GameState Current;

        public GameStateChangedEvent(GameState previous, GameState current)
        {
            Previous = previous;
            Current = current;
        }
    }

    /// <summary>
    /// 游戏总控管理器（单例）：维护当前游戏状态，并广播状态变化。
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GameState _currentState = GameState.Playing;

        public GameState CurrentState => _currentState;

        public void ChangeState(GameState newState)
        {
            if (_currentState == newState)
            {
                return;
            }

            GameState previous = _currentState;
            _currentState = newState;
            EventBus<GameStateChangedEvent>.Publish(new GameStateChangedEvent(previous, newState));
        }
    }
}
