using Content.Shared.EntityEffects;
using Content.Shared.Whitelist;

namespace Content.Trauma.Shared.FiringPin;

[RegisterComponent]
public sealed partial class FiringPinComponent : Component
{
    [DataField(required: true)]
    public List<EntityEffect> Effects = [];

    [DataField]
    public bool RaiseEffectsOnUser = false;

    [DataField(required: true)]
    public float DoAfterLength;

    [DataField]
    public EntityWhitelist? InstallWhitelist;

    [DataField]
    public EntityWhitelist? InstallBlacklist;

    [DataField]
    public EntityWhitelist? UserWhitelist;

    [DataField]
    public EntityWhitelist? UserBlacklist;

    [DataField]
    public LocId LocMessageInstalled = "firing-pin-installed";

    [DataField]
    public LocId LocMessageReplaced = "firing-pin-replaced";

    [DataField]
    public LocId LocMessageIncompatible = "firing-pin-incompatible";
}