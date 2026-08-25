using Content.Shared.DoAfter;
using Content.Shared.Examine;
using JetBrains.Annotations;

namespace Content.Trauma.Shared.ApcVirus;

[Serializable, NetSerializable]
public sealed partial class VirusUploaderDoAfterEvent : SimpleDoAfterEvent;

public abstract partial class SharedApcVirusSystem : EntitySystem
{
    [SubscribeLocalEvent, UsedImplicitly]
    private void OnExamine(Entity<ApcVirusUploaderComponent> uploader, ref ExaminedEvent args)
    {
        args.PushMarkup(uploader.Comp.IsSpent
            ? Loc.GetString(uploader.Comp.LocExamineSpent)
            : Loc.GetString(uploader.Comp.LocExamineUnspent));
    }
}