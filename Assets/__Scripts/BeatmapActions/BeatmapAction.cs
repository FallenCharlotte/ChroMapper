﻿using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib.Utils;

/// <summary>
///     A BeatmapAction contains a BeatmapObjectContainer as well as a methods to Undo and Redo the action.
///     An action can consist of placing and deleting, as well as more advanced options, like modifying via Node Editor,
///     or using Strobe Generator
/// </summary>
public abstract class BeatmapAction : INetSerializable
{
    public bool Active = true;
    public bool Networked = false;
    public string Comment = "No comment.";
    public Guid Guid = Guid.NewGuid();
    public IEnumerable<BeatmapObject> Data;
    public MapperIdentityPacket Identity; // Only used in United Mapping, assume local user if null

    internal bool inCollection = false;

    public BeatmapAction() => Networked = true;

    public BeatmapAction(IEnumerable<BeatmapObject> data, string comment = "No comment.")
    {
        Data = data;
        Comment = comment;
    }

    /// <summary>
    ///     Steps that should be taken to Undo an Action.
    /// </summary>
    /// <param name="param">Collection of useful stuff.</param>
    public abstract void Undo(BeatmapActionContainer.BeatmapActionParams param);

    /// <summary>
    ///     Steps that should be taken to Redo an Action.
    /// </summary>
    /// <param name="param">Collection of useful stuff.</param>
    public abstract void Redo(BeatmapActionContainer.BeatmapActionParams param);
    
    /// <summary>
    /// Serializes the Action to be sent over the network in a Multi Mapping session.
    /// </summary>
    /// <param name="writer"></param>
    public abstract void Serialize(NetDataWriter writer);
    
    /// <summary>
    /// Deserializes and populates an Action received in a Multi Mapping session.
    /// </summary>
    /// <param name="reader"></param>
    public abstract void Deserialize(NetDataReader reader);

    public virtual BeatmapObject DoesInvolveObject(BeatmapObject obj) => Data.Any(it => it.IsConflictingWith(obj)) ? obj : null;

    protected void RefreshPools(IEnumerable<BeatmapObject> data)
    {
        foreach (var unique in data.DistinctBy(x => x.BeatmapType))
        {
            var collection = BeatmapObjectContainerCollection.GetCollectionForType(unique.BeatmapType);
            collection.RefreshPool(true);

            if (collection is BPMChangesContainer con) con.RefreshModifiedBeat();
        }
    }

    protected void SpawnObject(BeatmapObject obj, bool removeConflicting = false, bool refreshesPool = false)
        => BeatmapObjectContainerCollection.GetCollectionForType(obj.BeatmapType).SpawnObject(obj, removeConflicting, refreshesPool);

    protected void DeleteObject(BeatmapObject obj, bool refreshesPool = true)
    {
        var collection = BeatmapObjectContainerCollection.GetCollectionForType(obj.BeatmapType);

        // If this Action was received over the network, we abuse the conflict check system
        //   to delete anything that was previously there.
        if (Networked && !collection.UnsortedObjects.Contains(obj))
        {
            collection.RemoveConflictingObjects(new[] { obj });
            return;
        }

        collection.DeleteObject(obj, false, refreshesPool);
    }

    protected void SerializeBeatmapObjectList(NetDataWriter writer, IEnumerable<BeatmapObject> list)
    {
        writer.Put(list.Count());

        foreach (var obj in list) writer.PutBeatmapObject(obj);
    }

    protected IEnumerable<BeatmapObject> DeserializeBeatmapObjectList(NetDataReader reader)
    {
        var count = reader.GetInt();
        var deserializedObjects = new List<BeatmapObject>(count);

        for (var i = 0; i < count; i++)
        {
            deserializedObjects.Add(reader.GetBeatmapObject());
        }

        return deserializedObjects;
    }
}
