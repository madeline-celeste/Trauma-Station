using Content.Shared.EntityEffects;
using Content.Shared.Whitelist;

namespace Content.Trauma.Shared.FiringPin;

[RegisterComponent]
public sealed partial class InstalledFiringPinComponent : Component
{
    [DataField(required: true)]
    public List<EntityEffect> Effects = [];

    [DataField]
    public bool RaiseEffectsOnUser = false;

    [DataField]
    public EntityWhitelist? UserWhitelist;

    [DataField]
    public EntityWhitelist? UserBlacklist;
}