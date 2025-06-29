namespace RebornRotations.Melee;

[Rotation("Default", CombatType.PvE, GameVersion = "7.25")]
[SourceCode(Path = "main/BasicRotations/Melee/DRG_Default.cs")]
[Api(4)]

public sealed class DRG_Default : DragoonRotation
{
    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Use Doom Spike for damage uptime if out of melee range even if it breaks combo")]
    public bool DoomSpikeWhenever { get; set; } = true;

    [RotationConfig(CombatType.PvE, Name = "Attempt to assign Stardiver to the first ogcd slot (Experimental)")]
    public bool OGCDTimers { get; set; } = false;
    #endregion

    private static bool InBurstStatus => Player.HasStatus(true, StatusID.BattleLitany);

    #region Additional oGCD Logic

    [RotationDesc(ActionID.WingedGlidePvE)]
    protected override bool MoveForwardAbility(IAction nextGCD, out IAction? act)
    {
        if (WingedGlidePvE.CanUse(out act, skipComboCheck: true))
        {
            return true;
        }
        return base.MoveForwardAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.ElusiveJumpPvE)]
    protected override bool MoveBackAbility(IAction nextGCD, out IAction? act)
    {
        if (ElusiveJumpPvE.CanUse(out act, skipComboCheck: true))
        {
            return true;
        }
        return base.MoveBackAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.FeintPvE)]
    protected sealed override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (FeintPvE.CanUse(out act, skipComboCheck: true))
        {
            return true;
        }
        return base.DefenseAreaAbility(nextGCD, out act);
    }
    #endregion

    #region oGCD Logic
    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (IsBurst && InCombat)
        {
            bool lifeSurgeReady =
                ((Player.HasStatus(true, StatusID.BattleLitany) || Player.HasStatus(true, StatusID.LanceCharge) || LOTDEndAfter(1000))
                    && nextGCD.IsTheSameTo(true, HeavensThrustPvE, DrakesbanePvE))
                || (Player.HasStatus(true, StatusID.BattleLitany)
                    && Player.HasStatus(true, StatusID.LanceCharge)
                    && LOTDEndAfter(1000)
                    && nextGCD.IsTheSameTo(true, ChaoticSpringPvE, LanceBarragePvE, WheelingThrustPvE, FangAndClawPvE))
                || (nextGCD.IsTheSameTo(true, HeavensThrustPvE, DrakesbanePvE)
                    && (LanceChargePvE.Cooldown.IsCoolingDown || BattleLitanyPvE.Cooldown.IsCoolingDown));

            if ((!BattleLitanyPvE.Cooldown.ElapsedAfter(60) || !BattleLitanyPvE.EnoughLevel) 
                && LanceChargePvE.CanUse(out act))
                {
                    return true;
                }

            if (Player.HasStatus(true, StatusID.LanceCharge) && BattleLitanyPvE.CanUse(out act))
                {
                    return true;
                }

            if (((Player.HasStatus(true, StatusID.BattleLitany) || Player.HasStatus(true, StatusID.LanceCharge) || LOTDEndAfter(1000)) 
                && nextGCD.IsTheSameTo(true, HeavensThrustPvE, DrakesbanePvE))
                || (Player.HasStatus(true, StatusID.BattleLitany) 
                    && Player.HasStatus(true, StatusID.LanceCharge)
                    && LOTDEndAfter(1000)
                    && !HeavensThrustPvE.EnoughLevel
                    && !DrakesbanePvE.EnoughLevel
                    && nextGCD.IsTheSameTo(true, ChaoticSpringPvE, LanceBarragePvE, WheelingThrustPvE, FangAndClawPvE))
                || (nextGCD.IsTheSameTo(true, HeavensThrustPvE, DrakesbanePvE)
                    && (LanceChargePvE.Cooldown.IsCoolingDown || BattleLitanyPvE.Cooldown.IsCoolingDown)))
                    {
                        if (LifeSurgePvE.CanUse(out act, usedUp: true))
                        {
                            return true;
                        }
                    }

            if (lifeSurgeReady
                || (!DisembowelPvE.EnoughLevel && nextGCD.IsTheSameTo(true, VorpalThrustPvE))
                || (!FullThrustPvE.EnoughLevel && nextGCD.IsTheSameTo(true, VorpalThrustPvE, DisembowelPvE))
                || (!LanceChargePvE.EnoughLevel && nextGCD.IsTheSameTo(true, DisembowelPvE, FullThrustPvE))
                || (!BattleLitanyPvE.EnoughLevel && nextGCD.IsTheSameTo(true, ChaosThrustPvE, FullThrustPvE)))
                    {
                        if (LifeSurgePvE.CanUse(out act, usedUp: true))
                        {
                            return true;
                        }
                    }
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (Player.HasStatus(true, StatusID.LanceCharge))
        {
            if (GeirskogulPvE.CanUse(out act))
            {
                return true;
            }
        }

        if ((BattleLitanyPvE.EnoughLevel && Player.HasStatus(true, StatusID.BattleLitany) && Player.HasStatus(true, StatusID.LanceCharge))
            || (!BattleLitanyPvE.EnoughLevel && Player.HasStatus(true, StatusID.LanceCharge)))
        {
            if (DragonfireDivePvE.CanUse(out act))
            {
                return true;
            }
        }

        if (Player.HasStatus(true, StatusID.BattleLitany) || Player.HasStatus(true, StatusID.LanceCharge) || LOTDEndAfter(1000)
            || nextGCD.IsTheSameTo(true, RaidenThrustPvE, DraconianFuryPvE))
        {
            if (WyrmwindThrustPvE.CanUse(out act, usedUp: true))
            {
                return true;
            }
        }

        if (JumpPvE.CanUse(out act))
        {
            return true;
        }

        if (HighJumpPvE.CanUse(out act))
        {
            return true;
        }

        if (StardiverPvE.CanUse(out act, isFirstAbility: OGCDTimers))
        {
            return true;
        }

        if (MirageDivePvE.CanUse(out act))
        {
            return true;
        }

        if (NastrondPvE.CanUse(out act))
        {
            return true;
        }

        if (StarcrossPvE.CanUse(out act))
        {
            return true;
        }

        if (RiseOfTheDragonPvE.CanUse(out act))
        {
            return true;
        }

        return base.AttackAbility(nextGCD, out act);
    }
    #endregion

    #region GCD Logic
    protected override bool GeneralGCD(out IAction? act)
    {
        bool doomSpikeRightNow = DoomSpikeWhenever;

        if (CoerthanTormentPvE.CanUse(out act))
        {
            return true;
        }

        if (SonicThrustPvE.CanUse(out act, skipStatusProvideCheck: true))
        {
            return true;
        }

        if (DoomSpikePvE.CanUse(out act, skipComboCheck: doomSpikeRightNow))
        {
            return true;
        }

        if (DrakesbanePvE.CanUse(out act))
        {
            return true;
        }

        if (FangAndClawPvE.CanUse(out act))
        {
            return true;
        }

        if (WheelingThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (FullThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (ChaosThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (SpiralBlowPvE.CanUse(out act))
        {
            return true;
        }

        if (DisembowelPvE.CanUse(out act))
        {
            return true;
        }

        if (LanceBarragePvE.CanUse(out act))
        {
            return true;
        }

        if (VorpalThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (RaidenThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (TrueThrustPvE.CanUse(out act))
        {
            return true;
        }

        if (!IsLastAction(true, WingedGlidePvE) && PiercingTalonPvE.CanUse(out act))
        {
            return true;
        }

        return base.GeneralGCD(out act);
    }
    #endregion
}
