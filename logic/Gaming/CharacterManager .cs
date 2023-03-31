﻿using System;
using System.Threading;
using System.Collections.Generic;
using GameClass.GameObj;
using Preparation.Utility;
using GameEngine;
using Preparation.Interface;
using Timothy.FrameRateTask;
using System.Numerics;

namespace Gaming
{
    public partial class Game
    {
        private readonly CharacterManager characterManager;
        private class CharacterManager
        {
            readonly Map gameMap;
            public CharacterManager(Map gameMap)
            {
                this.gameMap = gameMap;
            }


            public Character? AddPlayer(XY pos, int teamID, int playerID, CharacterType characterType, Character? parent = null)
            {
                Character newPlayer;

                if (characterType == CharacterType.Robot)
                {
                    newPlayer = new Golem(pos, GameData.characterRadius, parent);
                }
                else newPlayer = (GameData.IsGhost(characterType)) ? new Ghost(pos, GameData.characterRadius, characterType) : new Student(pos, GameData.characterRadius, characterType);
                gameMap.Add(newPlayer);

                newPlayer.TeamID = teamID;
                newPlayer.PlayerID = playerID;
                #region 人物装弹
                new Thread
                (
                    () =>
                    {
                        while (!gameMap.Timer.IsGaming)
                            Thread.Sleep(Math.Max(newPlayer.CD, GameData.checkInterval));
                        long lastTime = Environment.TickCount64;
                        new FrameRateTaskExecutor<int>(
                            loopCondition: () => gameMap.Timer.IsGaming && !newPlayer.IsResetting,
                            loopToDo: () =>
                            {
                                long nowTime = Environment.TickCount64;
                                if (newPlayer.BulletNum == newPlayer.MaxBulletNum)
                                    lastTime = nowTime;
                                else if (nowTime - lastTime >= newPlayer.CD)
                                {
                                    _ = newPlayer.TryAddBulletNum();
                                    lastTime = nowTime;
                                }
                            },
                            timeInterval: GameData.checkInterval,
                            finallyReturn: () => 0
                        )
                        {
                            AllowTimeExceed = true/*,
                        MaxTolerantTimeExceedCount = 5,
                        TimeExceedAction = exceedTooMuch =>
                        {
                            if (exceedTooMuch) Console.WriteLine("The computer runs too slow that it cannot check the color below the player in time!");
                        }*/
                        }
                            .Start();
                    }
                )
                { IsBackground = true }.Start();
                #endregion
                #region BGM,牵制得分更新
                new Thread
                (
                    () =>
                    {
                        while (!gameMap.Timer.IsGaming)
                            Thread.Sleep(GameData.checkInterval);
                        int TimePinningDown = 0, ScoreAdded = 0;

                        bool noise = false;
                        if (!newPlayer.IsGhost())
                        {
                            gameMap.GameObjLockDict[GameObjType.Character].EnterReadLock();
                            try
                            {
                                foreach (Character person in gameMap.GameObjDict[GameObjType.Character])
                                {
                                    if (person.IsGhost())
                                    {
                                        if (person.CharacterType == CharacterType.ANoisyPerson)
                                        {
                                            noise = true;
                                            newPlayer.AddBgm(BgmType.GhostIsComing, 1411180);
                                            newPlayer.AddBgm(BgmType.GeneratorIsBeingFixed, 154991);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                gameMap.GameObjLockDict[GameObjType.Character].ExitReadLock();
                            }
                        }
                        new FrameRateTaskExecutor<int>(
                        loopCondition: () => gameMap.Timer.IsGaming && !newPlayer.IsResetting,
                        loopToDo: () =>
                        {
                            gameMap.GameObjLockDict[GameObjType.Character].EnterReadLock();
                            try
                            {
                                if (newPlayer.IsGhost())
                                {
                                    double bgmVolume = 0;
                                    foreach (Character person in gameMap.GameObjDict[GameObjType.Character])
                                    {
                                        if (!person.IsGhost() && XY.Distance(newPlayer.Position, person.Position) <= (newPlayer.AlertnessRadius / person.Concealment))
                                        {
                                            if ((double)newPlayer.AlertnessRadius / XY.Distance(newPlayer.Position, person.Position) > bgmVolume)
                                                bgmVolume = newPlayer.AlertnessRadius / XY.Distance(newPlayer.Position, person.Position);
                                        }
                                    }
                                    if (bgmVolume > 0)
                                        newPlayer.AddBgm(BgmType.StudentIsApproaching, bgmVolume);
                                }
                                else
                                {
                                    foreach (Character person in gameMap.GameObjDict[GameObjType.Character])
                                    {
                                        if (person.IsGhost())
                                        {
                                            if (!noise && XY.Distance(newPlayer.Position, person.Position) <= (newPlayer.AlertnessRadius / person.Concealment))
                                                newPlayer.AddBgm(BgmType.GhostIsComing, (double)newPlayer.AlertnessRadius / XY.Distance(newPlayer.Position, person.Position));
                                            if (XY.Distance(newPlayer.Position, person.Position) <= GameData.basicViewRange)
                                            {
                                                TimePinningDown += GameData.checkInterval;
                                                newPlayer.AddScore(GameData.StudentScorePinDown(TimePinningDown) - ScoreAdded);
                                                ScoreAdded = GameData.StudentScorePinDown(TimePinningDown);
                                            }
                                            else TimePinningDown = ScoreAdded = 0;
                                            break;
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                gameMap.GameObjLockDict[GameObjType.Character].ExitReadLock();
                            }

                            if (!noise)
                            {
                                gameMap.GameObjLockDict[GameObjType.Generator].EnterReadLock();
                                try
                                {
                                    double bgmVolume = 0;
                                    foreach (Generator generator in gameMap.GameObjDict[GameObjType.Generator])
                                    {
                                        if (XY.Distance(newPlayer.Position, generator.Position) <= newPlayer.AlertnessRadius)
                                        {
                                            if ((double)newPlayer.AlertnessRadius * generator.DegreeOfRepair / GameData.degreeOfFixedGenerator / XY.Distance(newPlayer.Position, generator.Position) > bgmVolume)
                                                bgmVolume = (double)newPlayer.AlertnessRadius * generator.DegreeOfRepair / GameData.degreeOfFixedGenerator / XY.Distance(newPlayer.Position, generator.Position);
                                        }
                                    }
                                    if (bgmVolume > 0)
                                        newPlayer.AddBgm(BgmType.GeneratorIsBeingFixed, bgmVolume);
                                }
                                finally
                                {
                                    gameMap.GameObjLockDict[GameObjType.Generator].ExitReadLock();
                                }
                            }
                        },
                        timeInterval: GameData.checkInterval,
                        finallyReturn: () => 0
                    )
                        {
                            AllowTimeExceed = true/*,
                        MaxTolerantTimeExceedCount = 5,
                        TimeExceedAction = exceedTooMuch =>
                        {
                            if (exceedTooMuch) Console.WriteLine("The computer runs too slow that it cannot check the color below the player in time!");
                        }*/
                        }
                        .Start();
                    }
                )
                { IsBackground = true }.Start();
                #endregion

                return newPlayer;
            }

            public void BeAddictedToGame(Student player, Ghost ghost)
            {
                if (player.CharacterType == CharacterType.Robot)
                {
                    ghost.AddScore(GameData.TrickerScoreDestroyRobot);
                    Die(player);
                    return;
                }
                ghost.AddScore(GameData.TrickerScoreStudentBeAddicted);
                new Thread
                    (() =>
                    {
                        if (player.GamingAddiction > GameData.BeginGamingAddiction && player.GamingAddiction < GameData.MidGamingAddiction)
                            player.GamingAddiction = GameData.MidGamingAddiction;
                        player.PlayerState = PlayerStateType.Addicted;
#if DEBUG
                        Debugger.Output(player, " is addicted ");
#endif
                        new FrameRateTaskExecutor<int>(
                            () => (player.PlayerState == PlayerStateType.Addicted || player.PlayerState == PlayerStateType.Rescued) && player.GamingAddiction < player.MaxGamingAddiction && gameMap.Timer.IsGaming,
                            () =>
                            {
                                player.GamingAddiction += (player.PlayerState == PlayerStateType.Addicted) ? GameData.frameDuration : 0;
                            },
                            timeInterval: GameData.frameDuration,
                            () =>
                            {
                                if (player.GamingAddiction == player.MaxGamingAddiction && gameMap.Timer.IsGaming)
                                {
                                    ghost.AddScore(GameData.TrickerScoreStudentDie);
                                    Die(player);
                                }
                                return 0;
                            }
                        )
                            .Start();
                    }
                    )
                { IsBackground = true }.Start();
            }

            public static bool BeStunned(Character player, int time)
            {
                if (player.PlayerState == PlayerStateType.Stunned || player.NoHp() || player.CharacterType == CharacterType.Robot) return false;
                new Thread
                    (() =>
                    {
                        player.PlayerState = PlayerStateType.Stunned;
                        Thread.Sleep(time);
                        if (player.PlayerState == PlayerStateType.Stunned)
                            player.PlayerState = PlayerStateType.Null;
                    }
                    )
                { IsBackground = true }.Start();
                return true;
            }
            public static bool BackSwing(Character? player, int time)
            {
                if (player == null || time <= 0) return false;
                if (player.PlayerState == PlayerStateType.Swinging || (!player.Commandable() && player.PlayerState != PlayerStateType.TryingToAttack)) return false;
                player.PlayerState = PlayerStateType.Swinging;

                new Thread
                        (() =>
                        {
                            Thread.Sleep(time);

                            if (player.PlayerState == PlayerStateType.Swinging)
                            {
                                player.PlayerState = PlayerStateType.Null;
                            }
                        }
                        )
                { IsBackground = true }.Start();
                return true;
            }

            private void Die(Character player)
            {
#if DEBUG
                Debugger.Output(player, "die.");
#endif
                player.Die(PlayerStateType.Deceased);

                for (int i = 0; i < GameData.maxNumOfPropInPropInventory; i++)
                {
                    Prop? prop = player.UseProp(i);
                    if (prop != null)
                    {
                        prop.ReSetPos(player.Position, gameMap.GetPlaceType(player.Position));
                        gameMap.Add(prop);
                    }
                }
                if (player.CharacterType == CharacterType.Robot) return;
                ++gameMap.NumOfDeceasedStudent;
                if (GameData.numOfStudent - gameMap.NumOfDeceasedStudent - gameMap.NumOfEscapedStudent == 1)
                {
                    gameMap.GameObjLockDict[GameObjType.EmergencyExit].EnterReadLock();
                    try
                    {
                        foreach (EmergencyExit emergencyExit in gameMap.GameObjDict[GameObjType.EmergencyExit])
                            if (emergencyExit.CanOpen)
                            {
                                emergencyExit.IsOpen = true;
                                break;
                            }
                    }
                    finally
                    {
                        gameMap.GameObjLockDict[GameObjType.EmergencyExit].ExitReadLock();
                    }
                }
            }

        }
    }
}