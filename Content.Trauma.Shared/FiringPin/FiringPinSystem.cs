using Content.Shared.DoAfter;
using Content.Shared.EntityEffects;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using JetBrains.Annotations;
using Robust.Shared.Player;

namespace Content.Trauma.Shared.FiringPin;

[Serializable, NetSerializable]
public sealed partial class FiringPinInstallDoAfterEvent : SimpleDoAfterEvent;

public sealed partial class FiringPinSystem : EntitySystem
{
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private EntityWhitelistSystem _whitelist = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedEntityEffectsSystem _effect = default!;
    [Dependency] private SharedGunSystem _gun = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    // instead of ShotAttemptedEvent so cooldown applies before check
    // shit event naming is my passion
    [SubscribeLocalEvent, UsedImplicitly]
    private void OnAttemptShoot(Entity<InstalledFiringPinComponent> ent, ref AttemptShootEvent args)
    {
        var pin = ent.Comp;
        Log.Error(Name(args.User));

        if (_whitelist.CheckBoth(args.User, pin.UserBlacklist, pin.UserWhitelist))
        {
            Log.Error("wl pass");
            return;
        }

        Log.Error("Fucki hel");

        args.Cancelled = true;
        _effect.ApplyEffects(pin.RaiseEffectsOnUser ? args.User : ent, pin.Effects.ToArray());
    }

    [SubscribeLocalEvent, UsedImplicitly]
    private void OnInteractWith(Entity<GunComponent> gun, ref InteractUsingEvent args)
    {
        if (!TryComp(args.Used, out FiringPinComponent? pin))
            return;
        if (!_whitelist.CheckBoth(gun, pin.InstallBlacklist, pin.InstallWhitelist))
        {
            _popup.PopupEntity(Loc.GetString(pin.LocMessageIncompatible), gun);
            return;
        }

        var doArgs = new DoAfterArgs(
            entManager: EntityManager,
            user: args.User,
            seconds: pin.DoAfterLength,
            @event: new FiringPinInstallDoAfterEvent(),
            eventTarget: gun,
            target: gun,
            args.Used
        )
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = true,
            MultiplyDelay = true
        };

        _doAfter.TryStartDoAfter(doArgs);
        args.Handled = true;
    }

    [SubscribeLocalEvent, UsedImplicitly]
    private void OnDoAfter(Entity<GunComponent> gun, ref FiringPinInstallDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Used is null
            || !TryComp(args.Used, out FiringPinComponent? pin))
            return;

        var isReplace = HasComp<InstalledFiringPinComponent>(gun);
        var comp = EnsureComp<InstalledFiringPinComponent>(gun);
        comp.Effects = pin.Effects;
        comp.RaiseEffectsOnUser = pin.RaiseEffectsOnUser;
        comp.UserBlacklist = pin.UserBlacklist;
        comp.UserWhitelist = pin.UserWhitelist;

        _popup.PopupEntity(isReplace
            ? Loc.GetString(pin.LocMessageReplaced, ("item", Identity.Name(args.Used.Value, EntityManager)))
            : Loc.GetString(pin.LocMessageInstalled, ("item", Identity.Name(args.Used.Value, EntityManager))),
        gun, args.User);

        PredictedDel(args.Used);

        gun.Comp.ClumsyProof = true; // handled by firing pin

        args.Handled = true;
    }
}