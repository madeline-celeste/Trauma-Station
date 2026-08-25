using Content.Server.DoAfter;
using Content.Server.GameTicking;
using Content.Server.Power.Components;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Systems;
using Content.Shared.Interaction;
using Content.Trauma.Shared.ApcVirus;
using JetBrains.Annotations;

namespace Content.Trauma.Server.ApcVirus;

public sealed partial class ApcVirusSystem : SharedApcVirusSystem
{
    [Dependency] private DoAfterSystem _doAfter = default!;
    [Dependency] private GameTicker _tickTockClock = default!;

    // apc comp only exist on server cuz hell if i know
    [SubscribeLocalEvent, UsedImplicitly]
    private void OnInteractWith(Entity<ApcComponent> apc, ref InteractUsingEvent args)
    {
        if (!TryComp(args.Used, out ApcVirusUploaderComponent? uploader)
            || uploader.IsSpent)
            return;

        var doArgs = new DoAfterArgs(
            entManager: EntityManager,
            user: args.User,
            seconds: uploader.DoAfterLength,
            @event: new VirusUploaderDoAfterEvent(),
            eventTarget: apc,
            target: apc,
            used: args.Used
        )
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = true,
            MultiplyDelay = true // elite human hackers
        };

        _doAfter.TryStartDoAfter(doArgs);
        args.Handled = true;
    }

    [SubscribeLocalEvent, UsedImplicitly]
    private void OnDoAfter(Entity<ApcComponent> apc, ref VirusUploaderDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Used is null
            || !TryComp(args.Used, out ApcVirusUploaderComponent? uploader))
            return;

        args.Handled = true;

        if (uploader.ProductEvent is { })
            RaiseLocalEvent(apc, uploader.ProductEvent);

        // well This is stupid, but i guess its how you do it
        if (uploader.ProductGamerule is { })
            _tickTockClock.AddGameRule(uploader.ProductGamerule);

        uploader.IsSpent = true;
        Dirty(args.Used.Value, uploader);

        // hate it
        var emaggedEvent = new GotEmaggedEvent(args.User, EmagType.Interaction);
        RaiseLocalEvent(apc, ref emaggedEvent); // jesse why are you blue

        SpawnAtPosition(uploader.UploadEffect, Transform(apc).Coordinates);
    }
}