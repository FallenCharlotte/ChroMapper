using System;
using System.Collections.Generic;
using Beatmap.Base;
using Beatmap.Base.Customs;
using Beatmap.Containers;
using Beatmap.Helper;
using Beatmap.V2.Customs;
using Beatmap.V3.Customs;
using SimpleJSON;
using UnityEngine;

public class
    CustomEventPlacement : PlacementController<BaseCustomEvent, CustomEventContainer, CustomEventGridContainer>
{
    public enum CustomEventTypes
    {
        AnimateTrack,
        AssignPathAnimation,
        AssignTrackParent,
        AssignPlayerToTrack,
        AnimateComponent
    }

    private CustomEventTypes currentType = CustomEventTypes.AnimateTrack;

    private readonly List<TextAsset> customEventDataPresets = new List<TextAsset>();

    public override int PlacementXMax => objectContainerCollection.EventsByTrack.Count;

    [HideInInspector] protected override bool CanClickAndDrag { get; set; } = false;

    protected override Vector2 vanillaOffset { get; } = new Vector2(0, -1.1f);

    internal override void Start()
    {
        gameObject.SetActive(Settings.Instance.AdvancedShit);
        foreach (var asset in Resources.LoadAll<TextAsset>("Custom Event Presets"))
            customEventDataPresets.Add(asset);
        Debug.Log($"Loaded {customEventDataPresets.Count} presets for custom events.");
        base.Start();
    }

    public void SetType(CustomEventTypes type)
    {
        currentType = type;
    }

    public override BeatmapAction GenerateAction(BaseObject spawned, IEnumerable<BaseObject> conflicting) =>
        new BeatmapObjectPlacementAction(spawned, conflicting, "Placed a Custom Event.");

    public override BaseCustomEvent GenerateOriginalData() => BeatmapFactory.CustomEvent();

    public override void OnPhysicsRaycast(Intersections.IntersectionHit _, Vector3 __)
    {
        var localPosition = instantiatedContainer.transform.localPosition;
        instantiatedContainer.transform.localPosition = new Vector3(localPosition.x, 0.5f, localPosition.z);
        var customTrackId = Mathf.CeilToInt(localPosition.x);
        if (customTrackId >= 0 && customTrackId < objectContainerCollection.EventsByTrack.Count)
            queuedData.CustomTrack = objectContainerCollection.EventTracks[customTrackId];
        queuedData.Type = (currentType == CustomEventTypes.AnimateComponent && !Settings.Instance.Load_MapV3)
            ? "AssignFogTrack"
            : currentType.ToString();
    }

    internal override void ApplyToMap()
    {
        queuedData.Data = new JSONObject();

        base.ApplyToMap();
    }

    public override void TransferQueuedToDraggedObject(ref BaseCustomEvent dragged, BaseCustomEvent queued) { }
}
