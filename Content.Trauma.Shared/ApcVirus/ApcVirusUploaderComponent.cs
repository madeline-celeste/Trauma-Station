namespace Content.Trauma.Shared.ApcVirus;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ApcVirusUploaderComponent : Component
{
    /// <summary>
    /// The event that is broadcasted when the virus is uploaded.
    /// </summary>
    [DataField, NonSerialized]
    public object? ProductEvent;

    /// <summary>
    /// Whether it was spent.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IsSpent;

    /// <summary>
    /// Gamerule to add when the virus is uploaded.
    /// </summary>
    [DataField]
    public EntProtoId? ProductGamerule;

    [DataField(required: true)]
    public EntProtoId UploadEffect;

    [DataField]
    public float DoAfterLength = 1.0f;

    [DataField]
    public LocId LocExamineSpent = "virus-uploader-examine-spent";

    [DataField]
    public LocId LocExamineUnspent = "virus-uploader-examine-unspent";
}