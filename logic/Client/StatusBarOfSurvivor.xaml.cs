﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Protobuf;

namespace Client
{
    /// <summary>
    /// StatusBarOfSurvivor.xaml 的交互逻辑
    /// </summary>
    public partial class StatusBarOfSurvivor : UserControl
    {
        public StatusBarOfSurvivor(Grid parent, int Row, int Column)
        {
            InitializeComponent();
            parent.Children.Add(this);
            Grid.SetColumn(this, Column);
            Grid.SetRow(this, Row);
            initialized = false;
        }
        public void SetFontSize(double fontsize)
        {
            serial.FontSize = scores.FontSize = status.FontSize = activeSkill0.FontSize = activeSkill1.FontSize = activeSkill2.FontSize = prop0.FontSize = prop1.FontSize = prop2.FontSize = prop3.FontSize = fontsize;
        }

        private void SetStaticValue(MessageOfStudent obj)
        {
            switch (obj.StudentType)  // coolTime参数未设定，
            {
                case StudentType.Athlete:
                    coolTime0 = coolTime1 = coolTime2 = 10000;
                    serial.Text = "👥" + Convert.ToString(2) + "🧓" + Convert.ToString(obj.PlayerId) + "\n职业: Athlete";
                    break;
                case StudentType._2:
                    coolTime0 = coolTime1 = coolTime2 = 20000;
                    serial.Text = "👥" + Convert.ToString(2) + "🧓" + Convert.ToString(obj.PlayerId) + "\n职业: StudentType2";
                    break;
                case StudentType._3:
                    coolTime0 = coolTime1 = coolTime2 = 30000;
                    serial.Text = "👥" + Convert.ToString(2) + "🧓" + Convert.ToString(obj.PlayerId) + "\n职业: StudentType3";
                    break;
                case StudentType._4:
                    coolTime0 = coolTime1 = coolTime2 = 40000;
                    serial.Text = "👥" + Convert.ToString(2) + "🧓" + Convert.ToString(obj.PlayerId) + "\n职业: StudentType4";
                    break;
                case StudentType.NullStudentType:
                    coolTime0 = coolTime1 = coolTime2 = -1;
                    serial.Text = "👥" + Convert.ToString(2) + "🧓" + Convert.ToString(obj.PlayerId) + "\n职业: NullStudentType";
                    break;
            }
            activeSkill0.Text = "Skill0";
            activeSkill1.Text = "Skill1";
            activeSkill2.Text = "Skill2";
            initialized = true;
        }
        private void SetDynamicValue(MessageOfStudent obj)
        {
            int life = obj.Determination, death = obj.Addiction;
            switch (obj.PlayerState)
            {
                case PlayerState.Idle:
                    status.Text = "♥: " + Convert.ToString(life) + "\nIdle";
                    break;
                case PlayerState.Learning:
                    status.Text = "♥: " + Convert.ToString(life) + "\nLearning";
                    break;
                case PlayerState.Addicted:
                    status.Text = "💀: " + Convert.ToString(death) + "\nAddicted";
                    break;
                case PlayerState.Graduated:
                    status.Text = status.Text = "♥" + "\nGraduated";
                    break;
                case PlayerState.Quit:
                    status.Text = "💀" + "\nQuit";
                    break;
                case PlayerState.Treated:
                    status.Text = "♥: " + Convert.ToString(life) + "\nTreated";
                    break;
                case PlayerState.Rescued:
                    status.Text = "💀: " + Convert.ToString(death) + "\nRescued";
                    break;
                case PlayerState.Stunned:
                    status.Text = "♥: " + Convert.ToString(life) + "\nStunned";
                    break;
                case PlayerState.Treating:
                    status.Text = "♥: " + Convert.ToString(life) + "\nTreating";
                    break;
                case PlayerState.Rescuing:
                    status.Text = "♥: " + Convert.ToString(life) + "\nRescuing";
                    break;
                case PlayerState.Swinging:
                    status.Text = "♥: " + Convert.ToString(life) + "\nSwinging";
                    break;
                case PlayerState.Attacking:
                    status.Text = "♥: " + Convert.ToString(life) + "\nAttacking";
                    break;
                case PlayerState.Locking:
                    status.Text = "♥: " + Convert.ToString(life) + "\nLocking";
                    break;
                case PlayerState.Rummaging:
                    status.Text = "♥: " + Convert.ToString(life) + "\nRummaging";
                    break;
                case PlayerState.Climbing:
                    status.Text = "♥: " + Convert.ToString(life) + "\nClimbing";
                    break;
                case PlayerState.OpeningAChest:
                    status.Text = "♥: " + Convert.ToString(life) + "\nOpeningAChest";
                    break;
                case PlayerState.UsingSpecialSkill:
                    status.Text = "♥: " + Convert.ToString(life) + "\nUsingSpecialSkill";
                    break;
                case PlayerState.OpeningAGate:
                    status.Text = "♥: " + Convert.ToString(life) + "\nOpeningAGate";
                    break;
                default:
                    break;
            }
            scores.Text = "Scores: " + obj.Score;
            if (obj.TimeUntilSkillAvailable[0] >= 0)
                skillprogress0.Value = 100 - obj.TimeUntilSkillAvailable[0] / coolTime0 * 100;
            if (obj.TimeUntilSkillAvailable[1] >= 0)
                skillprogress1.Value = 100 - obj.TimeUntilSkillAvailable[1] / coolTime1 * 100;
            if (obj.TimeUntilSkillAvailable[2] >= 0)
                skillprogress2.Value = 100 - obj.TimeUntilSkillAvailable[2] / coolTime2 * 100;
            if (obj.PlayerState == PlayerState.Quit)
            {
                skillprogress0.Value = skillprogress1.Value = skillprogress2.Value = 0;
                skillprogress0.Background = skillprogress1.Background = skillprogress2.Background = Brushes.Gray;
            }
            else
                skillprogress0.Background = skillprogress1.Background = skillprogress2.Background = Brushes.White;
            int cnt = 0;
            foreach (var icon in obj.Prop)
            {
                switch (cnt)
                {
                    case 0:
                        switch (icon)
                        {
                            case Protobuf.PropType.Key3:
                                prop0.Text = "🔑";
                                break;
                            case Protobuf.PropType.Key5:
                                prop0.Text = "🔑";
                                break;
                            case Protobuf.PropType.Key6:
                                prop0.Text = "🔑";
                                break;
                            case Protobuf.PropType.AddSpeed:
                                prop0.Text = "⛸";
                                break;
                            case Protobuf.PropType.AddLifeOrAp:
                                prop0.Text = "🏅";
                                break;
                            case Protobuf.PropType.AddHpOrAp:
                                prop0.Text = "♥";
                                break;
                            case Protobuf.PropType.ShieldOrSpear:
                                prop0.Text = "🛡";
                                break;
                            default:
                                prop0.Text = "";
                                break;
                        }
                        cnt++;
                        break;
                    case 1:
                        switch (icon)
                        {
                            case Protobuf.PropType.Key3:
                                prop1.Text = "🔑";
                                break;
                            case Protobuf.PropType.Key5:
                                prop1.Text = "🔑";
                                break;
                            case Protobuf.PropType.Key6:
                                prop1.Text = "🔑";
                                break;
                            case Protobuf.PropType.AddSpeed:
                                prop1.Text = "⛸";
                                break;
                            case Protobuf.PropType.AddLifeOrAp:
                                prop1.Text = "🏅";
                                break;
                            case Protobuf.PropType.AddHpOrAp:
                                prop1.Text = "♥";
                                break;
                            case Protobuf.PropType.ShieldOrSpear:
                                prop1.Text = "🛡";
                                break;
                            default:
                                prop1.Text = "";
                                break;
                        }
                        cnt++;
                        break;
                    case 2:
                        switch (icon)
                        {
                            case Protobuf.PropType.Key3:
                                prop2.Text = "🔑";
                                break;
                            case Protobuf.PropType.Key5:
                                prop2.Text = "🔑";
                                break;
                            case Protobuf.PropType.Key6:
                                prop2.Text = "🔑";
                                break;
                            case Protobuf.PropType.AddSpeed:
                                prop2.Text = "⛸";
                                break;
                            case Protobuf.PropType.AddLifeOrAp:
                                prop2.Text = "🏅";
                                break;
                            case Protobuf.PropType.AddHpOrAp:
                                prop2.Text = "♥";
                                break;
                            case Protobuf.PropType.ShieldOrSpear:
                                prop2.Text = "🛡";
                                break;
                            default:
                                prop2.Text = "";
                                break;
                        }
                        cnt++;
                        break;
                    case 3:
                        switch (icon)
                        {
                            case Protobuf.PropType.Key3:
                                prop3.Text = "🔑";
                                break;
                            case Protobuf.PropType.Key5:
                                prop3.Text = "🔑";
                                break;
                            case Protobuf.PropType.Key6:
                                prop3.Text = "🔑";
                                break;
                            case Protobuf.PropType.AddSpeed:
                                prop3.Text = "⛸";
                                break;
                            case Protobuf.PropType.AddLifeOrAp:
                                prop3.Text = "🏅";
                                break;
                            case Protobuf.PropType.AddHpOrAp:
                                prop3.Text = "♥";
                                break;
                            case Protobuf.PropType.ShieldOrSpear:
                                prop3.Text = "🛡";
                                break;
                            default:
                                prop3.Text = "";
                                break;
                        }
                        cnt++;
                        break;
                    default:
                        break;
                }
            }
        }
        public void SetValue(MessageOfStudent obj)
        {
            if (!initialized)
                SetStaticValue(obj);
            SetDynamicValue(obj);
        }
        private int coolTime0, coolTime1, coolTime2;
        private bool initialized;
    }
}
