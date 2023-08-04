using Beatmap.Base;
using Beatmap.Base.Customs;
using SimpleJSON;
using UnityEngine;

namespace Beatmap.Containers
{
    public class CustomEventContainer : ObjectContainer
    {
        private CustomEventGridContainer collection;
        public BaseCustomEvent CustomEventData;

        public override BaseObject ObjectData
        {
            get => CustomEventData;
            set => CustomEventData = (BaseCustomEvent)value;
        }

        public static CustomEventContainer SpawnCustomEvent(BaseCustomEvent data,
            CustomEventGridContainer collection, ref GameObject prefab)
        {
            var container = Instantiate(prefab).GetComponent<CustomEventContainer>();
            container.CustomEventData = data;
            container.collection = collection;
            return container;
        }

        public override void UpdateGridPosition()
        {
            var track = CustomEventData.DataParentTrack ?? CustomEventData.CustomTrack switch {
                JSONString s => s.Value,
                JSONArray arr => arr[0].Value,
            };
            var x = collection.EventTracks.IndexOf(track);
            transform.localPosition = new Vector3(
                x,
                0.5f,
                CustomEventData.SongBpmTime * EditorScaleController.EditorScale);
            UpdateCollisionGroups();
        }
    }
}
