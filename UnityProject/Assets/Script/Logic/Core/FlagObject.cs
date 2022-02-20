
namespace Saltyfish
{
    public enum GameFlag
    {
        None = 0,
        Paused = 1 << 1,
        RestartGame = 1 << 2,
        Loading = 1 << 3,
        IsBackingMainMenu = 1 << 4,
        IsRestartingGame = 1 << 5,
        IsInGameOver = 1 << 6,
    }

    public class FlagObject
    {
        protected uint m_Flag = 0;

        public uint GetCurrentFlag()
        {
            return m_Flag;
        }

        public bool NoFlag
        {
            get
            {
                return m_Flag == 0;
            }
        }

        public bool HasFlag(uint flag)
        {
            return (m_Flag & flag) == flag;
        }

        public void AddFlag(uint flag)
        {
            m_Flag |= flag;
        }

        public void RemoveFlag(uint flag)
        {
            m_Flag &= ~flag;
        }

        public virtual void Clear()
        {
            m_Flag = 0;
        }
    }

    public class GenericEnumFlag<T> : FlagObject where T : System.Enum
    {
        public bool HasFlag(T flag)
        {
            uint val = (uint)flag.GetHashCode();
            return HasFlag(val);
        }

        public void AddFlag(T flag)
        {
            uint val = (uint)flag.GetHashCode();
            AddFlag(val);
        }

        public void RemoveFlag(T flag)
        {
            uint val = (uint)flag.GetHashCode();
            RemoveFlag(val);
        }
    }

    /// <summary>
    ///  游戏运行标志
    /// </summary>
    public class GameFlagObject : GenericEnumFlag<GameFlag> { }

}
