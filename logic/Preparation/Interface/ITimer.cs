﻿
namespace Preparation.Interface
{
    public interface ITimer
    {
        bool IsGaming { get; set; }
        public bool StartGame(int timeInMilliseconds);
    }
}
